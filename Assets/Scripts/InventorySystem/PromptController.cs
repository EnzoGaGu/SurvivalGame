using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class PromptController : MonoBehaviour
{
    public static PromptController Instance { get; private set; }
    public Transform target; // El objetivo a seguir
    public float distance = 0.5f; // Distancia del prompt al objetivo
    public float height = 0.5f; //Altura del prompt
    private Transform playerTransform;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        HidePrompt();
        playerTransform = Camera.main.transform;
    }

    
    void Update()
    {
        if(playerTransform != null && target != null)
        {
            // Calcular la dirección desde el objeto hacia el jugador
            Vector3 directionToPlayer = (playerTransform.position - target.position).normalized;

            // Calcular la posición del prompt
            Vector3 promptPosition = target.position + (Vector3.up*height);
            //promptPosition.y = target.position.y + height;

            // Ajustar la posición del prompt en el espacio del mundo
            transform.position = promptPosition;

            // Hacer que el prompt mire hacia el jugador
            transform.LookAt(playerTransform);
            transform.Rotate(0, 180, 0);
        }
    }

    public void ShowPrompt(Transform target)
    {
        this.target = target; 
        gameObject.SetActive(true);
        //UnityEngine.Debug.Log("Prompt shown");
    }

    public void HidePrompt()
    {
        gameObject.SetActive(false);
        ItemPickup[] allItems = FindObjectsByType<ItemPickup>(FindObjectsSortMode.None);
        Transform nextItemTransform = null;

        foreach (ItemPickup item in allItems)
        {
            if (!item.isCollected && item.isPlayerInTrigger && item.playerIsLooking)
            {
                nextItemTransform = item.transform;
                break; 
            }
        }

        if(nextItemTransform != null)
        {
            ShowPrompt(nextItemTransform);
        }
        else
        {
            target = null; 
            //UnityEngine.Debug.Log("Prompt hidden");
        }
    }

    public Transform GetTarget() {
        return target; 
    }
}
