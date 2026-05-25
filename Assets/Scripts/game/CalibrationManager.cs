using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CalibrationManager : MonoBehaviour
{
    [Header("Players")]
    [SerializeField] private string[] requiredPlayerIds = { "Player1", "Player2" };

    [Header("Scene")]
    [SerializeField] private string nextSceneName = "SampleScene";

    private readonly HashSet<string> readyPlayers = new HashSet<string>();
    private bool hasLoadedScene = false;

    public void MarkPlayerReady(string playerId)
    {
        if (hasLoadedScene)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(playerId))
        {
            Debug.LogWarning("Player ID is empty.");
            return;
        }

        readyPlayers.Add(playerId.Trim());

        if (AllPlayersReady())
        {
            hasLoadedScene = true;
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private bool AllPlayersReady()
    {
        foreach (string requiredId in requiredPlayerIds)
        {
            if (!readyPlayers.Contains(requiredId.Trim()))
            {
                return false;
            }
        }

        return true;
    }
}