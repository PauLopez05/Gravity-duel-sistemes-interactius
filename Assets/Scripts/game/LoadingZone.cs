using UnityEngine;
using UnityEngine.UI;
using System.IO; // Required for saving to a text file

public class LoadingZone : MonoBehaviour
{
    [Header("UI Elements")]
    public Image loadingCircle; 

    [Header("Calibration Settings")]
    [Tooltip("Time in seconds the player must stay still to calibrate.")]
    public float calibrationDuration = 3f; 
    
    [Tooltip("How much height jitter (up/down) is allowed per frame. Lower = stricter.")]
    public float stabilityThreshold = 0.02f;

    private bool isPlayerInside = false;
    private float calibrationTimer = 0f;
    
    private Transform playerTransform;
    private float lastFrameHeight; // Stores the height from the previous frame

    void Start()
    {
        Debug.Log(Application.persistentDataPath);
        // Ensure the circle starts completely empty
        if (loadingCircle != null)
        {
            loadingCircle.fillAmount = 0f;
        }
    }

    void Update()
    {
        if (isPlayerInside && playerTransform != null)
        {
            // 1. Get the current height of the spaceship
            float currentHeight = playerTransform.position.y;
            
            // 2. Calculate how much the height changed since the LAST frame
            float heightDifference = Mathf.Abs(currentHeight - lastFrameHeight);

            // 3. Check if the player is moving too erratically
            if (heightDifference > stabilityThreshold)
            {
                Debug.LogWarning("Calibration interrupted! Hold still.");
                
                // Penalize player: smoothly drain progress if they shake too much
                calibrationTimer = Mathf.Max(0f, calibrationTimer - (Time.deltaTime * 2f));
            }
            else
            {
                // Player is steady! Advance the 3-second timer
                calibrationTimer += Time.deltaTime;
            }

            // 4. Update the green circle fill progress
            if (loadingCircle != null)
            {
                loadingCircle.fillAmount = calibrationTimer / calibrationDuration;
            }

            // 5. Check if calibration is finished
            if (calibrationTimer >= calibrationDuration)
            {
                StartGame();
            }

            // 6. Save the current height for the next frame's comparison
            lastFrameHeight = currentHeight;
        }
        else
        {
            // If the player flies out of the zone, lose progress slowly
            if (calibrationTimer > 0f)
            {
                calibrationTimer -= Time.deltaTime;
                if (loadingCircle != null)
                {
                    loadingCircle.fillAmount = calibrationTimer / calibrationDuration;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something entered the trigger: " + other.gameObject.name);
        // Check if the object entering the 3D trigger is tagged as the Player
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
            playerTransform = other.transform;
            
            // Initialize the height right as they enter so we don't get a massive jump on frame one
            lastFrameHeight = playerTransform.position.y;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the object leaving is the Player
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
            playerTransform = null;
        }
    }

    private void StartGame()
    {
        Debug.Log("Calibration Complete!");

        // 1. Define where to save the text file
        string filePath = Path.Combine(Application.persistentDataPath, "CalibratedHeight.txt");

        // 2. Write the last recorded height into the file
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine(lastFrameHeight.ToString());
        }

        Debug.Log($"Saved height ({lastFrameHeight}) to: {filePath}");

        // 3. Trigger your scene load here when you are ready to implement it
        // UnityEngine.SceneManagement.SceneManager.LoadScene("YourNextSceneName");

        // Disable this script so calibration doesn't run again
        this.enabled = false; 
    }
}