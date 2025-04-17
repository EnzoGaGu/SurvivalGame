using System.Diagnostics;
using System.Collections;
using System.Runtime.Versioning;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemPickup : MonoBehaviour
{
    public ItemDatabase itemDatabase; // Referencia al item database
    public int itemId; // Id del item
    public int? instanceId = null; // Id de la instancia
    public float collectingAngle = 15; // Ángulo de recolección
    public float secondsUntilInteraction = 2; // Segundos hasta la interacción
    private SphereCollider sphereCollider; // Collider tipo trigger para el item
    public bool isCollected = false; // Variable para saber si el item fue recogido
    public bool isPlayerInTrigger = false; // Variable para saber si el jugador está en el trigger
    public bool isTarget = false; // Variable para saber si el objeto es el target del prompt
    private Transform playerCameraTransform; // Referencia al transform de la cámara del jugador
    public bool isPromptVisible = false; // Variable para saber si el prompt está visible
    public bool playerIsLooking = false; // Variable para saber si el jugador está mirando al objeto
    private InputAction interactAction; // Acción de interactuar
    private InputHandler inputHandler; 

    public void Start()
    {
        sphereCollider = FindFirstObjectByType<SphereCollider>();
        playerCameraTransform = Camera.main.transform; // Referencia al transform de la cámara del jugador
        inputHandler = FindFirstObjectByType<InputHandler>();
        if (sphereCollider == null)
        {
            UnityEngine.Debug.LogError("SphereCollider not found in the scene.");
        }
        StartCoroutine(DeactivateColliderTemporarily());

        interactAction = inputHandler.LoadInputAction("Player/Interact");
        if (interactAction != null)
        {
            interactAction.performed += OnInteract;
            interactAction.Enable();
        }

        if(itemDatabase == null)
        {
            UnityEngine.Debug.LogError("Item Database not found in the scene.");
        }

        if(itemDatabase.getItemById(itemId).unique)
        {
            instanceId = ItemUtils.GetNextFreeInstanceId(); // Get the next free instance id
        }
    }

    private void Update()
    {
        IsPlayerLookingAtItem();

        if (PromptController.Instance.GetTarget() != transform)     // Si el objeto no es el target del prompt
        {
            isTarget = false;   // El objeto no es el target
        }
        else
        {
            isTarget = true;    // El objeto es el target
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (itemDatabase == null)
        {
            UnityEngine.Debug.Log("Failed to load Item Database!");

        }
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // Si el jugador abandona el trigger
        {
            isTarget = false;
            isPlayerInTrigger = false;
            PromptController.Instance.HidePrompt();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
        }
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        //UnityEngine.Debug.Log("Interact action performed.");
        if (!isCollected && isPlayerInTrigger && isTarget && isPromptVisible) // Si el jugador está en el trigger y mirando al item, y éste no ha sido recogido aún
        {
            isCollected = true; // El item fue recogido
            InventoryManager.Instance.AddItem(itemDatabase.getItemById(itemId), instanceId); // Añadir el item al inventario
            UnityEngine.Debug.Log($"Item: {itemDatabase.getItemById(itemId).getItemName()}");
            Destroy(gameObject); // Destruir el objeto
            PromptController.Instance.HidePrompt(); // Ocultar el prompt
        }
    }

    public void IsPlayerLookingAtItem()
    {
        if (isPlayerInTrigger && !isCollected)
        {
            Vector3 directionToItem = (transform.position - playerCameraTransform.position).normalized;     // Dirección al item
            float dotProduct = Vector3.Dot(playerCameraTransform.forward, directionToItem);     // Producto punto entre la dirección de la cámara y la dirección al item

            if (dotProduct > Mathf.Cos(collectingAngle * Mathf.Deg2Rad))     // Si el jugador está mirando al objeto
            {
                if (!isPromptVisible) // Solo mostrar el prompt si no está ya visible
                {
                    PromptController.Instance.ShowPrompt(transform);
                    isPromptVisible = true;
                }
                playerIsLooking = true; // El jugador está mirando al objeto
            }
            else    // Si el jugador no está mirando al objeto
            {
                if (isPromptVisible) // Solo ocultar el prompt si está visible
                {
                    PromptController.Instance.HidePrompt();
                    isPromptVisible = false;
                }
                playerIsLooking = false; // El jugador no está mirando al objeto
            }
        }
        else
        {
            if (isPromptVisible) // Solo ocultar el prompt si está visible
            {
                PromptController.Instance.HidePrompt();
                isPromptVisible = false;
            }
        }
    }

    
    private IEnumerator DeactivateColliderTemporarily()
    {
        if (sphereCollider != null)
        {
            sphereCollider.enabled = false;
            yield return new WaitForSeconds(secondsUntilInteraction);
            sphereCollider.enabled = true;
        }
    }
}