using GameData;
using InheritanceDataBlocks.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InheritanceDataBlocks.InheritanceResolver
{
    public abstract class DataBlockNodeBase<T> where T : GameDataBlockBase<T>
    {
        public uint ID { get; private set; }
        // Exclusive list (does not include itself). Contains the next node on the path to the node with the key ID.
        protected readonly Dictionary<uint, DataBlockNode<T>> _subtreePath = new();
        // So any subclass can get the path dict of another
        protected static Dictionary<uint, DataBlockNode<T>> SubtreePath(DataBlockNodeBase<T> node) => node._subtreePath;

        public DataBlockNodeBase(uint ID) { this.ID = ID; }

        public ICollection<uint> GetIDs() { return _subtreePath.Keys; }
    }

    public class DataBlockNode<T> : DataBlockNodeBase<T> where T : GameDataBlockBase<T>
    {
        public uint ParentID { get; private set; }
        public string[] Names { get; private set; }
        public GameDataBlockBase<T> Block { get; private set; }

        public DataBlockNode(uint ID, GameDataBlockBase<T> block, string[] names, uint parentID) : base(ID)
        {
            Names = names;
            Block = block;
            ParentID = parentID;
        }
    }

    public class DataBlockRoot<T> : DataBlockNodeBase<T> where T : GameDataBlockBase<T>
    {
        public DataBlockRoot() : base(0) { }

        public void AddNode(DataBlockNode<T> newNode)
        {
            // Check for dependency cycle (
            if (_subtreePath.ContainsKey(newNode.ParentID) && _subtreePath[newNode.ParentID].ParentID == newNode.ID)
            {
                IDBLogger.Error("Error adding ID " + newNode.ID + ": dependency cycle detected with ID " + _subtreePath[newNode.ParentID].ID
                                + " on block " + newNode.Block.GetType().Name);
                return;
            }

            if (TryGetNode(newNode.ID, out var oldNode))
                RemoveNode(oldNode!);

            DataBlockNodeBase<T> node = this;
            // If there is a path to the parent, follow parent's path
            while (SubtreePath(node).ContainsKey(newNode.ParentID))
            {
                SubtreePath(node)[newNode.ID] = SubtreePath(node)[newNode.ParentID];
                node = SubtreePath(node)[newNode.ID];
            }

            // No path to parent node; node points directly to normal datablock
            SubtreePath(node)[newNode.ID] = newNode;

            // If the new node should be the parent of existing nodes, those nodes will be children of the root. Either
            // 1. Their parent ID was not a node (pointed to normal datablock)
            // 2. Their parent ID was newNode and they have been promoted to root as a result of the RemoveNode call
            foreach (var kv in _subtreePath.Where(kv => kv.Value.ParentID == newNode.ID).ToArray())
            {
                _subtreePath.Remove(kv.Key);
                AddNode(kv.Value);
            }
        }

        public virtual void RemoveNode(DataBlockNode<T> trgtNode)
        {
            DataBlockNodeBase<T> node = this;
            DataBlockNodeBase<T> next;
            while (SubtreePath(node).ContainsKey(trgtNode.ID))
            {
                next = SubtreePath(node)[trgtNode.ID];
                // Remove all nodes that are a child of the target node
                foreach (uint ID in trgtNode.GetIDs())
                    SubtreePath(node).Remove(ID);
                node = next;
            }

            // Move any IDs in the target node up to root (they no longer have a parent since chain is broken)
            foreach (uint ID in trgtNode.GetIDs())
                _subtreePath[ID] = SubtreePath(trgtNode)[ID];
        }

        public DataBlockNode<T>? GetNode(uint ID)
        {
            // Node is not in subtree, so it must not exist
            if (!_subtreePath.ContainsKey(ID))
                return null;

            DataBlockNode<T> node = _subtreePath[ID];
            while (node.ID != ID)
                node = SubtreePath(node)[ID];

            return node;
        }

        public bool TryGetNode(uint ID, out DataBlockNode<T>? node)
        {
            node = GetNode(ID);
            return node != null;
        }

        public LinkedList<DataBlockNode<T>>? GetInheritanceList(uint ID)
        {
            if (!_subtreePath.ContainsKey(ID)) return null;

            LinkedList<DataBlockNode<T>> nodeList = new();
            // If there is a path to the node (i.e. this is not the target), follow the path
            for (DataBlockNodeBase<T> node = this; SubtreePath(node).ContainsKey(ID); node = SubtreePath(node)[ID])
                nodeList.AddLast(SubtreePath(node)[ID]);

            return nodeList;
        }

        public void DebugPrintAllPaths()
        {
            IDBLogger.Log("Printing all inheritance chains for " + GameDataBlockBase<T>.m_fileNameNoExt);
            foreach(uint ID in GetIDs())
            {
                var list = GetInheritanceList(ID);
                StringBuilder str = new("ID " + ID + ": (Base) " + list!.First!.Value.ParentID.ToString());
                foreach(var node in list)
                {
                    str.Append(" -> " + node.ID);
                }
                IDBLogger.Log(str.ToString());
            }
        }
    }
}
