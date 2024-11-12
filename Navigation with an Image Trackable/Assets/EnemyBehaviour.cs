using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private int health = 100; // Starting health of the enemy

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("[Enemy] Health: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("[Enemy] Enemy has been destroyed.");
        Destroy(gameObject); // Destroy the enemy object
    }
}
