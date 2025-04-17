using System;
using System.Collections.Generic;
using UnityEngine;

public class ContainerManager
{
    private static ContainerManager _instance;
    public static ContainerManager Instance => _instance ??= new ContainerManager();

    private Dictionary<string, ContainerData> allContainers = new Dictionary<string, ContainerData>();

    private InventoryUI inventoryUI;

    public ContainerData LoadContainer(string containerId)
    {
        ContainerData containerData = SaveSystem.LoadContainerData(containerId) ?? new ContainerData();

        if(containerData.containerId == "")
        {
            containerData.containerId = containerId;
        }

        allContainers.Add(containerId, containerData);

        return containerData;
    }

    public ContainerData? GetContainer(string containerId)
    {
        if (allContainers.ContainsKey(containerId))
        {
            return allContainers[containerId];
        }
        else
        {
            Debug.LogError($"Container with ID {containerId} not found.");
            return null;
        }
    }

    public void MoveItemInContainer(string containerId, int fromInstanceId, int toInstanceId, int xToPosition, int yToPosition)
    {
        inventoryUI = UnityEngine.Object.FindObjectOfType<InventoryUI>();

        List<ItemPosition> updates = InventoryUtils.MoveItem(allContainers[containerId].inventoryGrid, allContainers[containerId].itemStack, allContainers[containerId].rowSize, fromInstanceId, toInstanceId, xToPosition, yToPosition, containerId);

        if (updates == null)
        {
            UnityEngine.Debug.Log("No updates to apply");
            return;
        }

        foreach (ItemPosition itemPosition in updates)
        {
            UnityEngine.Debug.Log($"Moving item: {itemPosition.instanceId} to position: ({itemPosition.x}, {itemPosition.y}) in container: {containerId}");

            if (itemPosition.x != -1)
            {
                inventoryUI.UpdateInventoryUI(itemPosition); // Update the inventory UI
                InventoryUtils.WriteMatrix(allContainers[containerId].inventoryGrid);
            }
        }
    }

    public void SwapItems(string fromContainerId, string toContainerId, int fromInstanceId, int toInstanceId, int xToPosition, int yToPosition)
    {
        ContainerData fromContainer = GetContainer(fromContainerId);
        ContainerData toContainer = GetContainer(toContainerId);

        inventoryUI = UnityEngine.Object.FindObjectOfType<InventoryUI>();

        if (fromContainer != null && toContainer != null)
        {
            List<ItemPosition> updates = ContainerUtils.SwapItemsBetweenContainers(fromContainer, toContainer, fromInstanceId, toInstanceId, xToPosition, yToPosition);

            ItemStack fromItem = fromContainer.itemStack.Find(item => item.instanceId == fromInstanceId);
            ItemStack toItem = toContainer.itemStack.Find(item => item.instanceId == toInstanceId);


            if(toItem.instanceId != -1)
            {
            fromContainer.itemStack.Add(new ItemStack(toInstanceId, toItem.item, toItem.orientation, 1)); // Add the item to the container
            }
            toContainer.itemStack.Add(new ItemStack(fromInstanceId, fromItem.item, fromItem.orientation, 1)); // Add the item to the container

            foreach (ItemPosition update in updates)
            {
                inventoryUI.UpdateInventoryUI(update); // Update the inventory UI for the item position
                UnityEngine.Debug.Log($"Swapping items: {fromItem.item.name} (ID: {fromItem.instanceId}) with {toItem.item.name} (ID: {toItem.instanceId})");
            }

            fromContainer.itemStack.Remove(fromItem); // Remove the item from the container

            if(toItem.instanceId != -1)
            {
                toContainer.itemStack.Remove(toItem); // Remove the item from the container
            }
        }
        else
        {
            UnityEngine.Debug.LogError($"One of the containers is null. From: {fromContainerId}, To: {toContainerId}");
        }
    }
}
