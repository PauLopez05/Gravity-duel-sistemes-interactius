using UnityEngine;

public class PoweUp : MonoBehaviour
{
[Header("Power-Up Settings")]
    [Tooltip("Drag and drop your power-up prefabs here.")]
    public GameObject[] powerUpPrefabs;
    
    [Tooltip("Time in seconds between each spawn.")]
    public float spawnInterval = 5f;
    
    [Tooltip("Time in seconds before the first spawn.")]
    public float initialDelay = 2f;

    [Header("Spawn Area")]
    [Tooltip("The center point of your spawning area relative to this object.")]
    public Vector3 spawnAreaCenter = Vector3.zero;
    
    [Tooltip("The size of the box where power-ups can spawn.")]
    public Vector3 spawnAreaSize = new Vector3(10f, 0f, 10f);

    private void Start()
    {
        if (powerUpPrefabs.Length > 0)
        {
            // Start the repeating spawn cycle
            InvokeRepeating(nameof(SpawnRandomPowerUp), initialDelay, spawnInterval);
        }
        else
        {
            Debug.LogWarning("PowerUpSpawner: No power-up prefabs assigned in the inspector!");
        }
    }

    private void SpawnRandomPowerUp()
    {
        int randomIndex = Random.Range(0, powerUpPrefabs.Length);
        GameObject selectedPrefab = powerUpPrefabs[randomIndex];

        
        Vector3 randomPosition = new Vector3(
            Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f),
            Random.Range(-spawnAreaSize.y / 2f, spawnAreaSize.y / 2f),
            Random.Range(-spawnAreaSize.z / 2f, spawnAreaSize.z / 2f)
        );

        Vector3 finalSpawnPosition = transform.position + spawnAreaCenter + randomPosition;
        GameObject Pu = Instantiate(selectedPrefab, finalSpawnPosition, Quaternion.identity);
        Rigidbody rb = Pu.GetComponent<Rigidbody>();
        rb.AddTorque(Vector3.up * 5.0f, ForceMode.Impulse);
        
    }


}
