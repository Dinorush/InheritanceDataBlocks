using InheritanceDataBlocks.Utils;
using GameData;
using Il2CppSystem.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace InheritanceDataBlocks.InheritanceResolver
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

        private bool _init = false;

        public InheritanceResolver()
        {
            InheritanceResolverManager.RegisterResolver(this);
        }

        // Only resets the inheritance tree to be blank. Does not reset affected DataBlocks!
        public void Reset()
        {
            _root = new();
            _init = false;
        }

        public void AddDataBlock(T block, string[] names, uint parentID)
        {
            uint ID = block.persistentID;
            InheritanceNode<T> newNode = new(ID, block, names, parentID);
            _root.AddNode(newNode);

            if (_init)
            {
                ResolveInheritance(ID, block);
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

            _init = true;
        }

        private void ResolveInheritance(uint ID, T? block = null)
        {
            LinkedList<InheritanceNode<T>>? inheritanceList = _root.GetInheritanceList(ID);
            if (inheritanceList == null || inheritanceList.Count == 0)
            {
                IDBLogger.Error("Inheritance list for ID " + ID + " is null or empty! Block: " + GameDataBlockBase<T>.m_fileNameNoExt);
                return;
            }

            block ??= GameDataBlockBase<T>.GetBlock(ID);
            T baseBlock = GameDataBlockBase<T>.GetBlock(inheritanceList.First!.Value.ParentID);
            if (baseBlock == null)
            {
                IDBLogger.Error("Error on ID " + ID + ": unable to find parent block with ID " + inheritanceList.First!.Value.ParentID + " in " + block.GetType().Name);
                return;
            }

            // We want to keep the original block so we don't lose its info (applied on last step).
            // Instead, build on a new one, then copy onto the original.
            T newBlock = (T)Activator.CreateInstance(typeof(T))!;
            CopyProperties(baseBlock, newBlock);
            foreach (InheritanceNode<T> node in inheritanceList)
                ApplyNodeToData(newBlock, node);
            CopyProperties(newBlock, block);
        }

        private const BindingFlags FieldFlags = BindingFlags.Instance | BindingFlags.Public;

        private static void ApplyNodeToData(T newBlock, InheritanceNode<T> node)
        {
            Type blockType = newBlock.GetType();

            foreach (string name in node.Names)
            {
                PropertyInfo? propertyInfo = blockType.GetProperty(name, FieldFlags);
                if (propertyInfo == null || !propertyInfo.CanWrite)
                {
                    IDBLogger.Warning("ID " + node.ID + " has property \"" + name + "\" which does not exist in " + blockType.Name);
                    continue;
                }

                propertyInfo.SetValue(newBlock, propertyInfo.GetValue(node.Data));
            }
        }

        // Ripped from PartialData
        private static object CopyProperties(object source, object target)
        {
            foreach (var sourceProp in source.GetType().GetProperties())
            {
                var sourceType = sourceProp.PropertyType;

                var targetProp = target.GetType().GetProperties().FirstOrDefault(x => x.Name == sourceProp.Name && x.PropertyType == sourceProp.PropertyType && x.CanWrite);
                if (targetProp != null)
                {
                    if (sourceProp.Name.Contains("_k__BackingField"))
                    {
                        continue;
                    }

                    if (sourceType == typeof(IntPtr))
                    {
                        IDBLogger.Error("Pointer has detected on CopyProperties!!!!");
                        continue;
                    }

                    targetProp.SetValue(target, sourceProp.GetValue(source));
                }
            }
            return target;
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
