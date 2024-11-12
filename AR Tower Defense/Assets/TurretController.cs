using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        // Get the Animator component attached to this GameObject
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Example: Trigger the Fire animation when the space bar is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Fire();
        }
    }

    // Method to trigger the Fire animation and start coroutine to return to Idle
    public void Fire()
    {
        animator.SetTrigger("FireTrigger");
        StartCoroutine(ReturnToIdleAfterFire());
    }

    // Coroutine to wait until the Fire animation completes before returning to Idle
    private IEnumerator ReturnToIdleAfterFire()
    {
        // Wait for the Fire animation to finish
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // Set the StopTrigger to return to Idle
        animator.SetTrigger("StopFire");
    }
}

