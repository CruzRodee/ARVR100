using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private int damage = 20; // Damage dealt by the bullet

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the bullet hit an enemy
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage); // Apply damage to the enemy
            Destroy(gameObject); // Destroy the bullet on impact
        }
        else
        {
            // Optionally, destroy the bullet if it hits anything else
            Destroy(gameObject);
        }
    }
}
