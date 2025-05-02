using System.Diagnostics;
using System.Collections;
using System.Runtime.Versioning;
using UnityEngine;
using UnityEngine.InputSystem;

public class ItemPickup : MonoBehaviour, IInteractable
{
    public ItemDatabase itemDatabase; // Referencia al item database

    public int itemId; // Id del item
    public int? instanceId; // Id de la instancia

    public float interactingAngle = 15; // Ángulo de recolección
    public float secondsUntilInteraction = 2; // Segundos hasta la interacción

    public bool isCollected = false; // Variable para saber si el item fue recogido
    public bool isPlayerInTrigger = false; // Variable para saber si el jugador está en el trigger
    public bool isTarget = false; // Variable para saber si el objeto es el target del prompt
    public bool isPromptVisible = false; // Variable para saber si el prompt está visible
    public bool playerIsLooking = false; // Variable para saber si el jugador está mirando al objeto

    private InputHandler inputHandler;
    private SphereCollider sphereCollider; // Collider tipo trigger para el item
    private InputAction interactAction; // Acción de interactuar

    public void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
        inputHandler = FindFirstObjectByType<InputHandler>();

        if (InteractionManager.Instance == null) { 
            
        }
        StartCoroutine(InteractionManager.Instance.DeactivateColliderTemporarily(sphereCollider, secondsUntilInteraction));

        interactAction = inputHandler.LoadInputAction("Player/Interact");
        if (interactAction != null)
        {
            interactAction.performed += OnInteract;
            interactAction.Enable();
        }

        if(itemDatabase.getItemById(itemId).unique)
        {
            instanceId = ItemUtils.GetNextFreeInstanceId(); // Get the next free instance id
        }
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

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // Si el jugador abandona el trigger
        {
            isTarget = false;
            isPlayerInTrigger = false;

            if(InteractionManager.Instance.currentObjectiveTransform == transform)
            {
                PromptController.Instance.HidePrompt();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
        }
    }

    public string GetPromptText()
    {
        return $"E to pick up"; // Return the container name
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        //UnityEngine.Debug.Log("Interact action performed.");
        if (!isCollected && isPlayerInTrigger && isTarget && PromptController.Instance.target == transform) // Si el jugador está en el trigger y mirando al item, y éste no ha sido recogido aún
        {
            isCollected = true; // El item fue recogido
            InventoryManager.Instance.AddItem(itemDatabase.getItemById(itemId), instanceId); // Añadir el item al inventario
            UnityEngine.Debug.Log($"Item: {itemDatabase.getItemById(itemId).getItemName()}");
            InteractionManager.Instance.OnObjectiveInteracted();
            PromptController.Instance.HidePrompt(); // Ocultar el prompt
            Destroy(gameObject); // Destruir el objeto
        }
    }
}