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
    [SerializeField] private float returnDelay = 6f;

    [Header("Win FX")]
    [SerializeField] private GameObject confettiPrefab;
    [SerializeField] private float confettiLifetime = 4f;
    private bool isReturning;

    [Header("Win SFX")]
    [SerializeField] private AudioClip winSfx;
    private AudioSource audioSource;

    [Header("Audio")]
    [SerializeField] private Perfect_loop backgroundLoop;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
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
        
        if (backgroundLoop != null)
        {
            backgroundLoop.StopLoop();
        }

        if (deathMessage != null)
        {
            int winner = (player == 1) ? 1 : 2;
            deathMessage.text = "Player " + winner + " wins";
            deathMessage.color = (winner == 1) ?  new Color(0.54f, 0.17f, 0.89f)
                                        : new Color(0.80f, 0.36f, 0.36f);
            deathMessage.gameObject.SetActive(true);
        }

        if (audioSource != null && winSfx != null)
        {
            audioSource.PlayOneShot(winSfx);
        }

        SpawnConfetti();
        StartCoroutine(ReturnToMainScene());
    }

    private void SpawnConfetti()
    {
        if (confettiPrefab == null)
        {
            return;
        }

        Vector3[] points = new Vector3[10];

        for (int i = 0; i < 5; i++)
        {
            points[i] = new Vector3(30f, 2f + i * 2f, 0f);
        }

        for (int i = 0; i < 5; i++)
        {
            points[i + 5] = new Vector3(-40f, 2f + i * 2f, 0f);
        }

        foreach (Vector3 position in points)
        {
            GameObject fx = Instantiate(confettiPrefab, position, Quaternion.identity);
            ParticleSystem ps = fx.GetComponent<ParticleSystem>() ?? fx.GetComponentInChildren<ParticleSystem>();
            if (ps != null) ps.Play();
            Destroy(fx, confettiLifetime);
        }
    }

    private IEnumerator ReturnToMainScene()
    {
        isReturning = true;
        yield return new WaitForSeconds(returnDelay);
        SceneManager.LoadScene(mainSceneName);
    }
}
