using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public ContainerData playerContainer;
    public List<ItemStack> itemStack; // List of items in the inventory
    public List<List<int>> inventoryGrid; // 2D list for the inventory grid

    private Transform playerCameraTransform;
    private int ammountOfSpacesOccupied = 0;
    private PlayerStats playerStats; // Reference to the player stats
    private InventoryUI inventoryUI; // Reference to the inventory UI
    private Hotbar hotbar; // Reference to the hotbar
    public ItemDatabase itemDatabase; // Reference to the item database
    public int rowSize;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);


        PlayerCameraRoot cameraRoot = GetComponentInChildren<PlayerCameraRoot>();   // If there's a fp camera (a player is instantiated)
        playerStats = FindFirstObjectByType<PlayerStats>();
        inventoryUI = FindFirstObjectByType<InventoryUI>();
        hotbar = FindFirstObjectByType<Hotbar>();

        playerContainer = ContainerManager.Instance.LoadContainer("Player_" + playerStats.playerName); // Load the container data for the player

        if(playerContainer.containerName == "")
        {
            playerContainer.containerName = playerStats.playerName + "'s Inventory"; // Set the container name
            playerContainer.containerSize = playerStats.inventorySize; // Set the container size
            
            rowSize = playerStats.inventoryRowSize; // Set the row size
            
            playerContainer.rowSize = rowSize; // Set the container row size
        }

        itemStack = playerContainer.itemStack; // Get the item stack from the container data
        inventoryGrid = playerContainer.inventoryGrid;

        if (cameraRoot != null)
        {
            playerCameraTransform = cameraRoot.transform;   // Get the camera transform
        }
        else
        {
            UnityEngine.Debug.LogError("PlayerCameraRoot not found in the playerCapsule.");
        }

        InventoryUtils.InitializeMatrix(inventoryGrid, rowSize, playerStats.inventorySize); // Initialize the inventory grid

        itemStack.Add(new ItemStack(-1, itemDatabase.getEmptyItem(), 0, 0)); // Add an empty item to the inventory
        //UnityEngine.Debug.Log(itemStack.Count);
    }

    public void AddItem(Item item, int? instanceId)
    {
        if (itemStack.Count != 0)
        {
            List<ItemStack> existingItem = itemStack.FindAll(i => i.getItemId() == item.getItemId()); // Find the item in the inventory

            foreach (ItemStack i in existingItem)
            {
                if (i != null && i.ammount < i.item.ammountPerSlot) // If the item is already in the inventory and the ammount is less than the max
                {
                    i.ammount++; // Add one to the ammount
                    UnityEngine.Debug.Log($"Added one of: {item.getItemName()} to slot number: {i.instanceId}");
                    inventoryUI.UpdateAmmountInInventory(i.instanceId, i.ammount); // Update the inventory UI
                    return;
                }
            }
        }


        ItemPosition availablePosition = InventoryUtils.GetFirstGridEmptySpace(inventoryGrid, rowSize, playerStats.inventorySize, item.xsize, item.ysize);

        if (availablePosition.x != -1 && availablePosition.y != -1 && availablePosition.orientation != -1)
        {
            int newInstanceId = instanceId ?? ItemUtils.GetNextFreeInstanceId();

            //If there's no space in the inventory occupied by this item type and with available space (less than the max ammount per slot)
            ammountOfSpacesOccupied = ammountOfSpacesOccupied + item.xsize + item.ysize; // Add to the ammount of spaces occupied
            itemStack.Add(new ItemStack(newInstanceId, item, availablePosition.orientation, 1)); // Add the item to the inventory list

            ItemPosition itemPosition = InventoryUtils.addItemToMatrix(inventoryGrid, new ItemPosition(availablePosition.x, availablePosition.y, availablePosition.orientation, item.xsize, item.ysize, newInstanceId, playerContainer.containerId));

            inventoryUI.UpdateInventoryUI(itemPosition); // Update the inventory UI
        }
        else
        {
            UnityEngine.Debug.Log("There's no space in the inventory for this item");
        }

        InventoryUtils.WriteMatrix(inventoryGrid);
    }

    public void DropItem(int instanceId)
    {
        ItemStack itemStacked = itemStack.Find(item => item.instanceId == instanceId);

        GameObject instantiatedItem = Instantiate(itemStacked.item.prefab, transform.position + (Vector3.up * 1 + playerCameraTransform.forward * 1), itemStacked.item.prefab.transform.rotation);

        if (itemStacked.ammount > 1)
        {
            itemStacked.ammount--;
            hotbar.UpdateAmmount(itemStacked.instanceId, itemStacked.ammount);
            
            inventoryUI.UpdateAmmountInInventory(instanceId, itemStacked.ammount); // Update the inventory UI
        }
        else
        {
            RemoveFromHotbarIfDropped(itemStacked.instanceId); // Remove the item from the hotbar if it's dropped


            ItemPosition itemPosition = InventoryUtils.RemoveItemFromMatrix(inventoryGrid, itemStacked, playerContainer.containerId);
            inventoryUI.UpdateInventoryUI(itemPosition); // Update the inventory UI
            itemStack.Remove(itemStacked);
        }
        InventoryUtils.WriteMatrix(inventoryGrid);


    }

    public void MoveItem(int fromInstanceId, int toInstanceId, int xToPosition, int yToPosition)
    {
        List<ItemPosition> updates = InventoryUtils.MoveItem(inventoryGrid, itemStack, rowSize, fromInstanceId, toInstanceId, xToPosition, yToPosition, playerContainer.containerId);

        if(updates == null)
        {
            UnityEngine.Debug.Log("No updates to apply");
            return;
        }

        foreach (ItemPosition itemPosition in updates)
        {
            if (itemPosition.x != -1)
            {
                inventoryUI.UpdateInventoryUI(itemPosition); // Update the inventory UI
                InventoryUtils.WriteMatrix(inventoryGrid);
            }
        }
    }

    public void RemoveFromHotbarIfDropped(int instanceId)
    {
        ItemStack itemStacked = itemStack.Find(item => item.instanceId == instanceId);
        if (itemStacked != null)
        {
            if (hotbar.hotbarItems.Contains(itemStacked))
            {
                int position = hotbar.hotbarItems.IndexOf(itemStacked);
                int hotbarSize = playerStats.hotbarSize;
                if (position >= 0 && position < hotbarSize)
                {
                    hotbar.RemoveFromHotbar(position);
                }
                else
                {
                    UnityEngine.Debug.LogError($"Invalid position: {position}. It must be between 0 and {hotbarSize - 1}.");
                }
            }
        }
    }
}