using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab; // The enemy prefab to spawn
    [SerializeField]
    private float spawnRate = 10f; // Time in seconds between enemy spawns
    [SerializeField]
    private float spawnDistance = 1.5f; // Distance in front of the turret to spawn enemies

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(spawnRate); // Wait before spawning the next enemy
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab != null)
        {
            // Calculate spawn position directly in front of the turret
            Vector3 spawnPosition = transform.position + transform.forward * spawnDistance;

            // Instantiate the enemy at the calculated position with the turret's rotation
            Instantiate(enemyPrefab, spawnPosition, transform.rotation);
            Debug.Log("[EnemySpawner] Enemy spawned in front of the turret.");
        }
        else
        {
            Debug.LogError("[EnemySpawner] Enemy prefab is missing!");
        }
    }
}
