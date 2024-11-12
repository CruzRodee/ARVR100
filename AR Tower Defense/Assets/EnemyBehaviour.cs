using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private int health = 100; // Starting health of the enemy
    [SerializeField]
    private float speed = 2f; // Movement speed

    private void Update()
    {
        // Move forward in the direction the enemy is currently facing
        transform.position += -transform.forward * speed * Time.deltaTime;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("[Enemy] Health: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("[Enemy] Collision detected with " + collision.gameObject.name);

        // Check if the collided object has the "Turret" tag
        if (collision.gameObject.CompareTag("Turret"))
        {
            // Destroy the entire turret on collision
            Destroy(collision.gameObject);
            Debug.Log("[Enemy] Turret destroyed on collision.");

            // Destroy the enemy as well
            Destroy(gameObject);
            Debug.Log("[Enemy] Enemy destroyed after hitting turret.");
        }
    }

    private void Die()
    {
        Debug.Log("[Enemy] Enemy has been destroyed.");
        Destroy(gameObject); // Destroy the enemy object
    }
}
