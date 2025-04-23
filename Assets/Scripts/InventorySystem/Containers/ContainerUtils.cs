using System.Collections.Generic;
using UnityEngine;

public static class ContainerUtils
{
    public static List<ItemPosition> SwapItemsBetweenContainers(ContainerData fromContainer, ContainerData toContainer, int fromInstanceId, int toInstanceId, int xToPosition, int yToPosition)
    {
        List<ItemPosition> updates = new List<ItemPosition>();

        ItemStack fromItem = fromContainer.itemStack.Find(item => item.instanceId == fromInstanceId);
        var fromPosition = InventoryUtils.FindItemPositionInGrid(fromContainer.inventoryGrid, fromInstanceId);

        if (fromPosition != null)
        {
            int fromX = fromPosition.Value.x;
            int fromY = fromPosition.Value.y;

            // If the item is being moved to a slot that isn't empty
            if (toInstanceId != -1)
            {
                ItemStack toItem = toContainer.itemStack.Find(item => item.instanceId == toInstanceId);
                var toPosition = InventoryUtils.FindItemPositionInGrid(toContainer.inventoryGrid, toInstanceId);

                // Remove both items from their respective spaces in the grid
                updates.Add(InventoryUtils.RemoveItemFromMatrix(fromContainer.inventoryGrid, fromItem, fromContainer.containerId));
                updates.Add(InventoryUtils.RemoveItemFromMatrix(toContainer.inventoryGrid, toItem, toContainer.containerId));

                bool firstSpaceEmpty = false;
                bool secondSpaceEmpty = false;
                if (toPosition != null)
                {
                    int toX = toPosition.Value.x;
                    int toY = toPosition.Value.y;

                    //Check if, after removing both items, the spaces to position them on their new slots are empty
                    if (InventoryUtils.CheckFreeSpace(fromContainer.inventoryGrid, fromContainer.rowSize, fromX, fromY, toItem.item.xsize, toItem.item.ysize, toItem.orientation, -1))
                    {
                        firstSpaceEmpty = true;
                    }
                    if (InventoryUtils.CheckFreeSpace(toContainer.inventoryGrid, toContainer.rowSize, toX, toY, fromItem.item.xsize, fromItem.item.ysize, fromItem.orientation, -1))
                    {
                        secondSpaceEmpty = true;
                    }

                    UnityEngine.Debug.Log($"From container: ");
                    InventoryUtils.WriteMatrix(fromContainer.inventoryGrid);
                    UnityEngine.Debug.Log($"To container: ");
                    InventoryUtils.WriteMatrix(toContainer.inventoryGrid);

                    //If both spaces are empty, add the items to their new slots
                    if (firstSpaceEmpty && secondSpaceEmpty)
                    {
                        updates.Add(InventoryUtils.addItemToMatrix(toContainer.inventoryGrid, new ItemPosition(toX, toY, fromItem.orientation, fromItem.item.xsize, fromItem.item.ysize, fromInstanceId, toContainer.containerId)));
                        if (InventoryUtils.CheckFreeSpace(fromContainer.inventoryGrid, fromContainer.rowSize, fromX, fromY, toItem.item.xsize, toItem.item.ysize, toItem.orientation, -1))
                        {
                            updates.Add(InventoryUtils.addItemToMatrix(fromContainer.inventoryGrid, new ItemPosition(fromX, fromY, toItem.orientation, toItem.item.xsize, toItem.item.ysize, toInstanceId, fromContainer.containerId)));
                            UnityEngine.Debug.Log("Both spaces are empty, items swapped successfully");
                        }
                        else
                        {
                            updates.Add(InventoryUtils.RemoveItemFromMatrix(toContainer.inventoryGrid, fromItem, toContainer.containerId)); // Remove the item from the inventory grid
                            updates.Add(InventoryUtils.addItemToMatrix(fromContainer.inventoryGrid, new ItemPosition(fromX, fromY, fromItem.orientation, fromItem.item.xsize, fromItem.item.ysize, fromInstanceId, fromContainer.containerId))); // Re-add the item to the new position
                            updates.Add(InventoryUtils.addItemToMatrix(toContainer.inventoryGrid, new ItemPosition(toX, toY, toItem.orientation, toItem.item.xsize, toItem.item.ysize, toInstanceId, toContainer.containerId)));
                        }
                    }
                    else
                    {
                        // If not, add the item back to its original position
                        updates.Add(InventoryUtils.addItemToMatrix(fromContainer.inventoryGrid, new ItemPosition(fromX, fromY, fromItem.orientation, fromItem.item.xsize, fromItem.item.ysize, fromInstanceId, fromContainer.containerId))); // Re-add the item to the new position
                        updates.Add(InventoryUtils.addItemToMatrix(toContainer.inventoryGrid, new ItemPosition(toX, toY, toItem.orientation, toItem.item.xsize, toItem.item.ysize, toInstanceId, toContainer.containerId)));
                    }
                }
            }
            else
            {
                // If the slot is empty, check for free space on adjacent slots
                if (InventoryUtils.CheckFreeSpace(toContainer.inventoryGrid, toContainer.rowSize, xToPosition, yToPosition, fromItem.item.xsize, fromItem.item.ysize, fromItem.orientation, fromInstanceId))
                {
                    updates.Add(InventoryUtils.RemoveItemFromMatrix(fromContainer.inventoryGrid, fromItem, fromContainer.containerId)); // Remove the item from the inventory grid
                    updates.Add(InventoryUtils.addItemToMatrix(toContainer.inventoryGrid, new ItemPosition(xToPosition, yToPosition, fromItem.orientation, fromItem.item.xsize, fromItem.item.ysize, fromInstanceId, toContainer.containerId))); // Re-add the item to the new position
                }
            }
        }

        return updates;
    }
}
