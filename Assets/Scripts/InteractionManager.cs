using UnityEngine;
using System.Diagnostics;
using System.Collections;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance;

    public Transform currentObjectiveTransform = null; // Referencia al transform del objetivo
    public float closestDotProduct = -1;

    private Transform playerCameraTransform; // Referencia al transform de la c�mara del jugador

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        playerCameraTransform = Camera.main.transform; // Referencia al transform de la c�mara del jugador
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
        Vector3 directionToItem = (objectiveTransform.position - playerCameraTransform.position).normalized;     // Direcci�n al item
        float dotProduct = Vector3.Dot(playerCameraTransform.forward, directionToItem);     // Producto punto entre la direcci�n de la c�mara y la direcci�n al item

        if (dotProduct > Mathf.Cos(interactingAngle * Mathf.Deg2Rad))     // Si el jugador est� mirando al objeto
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
