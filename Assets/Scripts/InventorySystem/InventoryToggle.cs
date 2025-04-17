using UnityEngine;
using StarterAssets;
using UnityEngine.InputSystem;

public class InventoryToggle : MonoBehaviour
{
    public GameObject inventoryPanel;
    public bool isInventoryOpen = false;
    public string currentContainer = "";
    private FirstPersonController firstPersonController;
    private InputAction inventoryToggle;
    private InputHandler inputHandler;

    private void Start()
    {
        firstPersonController = FindFirstObjectByType<FirstPersonController>();
        inputHandler = FindFirstObjectByType<InputHandler>();
        inventoryPanel.SetActive(false);

        inventoryToggle = inputHandler.LoadInputAction("UI/Inventory");
        if (inventoryToggle != null)
        {
            inventoryToggle.performed += ToggleInventory;
            inventoryToggle.Enable();
        }
    }

    public void ToggleInventory(InputAction.CallbackContext context) { 
        isInventoryOpen = !isInventoryOpen;
        inventoryPanel.SetActive(isInventoryOpen);

        if (isInventoryOpen)
        {
            firstPersonController.enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            firstPersonController.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            inventoryPanel.transform.Find("Inventory_Panel/Container_Side").gameObject.SetActive(true); // Show the container side of the inventory
        }
    }
}