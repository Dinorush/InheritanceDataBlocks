using InheritanceDataBlocks.Utils;
using GameData;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace InheritanceDataBlocks.Inheritance
{
    internal interface IResolver
    {
        public void Reset();
        public void Run();
    }

    internal sealed class InheritanceResolver<T> : IResolver where T : GameDataBlockBase<T>
    {
        public static InheritanceResolver<T> Instance = new();
        
        // Use root node to handle nodes that don't have a parent (i.e. refer to vanilla)
        private InheritanceRoot<T> _root = new();
        private readonly Dictionary<string, PropertyInfo> _propertyCache = new();

        private bool _init = false;

        public InheritanceResolver()
        {
            InheritanceResolverManager.RegisterResolver(this);
        }

        // Only resets the inheritance tree to be blank. Does not reset affected DataBlocks!
        public void Reset()
        {
            _root.Clear();
            _init = false;
        }

        public void AddDataBlock(T block, List<PropertyInfo> properties, uint parentID)
        {
            uint ID = block.persistentID;
            InheritanceNode<T> newNode = new(ID, block, properties, parentID);
            _root.AddNode(newNode);

            if (_init)
            {
                ResolveInheritance(ID);
                if (Configuration.DebugChains)
                    _root.DebugPrintAllPaths(GameDataBlockBase<T>.m_fileNameNoExt);
            }
        }

        public void Run()
        {
            foreach (uint ID in _root.GetIDs())
                ResolveInheritance(ID);

            if (Configuration.DebugChains)
                _root.DebugPrintAllPaths(GameDataBlockBase<T>.m_fileNameNoExt);

            // Free up memory since we no longer need it. Will slow down hot reload since cache must repopulate.
            // May still cache stuff after Run(), but it is unlikely there'll be a lot, so it's fine.
            _propertyCache.Clear();
            _init = true;
        }

        private void ResolveInheritance(uint ID)
        {
            LinkedList<InheritanceNode<T>>? inheritanceList = _root.GetInheritanceList(ID);
            if (inheritanceList == null || inheritanceList.Count == 0)
            {
                IDBLogger.Error("Inheritance list for ID " + ID + " is null or empty! Block: " + GameDataBlockBase<T>.m_fileNameNoExt);
                return;
            }

            T baseBlock = GameDataBlockBase<T>.GetBlock(inheritanceList.First!.Value.ParentID);
            if (baseBlock == null)
            {
                IDBLogger.Error("Error on ID " + ID + ": unable to find parent block with ID " + inheritanceList.First!.Value.ParentID + " in " + GameDataBlockBase<T>.m_fileNameNoExt);
                return;
            }

            // We want to keep the original block so we don't lose its info (applied on last step).
            // Instead, build on a new one, then copy onto the original.
            T newBlock = (T)Activator.CreateInstance(typeof(T))!;
            PropertyUtil.CopyProperties(baseBlock, newBlock);
            foreach (InheritanceNode<T> node in inheritanceList)
                foreach (PropertyInfo property in node.Properties)
                    property.SetValue(newBlock, property.GetValue(node.Data));
            PropertyUtil.CopyProperties(newBlock, inheritanceList.Last!.Value.Data);
        }

        public PropertyInfo? CacheProperty(Type blockType, string name)
        {
            if (_propertyCache.TryGetValue(name, out PropertyInfo? value))
                return value;

            PropertyInfo? propertyInfo = name.ToProperty(typeof(T));
            if (propertyInfo == null)
            {
                IDBLogger.Warning("Cannot find property \"" + name + "\" in " + blockType.Name);
                return null;
            }

            _propertyCache[name] = propertyInfo;
            return propertyInfo;
        }
    }

    internal static class InheritanceResolverManager
    {
        private readonly static List<IResolver> _resolvers = new();

        public static void RegisterResolver(IResolver resolver) => _resolvers.Add(resolver);
        public static void ResetResolvers() => _resolvers.ForEach(resolver => resolver.Reset());
        public static void RunResolvers() => _resolvers.ForEach(resolver => resolver.Run());
    }
}
