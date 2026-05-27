using UnityEngine;

public class asteroid_Manager : MonoBehaviour
{
    [Header("Spawner Settings")]
    public asteroid_spawn[] spawners; // Drag your 4 spawner GameObjects here in the Inspector
    public float spawnInterval = 3.0f;
    public int maxTotalAsteroids = 20; // The global limit across all spawners

    private float lastSpawnTime = 0f;

    void Update()
    {
        if (Time.time > lastSpawnTime + spawnInterval)
        {
            TrySpawnAsteroid();
            lastSpawnTime = Time.time;
        }
    }

    void TrySpawnAsteroid()
    {
        // 1. Check if we are already at or above the global limit
        // We find all active asteroids by searching for the component type
        GameObject[] currentAsteroids = GameObject.FindGameObjectsWithTag("Rocks"); 
        
        if (currentAsteroids.Length >= maxTotalAsteroids)
        {
            return; // Too many asteroids! Skip spawning this time.
        }

        // 2. Pick a random spawner from the array
        if (spawners != null && spawners.Length > 0)
        {
            int randomIndex = Random.Range(0, spawners.Length);
            spawners[randomIndex].spawn_asteroid();
        }
    }
}