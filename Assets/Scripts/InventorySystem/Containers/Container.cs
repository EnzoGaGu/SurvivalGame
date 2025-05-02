using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Container : MonoBehaviour, IInteractable
{
    public string containerId; // Unique identifier for the container
    public ContainerData containerData; // Reference to the container data

    public float interactingAngle = 45f; // Angle to interact with the container

    public ItemDatabase itemDatabase;
    private InputHandler inputHandler;
    private InputAction interactAction; // Acción de interactuar

    private SphereCollider sphereCollider;
    private bool isPlayerInTrigger = false; // Variable to know if the player is in the trigger
    private bool isTarget = false; 
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

    private void Update()
    {
        if (isPlayerInTrigger)
        {
            InteractionManager.Instance.PlayersObjectiveCandidate(transform, interactingAngle); // Llamar al método para saber si el jugador está mirando al objeto

            if (InteractionManager.Instance.currentObjectiveTransform == transform)
            {
                PromptController.Instance.ShowPrompt(transform); // Mostrar el prompt
                isTarget = true;
            }
            else
            {
                isTarget = false;
            }
        }
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
            isTarget = false; 
            isPlayerInTrigger = false;
            inventoryToggle.inventoryPanel.transform.Find("Inventory_Panel/Container_Side").gameObject.SetActive(false); // Show the container side of the inventory

            if (InteractionManager.Instance.currentObjectiveTransform == transform)
            {
                PromptController.Instance.HidePrompt();
            }
        }
    }

    public string GetPromptText()
    {
        return $"E to open {containerData.containerName}"; // Return the container name
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (isPlayerInTrigger && isTarget && PromptController.Instance.target == transform)
        {
            inventoryToggle.ToggleInventory(context); // Open the inventory

            inventoryUI.StartInventoryUI(containerData.inventoryGrid, containerData.rowSize, containerId); // Start the inventory UI

            inventoryToggle.inventoryPanel.transform.Find("Inventory_Panel/Container_Side").gameObject.SetActive(true); // Show the container side of the inventory
            inventoryToggle.inventoryPanel.transform.Find("Inventory_Panel/Container_Side/Container_Name").GetComponent<TMPro.TextMeshProUGUI>().text = containerData.containerName; // Set the container name
            inventoryToggle.currentContainer = containerId; // Set the current container
        }
    }
}
