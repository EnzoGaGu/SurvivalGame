using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;


public class InventoryUI : MonoBehaviour
{
    public GameObject itemPrefab;
    public GameObject hotbarItemPrefab; 
    public Transform InventoryPanel;
    public Transform hotbarPanel;
    public Transform ContainerPanel;

    private string playerContainerId;
    private string otherContainerId;

    private Hotbar hotbar;
    private GridLayoutGroup gridLayoutGroup;
    private GridLayoutGroup gridLayoutGroupContainer;

    private Dictionary<(int x, int y), InventorySlot> inventorySlotGrid = new Dictionary<(int x, int y), InventorySlot>();
    private Dictionary<(int x, int y), InventorySlot> containerSlotGrid = new Dictionary<(int x, int y), InventorySlot>();

    private void Start()
    {
        gridLayoutGroup = InventoryPanel.GetComponent<GridLayoutGroup>();
        gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount; // Fija el número de columnas

        gridLayoutGroupContainer = ContainerPanel.GetComponent<GridLayoutGroup>();
        gridLayoutGroupContainer.constraint = GridLayoutGroup.Constraint.FixedColumnCount; // Fija el número de columnas

        playerContainerId = InventoryManager.Instance.playerContainer.containerId;
        StartInventoryUI(InventoryManager.Instance.playerContainer.inventoryGrid, InventoryManager.Instance.rowSize, playerContainerId);
        //UpdateHotbarUI();

        // The background item for the inventoryPanel gets refreshed to readjust to inventory size
        LayoutRebuilder.ForceRebuildLayoutImmediate(InventoryPanel.parent.GetComponent<RectTransform>());
    }

    public void StartInventoryUI(List<List<int>> inventoryGrid, int rowSize, string containerId)
    {
        Transform panel;

        if (containerId == playerContainerId)
        {
            panel = InventoryPanel;
            gridLayoutGroup.constraintCount = rowSize; // rowSize es el número de columnas
        }
        else
        {
            panel = ContainerPanel;
            gridLayoutGroupContainer.constraintCount = rowSize; // rowSize es el número de columnas
        }

        foreach (Transform child in panel)
        {
            Destroy(child.gameObject);
        }

        // Recorre la matriz para colocar los ítems en la UI
        for (int y = 0; y < inventoryGrid.Count; y++)
        {
            for (int x = 0; x < inventoryGrid[y].Count; x++)
            {
                int instanceId = inventoryGrid[y][x];

                //UnityEngine.Debug.Log($"{instanceId}");

                GameObject slot = Instantiate(itemPrefab, panel);

                InventorySlot slotScript = slot.GetComponent<InventorySlot>();
                if (slotScript != null)
                {
                    slotScript.SetItem(instanceId, containerId);
                    slotScript.xPosition = x;
                    slotScript.yPosition = y;

                    // Asignar la posición en la cuadrícula
                    RectTransform slotRect = slot.GetComponent<RectTransform>();
                    slotRect.anchoredPosition = new Vector2(x, -y);

                    if(containerId == playerContainerId)
                    {
                        // Marcar el ítem como colocado para no duplicarlo
                        inventorySlotGrid[(x, y)] = slotScript;
                    }
                    else
                    {
                        // Marcar el ítem como colocado para no duplicarlo
                        containerSlotGrid[(x, y)] = slotScript;
                    }
                    
                }
                else
                {
                    UnityEngine.Debug.LogError("InventorySlot script is missing in the prefab.");
                }

            }
        }
    }

    public void UpdateInventoryUI(ItemPosition itemPosition)
    {
        // Recorre la matriz para colocar los ítems en la UI
        for (int y = itemPosition.y; y < itemPosition.y+itemPosition.ysize; y++)
        {
            for (int x = itemPosition.x; x < itemPosition.x+itemPosition.xsize; x++)
            {
                InventorySlot slotScript = GetInventorySlotAt(x, y, itemPosition.containerId);
                if (slotScript != null)
                {
                    UnityEngine.Debug.Log($"Updating item in position: {x}, {y} and container: {itemPosition.containerId} with instanceId: {itemPosition.instanceId}");
                    slotScript.SetItem(itemPosition.instanceId, itemPosition.containerId);
                    if (itemPosition.containerId == playerContainerId) //Player inventory
                    {
                        // Marcar el ítem como colocado para no duplicarlo
                        inventorySlotGrid[(x, y)] = slotScript;
                    }
                    else
                    {
                        // Marcar el ítem como colocado para no duplicarlo
                        containerSlotGrid[(x, y)] = slotScript;
                    }
                }
                else
                {
                    UnityEngine.Debug.LogError("InventorySlot script is missing in the prefab.");
                }
            }
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(InventoryPanel.parent.GetComponent<RectTransform>());

    }

    public void UpdateAmmountInInventory(int instanceId, int ammount)
    {
        foreach (var slot in inventorySlotGrid.Values)
        {
            if (slot.instanceId == instanceId)
            {
                slot.itemAmmount.text = ammount.ToString();    // Get the ammount of items in the slot from the itemStack
                slot.itemAmmountIsVisible();
                break;
            }
        }
    }


    public void StartHotbarUI()
    {
        hotbar = FindFirstObjectByType<Hotbar>();
        UnityEngine.Debug.Log(hotbar != null ? "Hotbar reference in InventoryUI is valid" : "Hotbar reference in InventoryUI is NULL");

        foreach (Transform child in hotbarPanel)
        {
            Destroy(child.gameObject);
        }
        hotbar.hotbarSlots.Clear();
        foreach (var i in hotbar.hotbarItems)
        {
            GameObject slot = Instantiate(hotbarItemPrefab, hotbarPanel);
            HotbarSlot slotScript = slot.GetComponent<HotbarSlot>();
            hotbar.hotbarSlots.Add(slotScript);
            if (slotScript != null)
            {

                slotScript.SetItem(i.instanceId);
            }
            else
            {
                UnityEngine.Debug.LogError("InventorySlot script is missing in the prefab.");
            }
        }
        hotbar.SelectHotbarSlot(hotbar.selectedPosition);
    }

    public void UpdateHotbarUI(int instanceId, int position)
    {
        HotbarSlot hotbarSlot = hotbar.hotbarSlots[position];
        if (hotbarSlot.instanceId != instanceId)
        {
            hotbarSlot.SetItem(instanceId);
        }
    }

    public InventorySlot GetInventorySlotAt(int x, int y, string containerid)
    {
        if(containerid == playerContainerId)
        {
            inventorySlotGrid.TryGetValue((x, y), out InventorySlot slot);
            UnityEngine.Debug.Log($"Slot: {slot} in player's inventory");
            return slot;
        }
        else
        {
            containerSlotGrid.TryGetValue((x, y), out InventorySlot slot);
            UnityEngine.Debug.Log($"Slot: {slot} in container: {containerid}");
            return slot;
        }
    }
}