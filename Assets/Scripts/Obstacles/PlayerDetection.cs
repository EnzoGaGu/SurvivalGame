using UnityEngine;
using System.Collections;
using System;

public class PlayerDetection : MonoBehaviour
{
    public event Action<bool> OnPlayerDetected; 

    public float visionRange = 10f;
    public float visionAngle = 60f; // Grados
    public LayerMask playerLayer;
    public LayerMask obstacleLayer;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UnityEngine.Debug.Log("Found a collider");

            //Check if the player is inside the vision radious (in front of the zombie)
            Vector3 directionToTarget = (other.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, directionToTarget);

            if (angle < visionAngle / 2)
            {
                //Send a raycast to detect if there's something between the zombie and the target
                if (!Physics.Raycast(transform.position, directionToTarget, visionRange, obstacleLayer))
                {
                    //If not, the player is detected
                    UnityEngine.Debug.Log("Jugador detectado!");
                    OnPlayerDetected?.Invoke(true);
                }
            }
        }
    }
}
