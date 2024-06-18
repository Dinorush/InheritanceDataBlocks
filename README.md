# Implementing into your own mod

InheritanceDataBlocks does not have an API to support inheritance for custom objects, but it *does* expose the data structure used to manage its internal behavior so that other mods may do so as well.

## Adding Inheritance

### 1. Create Root

You will need to create an `InheritanceRoot<T>` object from the `InheritanceDataBlocks.Inheritance` namespace. Nearly all behavior for managing inheritance is handled by the root node.

### 2. Add Nodes

You can add nodes to the root with `root.AddNode(InheritanceNode<T> node)`. Nodes can be instantiated with `InheritanceNode<T>(uint ID, T data, string[] names, uint parentID)`, where each field is:

- `ID`: The ID of the data/node
- `data`: The data the node references
- `names`: The list of property names in `data` that this node will apply to data that have it in their inheritance chain
- `parentID`: The ID of the parent data/node

Obtaining (and later applying) the `names` array is perhaps the most complex portion of the code that you will need to handle.

Calling `root.AddNode` again with an existing ID will remove the previous node and then add the new node.

### 3. Applying the Inheritance Chain

You should only move to this step once all nodes have been added (or you will need to repeat it).
By calling `root.GetInheritanceList(uint ID)`, you will receive a linked list of each `InheritanceNode` that `ID` is a child of with the basest class first. You can do this for all IDs by using it in tandem with `root.GetIDs()`.

By accessing the `ParentID` of the very first node, you can get the ID of the original `T` data to pull from. You can use the ID that was used to obtain the list to find the child data or get the child data from the last node in the list.

With the setup complete, you can follow the inheritance chain to apply parent values to the child data. However, it gets more involved. You will need to:

1. Create a copy of the original data
2. Iterate over the list and use reflection (or some other method) to apply each node's changes to the copy
3. Assign the copy's values to the child data

You may reference the source code for `InheritanceResolver` which has [CopyProperties](https://github.com/Dinorush/InheritanceDataBlocks/blob/3ce2c1b2a2a783d93bc6fd3ad6bd8329948697d3/InheritanceDataBlocks/Inheritance/InheritanceResolver.cs#L109)
(thank you Flow) to handle copying and [ApplyNodeToData](https://github.com/Dinorush/InheritanceDataBlocks/blob/3ce2c1b2a2a783d93bc6fd3ad6bd8329948697d3/InheritanceDataBlocks/Inheritance/InheritanceResolver.cs#L91)
to apply each node's changes to the copy.

### 4. Resetting

If you need to fully reset the structure (e.g. reloading the file), you can call `root.Clear()`. If you only want to clear a subset, you can instead call `root.RemoveNode(uint ID)` for each ID that it may have.

### Debugging

You can use `root.DebugPrintAllPaths()` to print all IDs and their inheritance chains to find broken inheritance chains and the like.
