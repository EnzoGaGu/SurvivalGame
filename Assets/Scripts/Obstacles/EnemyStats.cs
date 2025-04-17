using UnityEngine;
using System.Collections;

public class EnemyStats : MonoBehaviour
{
    public float maxHealth = 100;
    public float health;
    public bool isAwareOfPlayer = false;
    public bool isAttacking = false;
    public bool isDead = false;

    private Animator animator; 

    private void Start()
    {
        health = maxHealth;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }

    }

    private void Die()
    {
        //animator.SetBool("death", true);
        isDead = true;
        ActivateRagdoll();
        //Destroy(gameObject);
    }


    void ActivateRagdoll()
    {
        GetComponent<Animator>().enabled = false; // Desactiva la animación

        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = false; // Activa el ragdoll
        }

        foreach (Collider col in GetComponentsInChildren<Collider>())
        {
            col.enabled = true;  // Activar colisiones
        }
    }
}
