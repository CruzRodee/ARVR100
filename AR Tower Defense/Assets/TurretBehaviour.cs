using System.Collections;
using UnityEngine;

public class TurretBehaviour : MonoBehaviour
{
    private Animator animator;
    [SerializeField]
    private GameObject bulletPrefab; // Assign the bullet prefab here
    [SerializeField]
    private float bulletSpeed; // Speed of the bullet
    [SerializeField]
    private float fireRate = 1f; // Time in seconds between shots

    private bool canFire = true; // Controls firing cooldown

    void Start()
    {
        // Retrieve the Animator component
        animator = GetComponent<Animator>();
        Debug.Log("[TurretBehaviour] Animator component found and assigned.");
    }

    public void Fire()
    {
        if (!canFire) return; // If cooldown is active, prevent firing

        Debug.Log("[TurretBehaviour] Fire() called.");
        if (animator != null)
        {
            // Trigger the Fire animation
            animator.SetTrigger("FireTrigger");
            Debug.Log("[TurretBehaviour] Fire animation triggered.");

            // Spawn the bullet
            SpawnBullet();

            // Start cooldown to prevent immediate refiring
            StartCoroutine(FireCooldown());

            // Start the coroutine to return to idle after firing
            StartCoroutine(ReturnToIdleAfterFire());
        }
        else
        {
            Debug.LogError("[TurretBehaviour] Animator component is missing!");
        }
    }

    private void SpawnBullet()
    {
        if (bulletPrefab != null)
        {
            // Calculate the spawn position a bit forward from the muzzle point
            Vector3 spawnPosition = transform.position + transform.forward * 0.1f + transform.up * 0.04f;

            // Instantiate the bullet at the offset position with the turret's rotation
            GameObject spawnedBullet = Instantiate(bulletPrefab, spawnPosition, transform.rotation);
            Rigidbody bulletRb = spawnedBullet.GetComponent<Rigidbody>();

            if (bulletRb != null)
            {
                // Add forward force to the bullet
                bulletRb.AddForce(transform.forward * bulletSpeed);
                Debug.Log("[TurretBehaviour] Bullet spawned and force applied.");
            }
            else
            {
                Debug.LogError("[TurretBehaviour] No Rigidbody found on bullet prefab!");
            }
        }
        else
        {
            Debug.LogError("[TurretBehaviour] Bullet prefab is missing!");
        }
    }

    private IEnumerator FireCooldown()
    {
        canFire = false; // Disable firing
        yield return new WaitForSeconds(fireRate); // Wait for cooldown duration
        canFire = true; // Re-enable firing
        Debug.Log("[TurretBehaviour] Ready to fire again.");
    }

    private IEnumerator ReturnToIdleAfterFire()
    {
        Debug.Log("[TurretBehaviour] Returning to Idle after Fire.");
        // Wait for the Fire animation to complete
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // Trigger stop animation to return to idle
        animator.SetTrigger("StopFire");
        Debug.Log("[TurretBehaviour] Returned to Idle state.");
    }
}
