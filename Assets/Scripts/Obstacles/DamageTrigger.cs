using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    private PlayerStats playerStats;

    private void Start()
    {
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
    }


    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerStats.TakeDamage(10);
        }
    }
}
