using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Container : MonoBehaviour
{
    public string containerId; // Unique identifier for the container
    public ContainerData containerData; // Reference to the container data

    public ItemDatabase itemDatabase;
    private InputHandler inputHandler;
    private InputAction interactAction; // Acci�n de interactuar

    private SphereCollider sphereCollider;
    private bool isPlayerInTrigger = false; // Variable to know if the player is in the trigger
    private bool isInventoryOpen = false; // Variable to know if the inventory is open
    private InventoryToggle inventoryToggle; // Reference to the inventory toggle script
    private InventoryUI inventoryUI;

    public void Start()
    {
        containerId = gameObject.scene.name + "_" + gameObject.name + "_" + transform.position.ToString(); // Set the container ID (scene name + object name + position)

        containerData = ContainerManager.Instance.LoadContainer(containerId); // Load the container data

        if (containerData.containerName == "")
        {
            containerData.containerName = gameObject.name; // Set the container name
        }

        InventoryUtils.InitializeMatrix(containerData.inventoryGrid, containerData.rowSize, containerData.containerSize); // Initialize the inventory grid
        InventoryUtils.WriteMatrix(containerData.inventoryGrid);
        containerData.itemStack.Add(new ItemStack(-1, itemDatabase.getEmptyItem(), 0, 0)); // Add an empty item to the inventory

        foreach (ItemStack i in containerData.itemStack)
        {
            if (i.instanceId != -1)
            {
                InventoryUtils.addItemToMatrix(containerData.inventoryGrid, InventoryUtils.GetFirstGridEmptySpace(containerData.inventoryGrid, containerData.rowSize, containerData.containerSize, i.item.xsize, i.item.ysize));
            }
        }

        sphereCollider = GetComponent<SphereCollider>();
        inventoryUI = FindFirstObjectByType<InventoryUI>();

        inputHandler = FindFirstObjectByType<InputHandler>();
        interactAction = inputHandler.LoadInputAction("Player/Interact");
        if (interactAction != null)
        {
            interactAction.performed += OnInteract;
            interactAction.Enable();
        }

        inventoryToggle = FindFirstObjectByType<InventoryToggle>();
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            inventoryToggle.inventoryPanel.transform.Find("Inventory_Panel/Container_Side").gameObject.SetActive(false); // Show the container side of the inventory
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (isPlayerInTrigger)
        {
            inventoryToggle.ToggleInventory(context); // Open the inventory

            inventoryToggle.inventoryPanel.transform.Find("Inventory_Panel/Container_Side").gameObject.SetActive(true); // Show the container side of the inventory
            inventoryToggle.inventoryPanel.transform.Find("Inventory_Panel/Container_Side/Container_Name").GetComponent<TMPro.TextMeshProUGUI>().text = containerData.containerName; // Set the container name
            inventoryToggle.currentContainer = containerId; // Set the current container

            inventoryUI.StartInventoryUI(containerData.inventoryGrid, containerData.rowSize, containerId); // Start the inventory UI
        }
    }
}
