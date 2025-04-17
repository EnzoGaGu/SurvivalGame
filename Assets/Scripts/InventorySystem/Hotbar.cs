using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static System.Net.Mime.MediaTypeNames;
using UnityEngine.InputSystem;

public class Hotbar : MonoBehaviour
{
    public List<ItemStack> hotbarItems = new List<ItemStack>(); // List of items in the hotbar
    public List<HotbarSlot> hotbarSlots = new List<HotbarSlot>(); // List of hotbar slots
    public int selectedPosition = 0; // The selected position in the hotbar
    public ItemDatabase itemDatabase; // Reference to the item database
    private InventoryUI inventoryUI; // Reference to the InventoryUI
    private PlayerStats playerStats; // Reference to the PlayerStats
    private InputAction selectPositionAction; // Select hotbar position action
    public HotbarSlot selectedHotbarSlot; // The selected hotbar slot
    private HoldingItem holdingItem; // Reference to the HoldingItem
    private InputHandler inputHandler; 


    void Awake()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
        if (playerStats == null)
        {
            UnityEngine.Debug.LogError("PlayerStats not found in the scene.");
        }
        else
        {
            playerStats.OnHotbarSizeChange += UpdateHotbarLength;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventoryUI = FindFirstObjectByType<InventoryUI>();
        holdingItem = FindFirstObjectByType<HoldingItem>();
        inputHandler = FindFirstObjectByType<InputHandler>();
        if (InventoryManager.Instance == null)
        {
            UnityEngine.Debug.LogError("InventoryManager not found in the scene.");
        }

        for (int i = 0; i < playerStats.hotbarSize; i++)
        {
            hotbarItems.Add(new ItemStack(-1, itemDatabase.getEmptyItem(), 0, 0));
        }

        inventoryUI.StartHotbarUI();
        SelectHotbarSlot(0);

        selectPositionAction = inputHandler.LoadInputAction("UI/SelectHotbarPosition");
        if (selectPositionAction != null)
        {
            selectPositionAction.performed += OnSelectPosition;
            selectPositionAction.Enable();
        }
    }

    private void UpdateHotbarLength(int newSize)
    {
        UnityEngine.Debug.Log("Updating hotbar length to " + newSize);
        List<ItemStack> newList = new List<ItemStack>(newSize);

        
        if (newSize > hotbarItems.Count)
        {
            for (int i = 0; i < hotbarItems.Count; i++)
            {
                newList.Add(hotbarItems[i]);
            }
            for (int i = hotbarItems.Count; i < newSize; i++)
            {
                newList.Add(new ItemStack(0, itemDatabase.getEmptyItem(), 0, 0));
            }
        }
        else
        {
            for (int i = 0; i < newSize; i++)
            {
                newList.Add(hotbarItems[i]);
            }
        }

        hotbarItems = newList;
        inventoryUI.StartHotbarUI();
    }

    public void AddToHotbar(int instanceId, int position)
    {
        //UnityEngine.Debug.Log("Start of AddToHotbar");
        Hotbar hotbar = FindFirstObjectByType<Hotbar>();    // Get the hotbar

        
        if (instanceId == -1)
        {
            hotbarItems[position] = new ItemStack(-1, itemDatabase.getEmptyItem(), 0, 0);
            inventoryUI.UpdateHotbarUI(instanceId, position);
            SelectHotbarSlot(position);
            return;
        }
        

        ItemStack itemStacked = InventoryManager.Instance.playerContainer.itemStack.Find(i => i.instanceId == instanceId);    //Find the item in the inventory
        //UnityEngine.Debug.Log("ItemStacked: " + itemStacked.instanceId);

        if (itemStacked != null)
        {
            if (hotbarItems.Find(i => i.instanceId == instanceId) != null)
            {
                UnityEngine.Debug.Log("Item already in hotbar");
                return;
            }
            else
            {
                hotbarItems[position] = itemStacked;
                hotbar.UpdateHotbar(playerStats.hotbarSize - position);
            }
        }
        inventoryUI.UpdateHotbarUI(instanceId, position);
        SelectHotbarSlot(position);
        //UnityEngine.Debug.Log($"Added to hotbar: {itemStacked.item.getItemName()}");
    }

   
    public void UpdateAmmount(int instanceId, int ammount)
    {
       //UnityEngine.Debug.Log("Start of updateAmmount");
        ItemStack itemStacked = hotbarItems.Find(i => i.instanceId == instanceId);    //Find the item in the inventory
        if (itemStacked != null)
        {
            itemStacked.ammount = ammount;
            inventoryUI.UpdateHotbarUI(instanceId, hotbarItems.FindIndex(i => i.instanceId == instanceId));
        }
    }

    private void OnSelectPosition(InputAction.CallbackContext context)
    {
        UnityEngine.Debug.Log("Start of OnSelectPosition");
        // Get the control that triggered the action
        var control = context.control;

        var selectedPosition = 0;

        if (control != null && control.device is Keyboard)   //Si existe el control, es un teclado, y el mouse está sobre el inventorySlot
        {
            // Check which key was pressed and set the selectedPosition accordingly
            if (Keyboard.current.digit0Key.wasPressedThisFrame) selectedPosition = 10;
            else if (Keyboard.current.digit1Key.wasPressedThisFrame) selectedPosition = 1;
            else if (Keyboard.current.digit2Key.wasPressedThisFrame) selectedPosition = 2;
            else if (Keyboard.current.digit3Key.wasPressedThisFrame) selectedPosition = 3;
            else if (Keyboard.current.digit4Key.wasPressedThisFrame) selectedPosition = 4;
            else if (Keyboard.current.digit5Key.wasPressedThisFrame) selectedPosition = 5;
            else if (Keyboard.current.digit6Key.wasPressedThisFrame) selectedPosition = 6;
            else if (Keyboard.current.digit7Key.wasPressedThisFrame) selectedPosition = 7;
            else if (Keyboard.current.digit8Key.wasPressedThisFrame) selectedPosition = 8;
            else if (Keyboard.current.digit9Key.wasPressedThisFrame) selectedPosition = 9;

            UnityEngine.Debug.Log($"Selected hotbar position: {selectedPosition}");
            
            SelectHotbarSlot(selectedPosition - 1);
            //inventoryUI.UpdateHotbarUI();
        }
    }

    public void SelectHotbarSlot(int position)
    {
        this.selectedPosition = position;
        if (this.selectedHotbarSlot != null)
        {
            this.selectedHotbarSlot.Deselect();
        }

        this.selectedHotbarSlot = hotbarSlots[position];


        hotbarSlots[position].Select();
    }

    public void RemoveFromHotbar(int position)
    {
        if(position == selectedPosition)
        {
            holdingItem.UnequipItem();
        }

        AddToHotbar(-1, position);
    }



    public void UpdateHotbar(int selectedPosition)
    {
        //List<HotbarSlot> hotbarSlots = new List<HotbarSlot>(GetComponentsInChildren<HotbarSlot>());
        //hotbarSlots[selectedPosition].Select();
        //UnityEngine.Debug.Log("Selected position: " + selectedPosition);
    }
}
