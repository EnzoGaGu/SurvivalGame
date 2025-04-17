using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class ZombieController : MonoBehaviour
{
    [Header("Zombie")]
    [Tooltip("Move speed of the character in m/s")]
    public float MoveSpeed = 1.0f;
    [Tooltip("Rotation speed of the character")]
    public float RotationSpeed = 1.0f;
    [Tooltip("Acceleration and deceleration")]
    public float SpeedChangeRate = 10.0f;

    [Header("Zombie parts")]
    public GameObject Head;
    public GameObject lArm; // Left arm
    public GameObject rArm; // Right arm

    [Header("Player detection")]
    public GameObject playerDetection;
    private PlayerDetection playerDetectionScript; 

    private GameObject player;
    private Animator animator;
    private EnemyStats enemyStats;
    private bool isBeingDamaged;
    private bool isInvincible = false;
    private bool hasDetectedPlayer = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        //transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        animator = GetComponent<Animator>();
        enemyStats = GetComponent<EnemyStats>();


        playerDetectionScript = playerDetection.GetComponent<PlayerDetection>();

        if (playerDetectionScript != null)
        {
            playerDetectionScript.OnPlayerDetected += DetectPlayer; 
        }
        else
        {
            UnityEngine.Debug.LogWarning("No playerDetectionScript found");
        }
    }

    private void Update()
    {
        WalkTowardsPlayer();
        Attack();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Weapon") || other.CompareTag("Bullet"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();

            if (rb != null/* && rb.linearVelocity.magnitude > 0f*/ && !isInvincible)
            {
                enemyStats.TakeDamage(10);
                animator.SetTrigger("damage");
                DetectPlayer(true);
                StartCoroutine(waitForDamageAnimation());
                if (other.CompareTag("Weapon"))
                {
                    StartCoroutine(InvincibilityTime());
                }
            }
            else if(rb == null)
            {
                UnityEngine.Debug.Log("No rigidbody attached to the collider");
            }
        }
    }

    private void DetectPlayer(bool detected)
    {
        hasDetectedPlayer = detected;
        if (hasDetectedPlayer)
        {
            playerDetectionScript = null;
            Destroy(playerDetection);
        }
    }

    private void WalkTowardsPlayer()
    {
        if (player != null && !enemyStats.isDead && !isBeingDamaged && hasDetectedPlayer && !enemyStats.isAttacking)
        {
            Vector3 direction = player.transform.position - transform.position;
            direction.y = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), RotationSpeed * Time.deltaTime);
            transform.Translate(0, 0, MoveSpeed * Time.deltaTime);
            if (animator != null)
            {
                animator.SetFloat("speed", MoveSpeed);
            }

        }
    }

    private void Attack()
    {
        if(Vector3.Distance(transform.position, player.transform.position) < 3f)
        {
            animator.SetTrigger("attack");
        }

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("rig|Attack"))
        {
            enemyStats.isAttacking = true;
            lArm.GetComponent<Collider>().isTrigger = true;
            rArm.GetComponent<Collider>().isTrigger = true;
        }
        else
        {
            enemyStats.isAttacking = false;
            lArm.GetComponent<Collider>().isTrigger = false;
            rArm.GetComponent<Collider>().isTrigger = false;
        }

        
    }

    private IEnumerator waitForDamageAnimation()
    {
        isBeingDamaged = true;
        yield return new WaitForSeconds(3f);
        isBeingDamaged = false;
    }

    private IEnumerator InvincibilityTime()
    {
        isInvincible = true;
        yield return new WaitForSeconds(0.5f);
        isInvincible = false;
    }
}
