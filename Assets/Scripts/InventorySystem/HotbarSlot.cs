using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems; 
using static System.Net.Mime.MediaTypeNames;
using System.ComponentModel;

public class HotbarSlot : MonoBehaviour, IDropHandler
{
    public UnityEngine.UI.Image icon;
    public TMP_Text itemAmmount; // This is the ammount of items in the slot
    public Item item; 
    public int instanceId;
    public bool selected = false; // If the slot is selected
    public Color selectedColor = new Color();
    public Color deselectedColor = new Color();
    private Hotbar hotbar;

    public void SetItem(int instanceId)
    {
        hotbar = FindFirstObjectByType<Hotbar>();
        ItemStack itemStack = InventoryManager.Instance.playerContainer.itemStack.Find(i => i.instanceId == instanceId);
        if (itemStack != null)
        {
            icon.sprite = itemStack.item.getItemIcon();
            itemAmmount.text = itemStack.ammount.ToString();    // Get the ammount of items in the slot from the itemStack
            item = itemStack.item;

            if (itemAmmount.text == "1" || itemAmmount.text == "0")    // If there's only one item in the slot
            {
                itemAmmountIsVisible(false);   // Hide the ammount of items
            }
            else
            {
                itemAmmountIsVisible(true);   // Show the ammount of items
            }
        }
    }

    public void Select()
    {
        this.selected = true; // Set the slot as selected

        Transform selectedTransform = transform.Find("Selected");
        if (selectedTransform != null)
        {
            selectedTransform.gameObject.SetActive(true);
            ChangeIconColor(selectedColor);
        }
        else
        {
            UnityEngine.Debug.LogError("Selected child not found as a child of this GameObject.");
        }
    }

    public void Deselect()
    {
        this.selected = false; // Set the slot as not selected
        Transform selectedTransform = transform.Find("Selected");
        if (selectedTransform != null)
        {
            selectedTransform.gameObject.SetActive(false);
            ChangeIconColor(deselectedColor);

        }
        else
        {
            UnityEngine.Debug.LogError("Selected child not found as a child of this GameObject.");
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Get the item that is being dragged
        InventorySlot draggedItem = eventData.pointerDrag.GetComponent<InventorySlot>();
        if (draggedItem != null)
        {
            // Swap the items in the hotbar
            hotbar.AddToHotbar(draggedItem.instanceId, hotbar.hotbarSlots.FindIndex(slot => slot == this));
        }
    }


    private void itemAmmountIsVisible(bool isVisible)
    {
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

    public void ChangeIconColor(Color newColor)
    {
        if (icon != null)
        {
            icon.color = newColor;
        }
        else
        {
            UnityEngine.Debug.LogError("Icon Image component not found.");
        }
    }
}
