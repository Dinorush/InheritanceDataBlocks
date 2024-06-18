using GameData;
using InheritanceDataBlocks.Inheritance;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace InheritanceDataBlocks.API
{
    public static class InheritanceAPI<T> where T : GameDataBlockBase<T>
    {
        public static PropertyInfo? CacheProperty(Type blockType, string name) => InheritanceResolver<T>.Instance.CacheProperty(blockType, name);
        public static void ApplyAllInheritance() => InheritanceResolver<T>.Instance.Run();

        public static InheritanceRoot<T> GetRoot() => InheritanceResolver<T>.GetRoot();
        public static void AddDataBlock(T data, List<PropertyInfo> properties, uint parentID) => AddNode(data.persistentID, data, properties, parentID);
        public static bool AddNode(uint ID, T data, List<PropertyInfo> properties, uint parentID) => InheritanceResolver<T>.GetRoot().AddNode(ID, data, properties, parentID);
        public static bool AddNode(InheritanceNode<T> node) => InheritanceResolver<T>.GetRoot().AddNode(node);
        public static LinkedList<InheritanceNode<T>>? GetInheritanceList(uint ID) => InheritanceResolver<T>.GetRoot().GetInheritanceList(ID);
    }
}
