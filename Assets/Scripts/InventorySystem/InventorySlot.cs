using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using static System.Net.Mime.MediaTypeNames;
using System.Security.Cryptography;
using TMPro;

public class InventorySlot : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler
{
    public UnityEngine.UI.Image icon;
    public TMP_Text itemNameText; // This is the name of the item
    public TMP_Text itemAmmount; // This is the ammount of items in the slot
    public int itemId;  
    public int instanceId; 
    public bool isMouseOver = false;
    public bool onPlayer = false; 
    public int xPosition; // The x position of the item in the inventory grid
    public int yPosition; // The y position of the item in the inventory grid
    [SerializeField] private GameObject draggablePrefab;
    private InputAction selectPositionAction; // Select hotbar position action
    private InputAction secondaryAction; // Secondary action (for rotating items, or adding them to a new stack)
    private InputHandler inputHandler;
    private Hotbar hotbar;
    private InventoryToggle inventoryToggle;

    private ContainerData containerData; // Reference to the container data

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    private Vector2 currentPosition;
    private Transform dragLayer;
    private GameObject draggable; 

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        dragLayer = FindFirstObjectByType<Canvas>().transform.Find("Drag_Layer"); // The layer of the canvas where the dragged item will be shown (it's on top of everything else) 

        if(transform.parent.name == "Inventory_Items_Panel")
        {
            onPlayer = true; 
        }
    }

    public void SetItem(int instanceId, string containerId)
    {
        hotbar = FindFirstObjectByType<Hotbar>();
        inventoryToggle = FindFirstObjectByType<InventoryToggle>();
        inputHandler = FindFirstObjectByType<InputHandler>();

        containerData = ContainerManager.Instance.GetContainer(containerId); // Get the container data from the container manager

        ItemStack itemStack = ContainerManager.Instance.GetContainer(containerId).itemStack.Find(i => i.instanceId == instanceId);

        if (itemStack != null)
        {
            this.instanceId = instanceId;
            icon.sprite = itemStack.item.getItemIcon();
            itemNameText.text = itemStack.item.getItemName();
            itemId = itemStack.item.getItemId();
            itemAmmount.text = itemStack.ammount.ToString();    // Get the ammount of items in the slot from the itemStack
            itemAmmountIsVisible();   // Show or hide the ammount of items
        }
        else
        {
            UnityEngine.Debug.LogError("The item is null");
        }

        selectPositionAction = inputHandler.LoadInputAction("UI/SelectHotbarPosition");
        if(selectPositionAction != null)
        {
            selectPositionAction.performed += OnSelectPosition;
            selectPositionAction.Enable();
        }

        secondaryAction = inputHandler.LoadInputAction("Player/SecondaryAction");
        if(secondaryAction != null)
        {
            secondaryAction.performed += OnSecondaryAction;
            secondaryAction.Enable();
        }
    }

    public void onClick()
    {
        if (instanceId != -1 && containerData.containerId.StartsWith("Player_"))
        {
            InventoryManager.Instance.DropItem(instanceId);
        }
    }

    public void onPointerEnter()
    {
        isMouseOver = true; 
    }

    public void onPointerExit()
    {
        isMouseOver = false;
    }

    private void OnSelectPosition(InputAction.CallbackContext context)
    {
        //UnityEngine.Debug.Log("Start of OnSelectPosition");
        // Get the control that triggered the action
        var control = context.control;

        var selectedPosition = 0;

        if (control != null && control.device is Keyboard && isMouseOver && itemId != null && inventoryToggle.isInventoryOpen)   // If control exists, it's a keyboard and the mouse is over the inventorySlot
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
            hotbar.AddToHotbar(instanceId, selectedPosition - 1);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        currentPosition = rectTransform.anchoredPosition; // Save the current position of the slot
        canvasGroup.blocksRaycasts = false;

        draggable = Instantiate(draggablePrefab, dragLayer);

        InventorySlotDraggable draggableScript = draggable.GetComponent<InventorySlotDraggable>();

        draggableScript.SetItem(icon.sprite);

        draggable.transform.position = Input.mousePosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        draggable.transform.position = Input.mousePosition; // The draggable follows the mouse
    }

    // Se llama cuando el arrastre termina
    public void OnEndDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition = currentPosition;
        canvasGroup.blocksRaycasts = true;
        Destroy(draggable); // Destroy the draggable
    }

    public void OnDrop(PointerEventData eventData)
    {
        UnityEngine.Debug.Log("OnDrop called");

        var slot = eventData.pointerDrag.GetComponent<InventorySlot>();
        if (slot == null) return;

        InventorySlot fromSlot = slot;
        InventorySlot toSlot = this;
        
        if(fromSlot.onPlayer && toSlot.onPlayer)
        {
            InventoryManager.Instance.MoveItem(fromSlot.instanceId, toSlot.instanceId, xPosition, yPosition); // Move the item between slots
        }
        else if(!fromSlot.onPlayer && !toSlot.onPlayer)
        {
            ContainerManager.Instance.MoveItemInContainer(inventoryToggle.currentContainer, fromSlot.instanceId, toSlot.instanceId, xPosition, yPosition); // Move the item between slots
        }
        else
        {
            if (fromSlot.onPlayer) {
                ContainerManager.Instance.SwapItems(InventoryManager.Instance.playerContainer.containerId, inventoryToggle.currentContainer, fromSlot.instanceId, toSlot.instanceId, xPosition, yPosition);
            }
            else
            {
                ContainerManager.Instance.SwapItems(inventoryToggle.currentContainer, InventoryManager.Instance.playerContainer.containerId, fromSlot.instanceId, toSlot.instanceId, xPosition, yPosition);
            }
        }

    }

    public void OnSecondaryAction(InputAction.CallbackContext context)
    {
        if (context.performed && draggable != null && itemId != -1 && inventoryToggle.isInventoryOpen) // If the secondary action is performed and the mouse is over the inventory slot
        {
            ItemStack itemStack = ContainerManager.Instance.GetContainer(containerData.containerId).itemStack.Find(i => i.instanceId == instanceId); // Get the item stack from the container data
            itemStack.orientation = itemStack.orientation == 0 ? 1 : 0; // Change the orientation of the item stack
        }
    }

    public void itemAmmountIsVisible()
    {
        bool isVisible = true; 

        if (itemAmmount.text == "1" || itemAmmount.text == "0")    // If there's only one item in the slot
        {
            isVisible = false; 
        }
        else
        {
            isVisible = true; 
        }

        Transform itemAmmountTransform = transform.Find("Item_Ammount");
        if (itemAmmountTransform != null)
        {
            itemAmmountTransform.gameObject.SetActive(isVisible);
        }
        else
        {
            UnityEngine.Debug.LogError("ItemAmmount not found as a child of this GameObject.");
        }
    }



    private void OnDestroy()
    {
        if (selectPositionAction != null)
        {
            selectPositionAction.performed -= OnSelectPosition;
            //selectPositionAction.Disable();
            //UnityEngine.Debug.Log("OnSelectPosition desuscrito en OnDestroy.");
        }
    }
}
