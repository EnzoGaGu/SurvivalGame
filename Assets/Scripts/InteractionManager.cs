using UnityEngine;
using System.Diagnostics;
using System.Collections;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance;

    public Transform currentObjectiveTransform = null; // Referencia al transform del objetivo
    public float closestDotProduct = -1;

    private Transform playerCameraTransform; // Referencia al transform de la cámara del jugador

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        playerCameraTransform = Camera.main.transform; // Referencia al transform de la cámara del jugador
    }

    void Update()
    {
        ResetObjective();
    }

    public void ResetObjective()
    {
        currentObjectiveTransform = null;
        closestDotProduct = -1f;
    }

    public void PlayersObjectiveCandidate(Transform objectiveTransform, float interactingAngle)
    {
        Vector3 directionToItem = (objectiveTransform.position - playerCameraTransform.position).normalized;     // Dirección al item
        float dotProduct = Vector3.Dot(playerCameraTransform.forward, directionToItem);     // Producto punto entre la dirección de la cámara y la dirección al item

        if (dotProduct > Mathf.Cos(interactingAngle * Mathf.Deg2Rad))     // Si el jugador está mirando al objeto
        {
            if (dotProduct > closestDotProduct)
            {
                closestDotProduct = dotProduct;
                currentObjectiveTransform = objectiveTransform;
            }
        }
    }

    public void OnObjectiveInteracted()
    {
        if (currentObjectiveTransform != null)
        {
            closestDotProduct = -1;
            currentObjectiveTransform = null; // Resetear el objetivo actual
        }
    }

    public IEnumerator DeactivateColliderTemporarily(SphereCollider sphereCollider, float secondsUntilInteraction)
    {
        if (sphereCollider != null)
        {
            sphereCollider.enabled = false;
            yield return new WaitForSeconds(secondsUntilInteraction);
            sphereCollider.enabled = true;
        }
    }
}
