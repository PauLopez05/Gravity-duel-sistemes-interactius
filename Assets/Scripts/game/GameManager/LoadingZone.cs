using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class LoadingZone : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image loadingCircle;

    [Header("Calibration Settings")]
    [Tooltip("Time in seconds the player must stay still to calibrate.")]
    [SerializeField] private float calibrationDuration = 3f;

    [Tooltip("How much height jitter (up/down) is allowed per frame. Lower = stricter.")]
    [SerializeField] private float stabilityThreshold = 0.02f;

    [Header("Player Save")]
    [Tooltip("Unique ID for this player, used in the save file name.")]
    [SerializeField] private string playerId;

    [Header("Game Flow")]
    [Tooltip("Reference to the shared manager that starts the next scene when both players are ready.")]
    [SerializeField] private CalibrationManager calibrationManager;

    private bool isPlayerInside = false;
    private bool isFinished = false;
    private float calibrationTimer = 0f;

    private PlayerMovement playerMovement;
    private float lastFrameHeight;

    private void Start()
    {
        if (loadingCircle != null)
        {
            loadingCircle.fillAmount = 0f;
        }
    }

    private void Update()
    {
        if (isFinished)
        {
            return;
        }

        if (isPlayerInside && playerMovement != null)
        {
            float currentHeight = playerMovement.Y;
            float heightDifference = Mathf.Abs(currentHeight - lastFrameHeight);

            if (heightDifference > stabilityThreshold)
            {
                calibrationTimer = Mathf.Max(0f, calibrationTimer - (Time.deltaTime * 2f));
            }
            else
            {
                calibrationTimer += Time.deltaTime;
            }

            if (loadingCircle != null)
            {
                loadingCircle.fillAmount = calibrationTimer / calibrationDuration;
            }

            if (calibrationTimer >= calibrationDuration)
            {
                FinishCalibration();
            }

            lastFrameHeight = currentHeight;
        }
        else
        {
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
            if (!other.CompareTag("Player")) return;

            var pm = other.GetComponent<PlayerMovement>();
            var team = $"Player{other.GetComponent<SpaceShip>().team}";
            if (pm == null) return;

            if (!string.Equals(team, playerId, System.StringComparison.OrdinalIgnoreCase)) return;
            isPlayerInside = true;
            playerMovement = pm;
            lastFrameHeight = playerMovement.Y;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            var pm = other.GetComponent<PlayerMovement>();
            var team = $"Player{other.GetComponent<SpaceShip>().team}";

            if (pm == null) return;

            if (!string.Equals(team, playerId, System.StringComparison.OrdinalIgnoreCase)) return;

            isPlayerInside = false;
            playerMovement = null;
        }

    private void FinishCalibration()
    {
        if (isFinished)
        {
            return;
        }

        isFinished = true;

        string safePlayerId = playerId.Trim();
        string fileName = $"CalibratedHeight_{safePlayerId}.txt";
        string filePath = Path.Combine(Application.persistentDataPath, fileName);

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine(lastFrameHeight.ToString());
        }

        Debug.Log($"Saved height ({lastFrameHeight}) to: {filePath}");

        if (calibrationManager != null)
        {
            calibrationManager.MarkPlayerReady(playerId);
        }

        if (loadingCircle != null)
        {
            loadingCircle.fillAmount = 1f;
        }

        enabled = false;
    }
}