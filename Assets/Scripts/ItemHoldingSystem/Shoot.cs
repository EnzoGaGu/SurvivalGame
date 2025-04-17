using UnityEngine;
using System.Collections;

public class Shoot : MonoBehaviour
{
    public GameObject projectilePrefab;
    public GameObject muzzleFlashPrefab;
    public Transform projectileSpawnPoint;
    public float ejectionForce = 2f;
    public float projectileLifetime = 2;
    public float shootCooldown = 0.0f;
    private Camera playerCamera;
    private AudioSource audioSource; 

    void Start()
    {
        projectileSpawnPoint = transform.GetChild(1);

        playerCamera = Camera.main;
        audioSource = GetComponent<AudioSource>();
    }

    public void ShootProjectile()
    {
        if (Time.time >= shootCooldown)
        {
            if(audioSource != null)
            {
                audioSource.PlayOneShot(audioSource.clip);
            }
            else
            {
                UnityEngine.Debug.Log("No audio source found, the item has no audio");
            }
            projectileSpawnPoint.LookAt(GetAimPoint());
            
            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
            StartCoroutine(muzzleFlash());

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            rb.AddForce(projectileSpawnPoint.forward * ejectionForce, ForceMode.Impulse);
            Destroy(projectile, projectileLifetime);
            //shootCooldown = Time.time + shootCooldown;

            
        }
    }

    private Vector3 GetAimPoint()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;
        // Si el raycast golpea algo, usamos ese punto, sino disparamos a un punto lejano
        if (Physics.Raycast(ray, out hit, 1000f, ~0, QueryTriggerInteraction.Ignore))
        {
            return hit.point;  // Punto donde el raycast impactó
        }
        else
        {
            return playerCamera.transform.position + playerCamera.transform.forward * 1000f; // Punto lejano en la dirección de la mira
        }
    }
    
    private IEnumerator muzzleFlash()
    {
        GameObject muzzleFlash = Instantiate(muzzleFlashPrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        yield return new WaitForSeconds(0.05f);
        Destroy(muzzleFlash);
    }
}
