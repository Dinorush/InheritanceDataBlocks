using InheritanceDataBlocks.Utils;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;

namespace InheritanceDataBlocks.Inheritance
{

    public class InheritanceNode<T>
    {
        public uint ID { get; private set; }
        public uint ParentID { get; private set; }
        public List<PropertyInfo> Properties { get; private set; }
        public T Data { get; private set; }

        public InheritanceNode(uint id, T data, List<PropertyInfo> properties, uint parentID)
        {
            ID = id;
            Properties = properties;
            Data = data;
            ParentID = parentID;
        }
    }

    public class InheritanceRoot<T>
    {
        private readonly Dictionary<uint, InheritanceNode<T>> _nodes = new();

        public void Clear() => _nodes.Clear();

        public bool AddNode(uint ID, T data, List<PropertyInfo> properties, uint parentID) => AddNode(new InheritanceNode<T>(ID, data, properties, parentID));

        public bool AddNode(InheritanceNode<T> newNode)
        {
            // Check for dependency cycle
            for (InheritanceNode<T>? next = _nodes.GetValueOrDefault(newNode.ParentID); next != null; next = _nodes.GetValueOrDefault(next.ParentID))
            {
                if(next.ParentID == newNode.ID)
                {
                    IDBLogger.Error("Error adding ID " + newNode.ID + ": dependency cycle detected with ID " + next.ID
                                + " on data " + newNode.Data?.GetType().Name);
                    return false;
                }
            }

            _nodes[newNode.ID] = newNode;

            return true;
        }

        public bool RemoveNode(uint ID) => _nodes.Remove(ID);

        public ICollection<uint> GetIDs() => _nodes.Keys;
        public InheritanceNode<T>? GetNode(uint ID) => _nodes.GetValueOrDefault(ID);
        public bool TryGetNode(uint ID, [MaybeNullWhen(false)] out InheritanceNode<T> node) => _nodes.TryGetValue(ID, out node);

        public LinkedList<InheritanceNode<T>>? GetInheritanceList(uint ID)
        {
            if (!_nodes.ContainsKey(ID)) return null;

            LinkedList<InheritanceNode<T>> nodeList = new();
            for (InheritanceNode<T>? node = _nodes[ID]; node != null; node = _nodes.GetValueOrDefault(node.ParentID))
                nodeList.AddFirst(node);

            return nodeList;
        }

        public void DebugPrintAllPaths(string? groupName = null)
        {
            IDBLogger.Log("Printing all inheritance chains" + (groupName != null ? " for " + groupName : ""));
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
