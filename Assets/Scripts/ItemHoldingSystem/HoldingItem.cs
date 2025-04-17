using System.Diagnostics;
using System.Collections;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;

public class HoldingItem : MonoBehaviour
{
    public bool isHoldingSomething = false;
    public Animator animator;
    private Item currentlyHolding;
    private Hotbar hotbar;
    private Camera playerCamera;
    private InventoryToggle inventoryToggle;
    private InputHandler inputHandler;
    private InputAction actionInputAction;
    private AudioSource audioSource;
    private StaminaController staminaController;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hotbar = FindFirstObjectByType<Hotbar>();
        playerCamera = Camera.main;
        inventoryToggle = FindFirstObjectByType<InventoryToggle>();
        inputHandler = FindFirstObjectByType<InputHandler>();
        staminaController = FindFirstObjectByType<StaminaController>();

        if (hotbar == null || playerCamera == null || inventoryToggle == null)
        {
            UnityEngine.Debug.LogError("Componentes necesarios no encontrados.");
        }

        actionInputAction = inputHandler.LoadInputAction("Player/Action");
        if (actionInputAction != null)
        {
            actionInputAction.performed += HandleSwing;
            actionInputAction.Enable();
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleItemHolding();
        //HandleSwing();
    }

    public void EquipItemInHand()
    {
        if (isHoldingSomething)
        {
            GameObject itemObject = Instantiate(currentlyHolding.holdPrefab);

            itemObject.transform.position = transform.position;

            Quaternion newRotation = Quaternion.Euler(playerCamera.transform.rotation.eulerAngles.x, playerCamera.transform.rotation.eulerAngles.y, itemObject.transform.rotation.eulerAngles.z);

            itemObject.transform.rotation = newRotation; 

            itemObject.transform.SetParent(transform, true);
           
        }
    }

    public void UnequipItem()
    {
        if (this.isHoldingSomething && !animator.GetCurrentAnimatorStateInfo(0).IsName("Swing") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Shoot Pistol"))
        {
            Destroy(transform.GetChild(0).gameObject);  //Destroy the instance of the ítem that's being held in hand
            this.isHoldingSomething = false; //Set the player to not be holding anything
            this.currentlyHolding = null; //Set the currently held item to null
        }
    }

    //Handle how holding an item in hand works
    private void HandleItemHolding()
    {
        if (hotbar.selectedHotbarSlot.item != null && hotbar.selectedHotbarSlot.item.isHoldable && currentlyHolding == null) //If the item is holdable and the player is not holding anything
        { 
            this.isHoldingSomething = true;
            this.currentlyHolding = hotbar.selectedHotbarSlot.item;
            EquipItemInHand();
        }
        else if (hotbar.selectedHotbarSlot.item == null || !hotbar.selectedHotbarSlot.item.isHoldable || hotbar.selectedHotbarSlot.item != currentlyHolding) //If the player has nothing selected in the hotbar or the item is not holdable
        {
            UnequipItem();
            
        }
    }

    private void HandleSwing(InputAction.CallbackContext context)
    {
        var control = context.control;
        if (control != null && isHoldingSomething && !inventoryToggle.isInventoryOpen && !currentlyHolding.shoots && !staminaController.isTired)
        {
            animator.SetTrigger("Swing");
            staminaController.SpendStamina(currentlyHolding.swingCost);
            audioSource = transform.GetChild(0).GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.Play();
            }
            
        }
        else if (control != null && isHoldingSomething && !inventoryToggle.isInventoryOpen && currentlyHolding.shoots)
        {
            GameObject child = this.transform.GetChild(0).gameObject;
            if (child != null)
            {
                child.GetComponent<Shoot>().ShootProjectile();
                animator.SetTrigger("Shoot Pistol");
            }
        }
    }
}
