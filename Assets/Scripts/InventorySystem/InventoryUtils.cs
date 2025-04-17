using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public static class InventoryUtils
{
    public static void InitializeMatrix(List<List<int>> inventoryGrid, int rowSize, int inventorySize)
    {
        int row = 0;
        inventoryGrid.Add(new List<int>());

        for (int i = 0; i < inventorySize; i++)
        {
            if (inventoryGrid[row].Count < rowSize)
            {
                inventoryGrid[row].Add(-1);
            }
            else
            {
                UnityEngine.Debug.Log(inventoryGrid[row].Count);
                inventoryGrid.Add(new List<int>());
                row++;
                inventoryGrid[row].Add(-1);
            }
        }
    }


    public static List<ItemPosition> MoveItem(List<List<int>> inventoryGrid, List<ItemStack> itemStack, int rowSize, int fromInstanceId, int toInstanceId, int xToPosition, int yToPosition, string containerId)
    {
        ItemStack moveFrom = itemStack.Find(item => item.instanceId == fromInstanceId);
        ItemStack moveTo = itemStack.Find(item => item.instanceId == toInstanceId);

        var fromPosition = FindItemPositionInGrid(inventoryGrid, fromInstanceId);

        List<ItemPosition> updates = new List<ItemPosition>();

        if (fromPosition != null)
        {
            int fromX = fromPosition.Value.x;
            int fromY = fromPosition.Value.y;

            // If the item is being moved to a slot that isn't empty
            if (toInstanceId != -1)
            {
                var toPosition = FindItemPositionInGrid(inventoryGrid, toInstanceId);

                // Remove both items from their respective spaces in the grid
                updates.Add(RemoveItemFromMatrix(inventoryGrid, moveFrom, containerId));
                updates.Add(RemoveItemFromMatrix(inventoryGrid, moveTo, containerId));

                bool firstSpaceEmpty = false;
                bool secondSpaceEmpty = false;
                if (toPosition != null)
                {
                    int toX = toPosition.Value.x;
                    int toY = toPosition.Value.y;

                    //Check if, after removing both items, the spaces to position them on their new slots are empty
                    if (CheckFreeSpace(inventoryGrid, rowSize, toX, toY, moveFrom.item.xsize, moveFrom.item.ysize, moveFrom.orientation, -1))
                    {
                        firstSpaceEmpty = true;
                    }
                    if (CheckFreeSpace(inventoryGrid, rowSize, fromX, fromY, moveTo.item.xsize, moveTo.item.ysize, moveTo.orientation, -1))
                    {
                        secondSpaceEmpty = true;
                    }

                    //If both spaces are empty, add the items to their new slots
                    if (firstSpaceEmpty && secondSpaceEmpty)
                    {
                        updates.Add(addItemToMatrix(inventoryGrid, new ItemPosition(toX, toY, moveFrom.orientation, moveFrom.item.xsize, moveFrom.item.ysize, fromInstanceId, containerId)));
                        if(CheckFreeSpace(inventoryGrid, rowSize, fromX, fromY, moveTo.item.xsize, moveTo.item.ysize, moveTo.orientation, -1))
                        {
                            updates.Add(addItemToMatrix(inventoryGrid, new ItemPosition(fromX, fromY, moveTo.orientation, moveTo.item.xsize, moveTo.item.ysize, toInstanceId, containerId)));
                        }
                        else
                        {
                            updates.Add(RemoveItemFromMatrix(inventoryGrid, moveFrom, containerId)); // Remove the item from the inventory grid
                            updates.Add(addItemToMatrix(inventoryGrid, new ItemPosition(fromX, fromY, moveFrom.orientation, moveFrom.item.xsize, moveFrom.item.ysize, fromInstanceId, containerId))); // Re-add the item to the new position
                            updates.Add(addItemToMatrix(inventoryGrid, new ItemPosition(toX, toY, moveTo.orientation, moveTo.item.xsize, moveTo.item.ysize, toInstanceId, containerId)));
                        }
                    }
                    else
                    {
                        // If not, add the item back to its original position
                        updates.Add(addItemToMatrix(inventoryGrid, new ItemPosition(fromX, fromY, moveFrom.orientation, moveFrom.item.xsize, moveFrom.item.ysize, fromInstanceId, containerId)));
                        updates.Add(addItemToMatrix(inventoryGrid, new ItemPosition(toX, toY, moveTo.orientation, moveTo.item.xsize, moveTo.item.ysize, toInstanceId, containerId)));
                    }
                }
            }
            else
            {
                // If the slot is empty, check for free space on adjacent slots
                if (CheckFreeSpace(inventoryGrid, rowSize, xToPosition, yToPosition, moveFrom.item.xsize, moveFrom.item.ysize, moveFrom.orientation, fromInstanceId))
                {
                    updates.Add(RemoveItemFromMatrix(inventoryGrid, moveFrom, containerId)); // Remove the item from the inventory grid
                    updates.Add(addItemToMatrix(inventoryGrid, new ItemPosition(xToPosition, yToPosition, moveFrom.orientation, moveFrom.item.xsize, moveFrom.item.ysize, fromInstanceId, containerId))); // Re-add the item to the new position
                }
            }
        }

        return updates;
    }

    public static ItemPosition GetFirstGridEmptySpace(List<List<int>> inventoryGrid, int rowSize, int inventorySize, int xsize, int ysize)
    {
        // Orientation: 0=Horizontal, 1=Vertical

        int y = 0;
        int x = 0;
        int orientation = 0;

        int xSizeNeeded = xsize;
        int ySizeNeeded = ysize;

        // For every inventory slot
        for (int i = 0; i < inventorySize; i++)
        {
            // And meanwhile the pointer is targeting a position that's in the row (its X position is inferior than the max row size)
            if (x < rowSize)
            {
                // If the slot is empty (-1)
                if (inventoryGrid[y][x] == -1)
                {
                    UnityEngine.Debug.Log("Empty space found!");

                    for (orientation = 0; orientation < 2; orientation++)
                    {
                        // Check if the spaces needed for the object on the X axis (xsize) are also empty
                        bool xIsEmpty = true;
                        for (int j = 0; j < xSizeNeeded; j++)
                        {
                            if (x + j >= rowSize || inventoryGrid[y][x + j] != -1)
                            {
                                xIsEmpty = false;
                                break;
                            }
                        }

                        // And if the spaces needed for the object on the Y axis (ysize) are also empty
                        bool yIsEmpty = true;
                        for (int j = 0; j < ySizeNeeded; j++)
                        {
                            if (y + j >= inventoryGrid.Count || inventoryGrid[y + j][x] != -1)
                            {
                                yIsEmpty = false;
                                break;
                            }
                        }

                        // If all the slots are empty, return the position of the first empty slot detected and the orientation of the object
                        if (xIsEmpty && yIsEmpty)
                        {
                            return new ItemPosition(x, y, orientation);
                        }
                        else
                        {
                            // If the space wasn't empty horizontally, change orientation of the object and try again
                            xSizeNeeded = ysize;
                            ySizeNeeded = xsize;
                        }
                    }
                    xSizeNeeded = xsize;
                    ySizeNeeded = ysize;
                    orientation = 0;
                }
                x++;
            }
            else
            {
                // If the pointer reached the end of the row without returning anything, continue with the next possible row
                y++;
                x = 0;
            }
        }
        // If the pointer went through every inventory slot without finding a space that passes all the requirements, return null
        return new ItemPosition(-1, -1, -1);
    }
    
    

    public static bool CheckFreeSpace(List<List<int>> inventoryGrid, int rowSize, int x, int y, int xsize, int ysize, int orientation, int instanceId)
    {
        // Orientation: 0=Horizontal, 1=Vertical
        int xSizeToOccupy = xsize;
        int ySizeToOccupy = ysize;
        if (orientation == 1)
        {
            xSizeToOccupy = ysize;
            ySizeToOccupy = xsize;
        }
        for (int i = 0; i < xSizeToOccupy; i++)
        {
            for (int j = 0; j < ySizeToOccupy; j++)
            {
                if (x + i >= rowSize || y + j >= inventoryGrid.Count)
                {
                    UnityEngine.Debug.Log($"Out of bounds at: {x + i}, {y + j}");
                    return false;
                }
                if (inventoryGrid[y + j][x + i] != -1 && inventoryGrid[y + j][x + i] != instanceId)
                {
                    UnityEngine.Debug.Log($"Space occupied at: {x + i}, {y + j}");
                    return false;
                }
            }
        }
        return true;
    }

    public static ItemPosition addItemToMatrix(List<List<int>> inventoryGrid, ItemPosition itemPosition)
    {
        int xSizeToOccupy = itemPosition.xsize;
        int ySizeToOccupy = itemPosition.ysize;

        if (itemPosition.orientation == 1)
        {
            xSizeToOccupy = itemPosition.ysize;
            ySizeToOccupy = itemPosition.xsize;
        }

        for (int i = 0; i < xSizeToOccupy; i++)
        {
            for (int j = 0; j < ySizeToOccupy; j++)
            {
                inventoryGrid[itemPosition.y + j][itemPosition.x + i] = itemPosition.instanceId;
            }
        }
        return itemPosition;
    }

    public static ItemPosition RemoveItemFromMatrix(List<List<int>> inventoryGrid, ItemStack itemStacked, string containerId)
    {
        // Remove the item from the inventory grid
        var positionInGrid = FindItemPositionInGrid(inventoryGrid, itemStacked.instanceId);
        if (positionInGrid != null)
        {
            int x = positionInGrid.Value.x;
            int y = positionInGrid.Value.y;
            int xSize = itemStacked.item.xsize;
            int ySize = itemStacked.item.ysize;

            if (itemStacked.orientation == 1)
            {
                xSize = itemStacked.item.ysize;
                ySize = itemStacked.item.xsize;
            }

            for (int i = 0; i < xSize; i++)
            {
                for (int j = 0; j < ySize; j++)
                {
                    inventoryGrid[y + j][x + i] = -1;
                }
            }
            return new ItemPosition(x, y, itemStacked.orientation, xSize, ySize, -1, containerId);
        }
        return new ItemPosition(-1); 
    }

    public static (int x, int y)? FindItemPositionInGrid(List<List<int>> inventoryGrid, int instanceId)
    {
        for (int y = 0; y < inventoryGrid.Count; y++)
        {
            for (int x = 0; x < inventoryGrid[y].Count; x++)
            {
                if (inventoryGrid[y][x] == instanceId)
                {
                    return (x, y);
                }
            }
        }
        return null;
    }


    public static void WriteMatrix(List<List<int>> inventoryGrid)
    {
        string[] strings = new string[inventoryGrid.Count];
        // Print the inventory grid
        for (int i = 0; i < inventoryGrid.Count; i++)
        {
            strings[i] = "{";
            for (int j = 0; j < inventoryGrid[i].Count; j++)
            {
                strings[i] += inventoryGrid[i][j].ToString() + ", ";
            }
            strings[i] += "}";
        }

        for (int i = 0; i < strings.Length; i++)
        {
            UnityEngine.Debug.Log(strings[i]);
        }
    }
}
