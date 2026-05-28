using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text deathMessage;

    [Header("Scene")]
    [SerializeField] private string mainSceneName = "Inteface";

    [Header("Timing")]
    [SerializeField] private float returnDelay = 5f;

    private bool isReturning;

    private void OnEnable()
    {
        EventManager.OnPlayerDeath += HandlePlayerDeath;
    }

    private void OnDisable()
    {
        EventManager.OnPlayerDeath -= HandlePlayerDeath;
    }

    private void HandlePlayerDeath(int player)
    {
        if (isReturning)
        {
            return;
        }

        if (deathMessage != null)
        {
            int winner = (player == 1) ? 1 : 2;
            deathMessage.text = "Player " + winner + " wins";
            deathMessage.gameObject.SetActive(true);
        }
        StartCoroutine(ReturnToMainScene());
    }

    private IEnumerator ReturnToMainScene()
    {
        isReturning = true;
        yield return new WaitForSeconds(returnDelay);
        SceneManager.LoadScene(mainSceneName);
    }
}
