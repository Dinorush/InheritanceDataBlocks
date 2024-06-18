# Implementing into your own mod

InheritanceDataBlocks does not have an API to support inheritance for custom objects, but it *does* expose the data structure used to manage its internal behavior so that other mods may do so as well.

## Adding Inheritance

### 1. Create Root

You will need to create an `InheritanceRoot<T>` object from the `InheritanceDataBlocks.Inheritance` namespace. Nearly all behavior for managing inheritance is handled by the root node.

### 2. Add Nodes

You can add nodes to the root with `root.AddNode(InheritanceNode<T> node)`. Nodes can be instantiated with `InheritanceNode<T>(uint ID, T data, List<PropertyInfo> names, uint parentID)`, where each field is:

- `ID`: The ID of the data/node
- `data`: The data the node references
- `properties`: The list of properties in `data` that this node will apply to data that have it in their inheritance chain
- `parentID`: The ID of the parent data/node

To facilitate obtaining the `PropertyInfo`s, `InheritanceDataBlocks.Utils` exposes the string extension method `string.ToProperty(Type type)`.

Calling `root.AddNode` again with an existing ID will remove the previous node and then add the new node.

### 3. Applying the Inheritance Chain

You should only move to this step once all nodes have been added (or you will need to repeat it).
By calling `root.GetInheritanceList(uint ID)`, you will receive a linked list of each `InheritanceNode` that `ID` is a child of with the basest class first. You can do this for all IDs by using it in tandem with `root.GetIDs()`.

By accessing the `ParentID` of the very first node, you can get the ID of the original `T` data to pull from. You can use the ID that was used to obtain the list to find the child data or get the child data from the last node in the list.

With the setup complete, you can follow the inheritance chain to apply parent values to the child data. You will need to:

1. Create a copy of the original data
2. Iterate over the list and use reflection to apply each node's changes to the copy
3. Assign the copy's values to the child data

`InheritanceDataBlocks.Utils` exposes the function `PropertyUtil.CopyProperties<T>(T source, T target)` to help create copies. You may reference the `InheritanceResolver` [source code](https://github.com/Dinorush/InheritanceDataBlocks/blob/976c35128a5e5d744510d38c0f8e79671b88b654/InheritanceDataBlocks/Inheritance/InheritanceResolver.cs#L83) to apply these steps.

### Resetting

If you need to fully reset the structure (e.g. reloading the file), you can call `root.Clear()`. If you only want to clear a subset, you can instead call `root.RemoveNode(uint ID)` for each target ID.

### Debugging

You can use `root.DebugPrintAllPaths()` to print all IDs and their inheritance chains to find broken inheritance chains and the like.
