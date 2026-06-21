using System.Collections;
using UnityEngine;
using TMPro;
using System;

public class TrashManager : MonoBehaviour
{
    public static TrashManager Instance;

    [Header("Trash Settings")]
    public int totalTrash = 2;
    public int collectedTrash = 0;

    [Header("UI References")]
    public TextMeshProUGUI trashCountText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI accuracyText;
    public GameObject wrongBinPanel;
    public TextMeshProUGUI wrongBinText;

    [Header("Win Effects")]
    [Tooltip("Drag your Fireworks Particle System here")]
    public ParticleSystem fireworksEffect;

    [Tooltip("Drag your Audio Source here")]
    public AudioSource celebrationAudio;

    [Header("Timer Settings")]
    public float timeRemaining = 120f;
    private bool isTimerRunning = true;

    [Header("Score Settings")]
    public int pointsPerCorrect = 100;
    public int pointsPerWrong = 50;
    public float wrongBinDisplayDuration = 2f;

    [Header("Stage 3 Custom Settings")]
    public Renderer cubeRenderer;

    private int currentScore = 0;
    private int totalAttempts = 0;
    private int correctSorts = 0;
    private Coroutine wrongBinCoroutine;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        if (cubeRenderer != null)
            cubeRenderer.material.color = Color.red;

        if (wrongBinPanel != null)
            wrongBinPanel.SetActive(false);

        UpdateTrashUI();
        UpdateScoreUI();
        UpdateAccuracyUI();
        UpdateTimerUI(timeRemaining);
    }

    private void Update()
    {
        if (!isTimerRunning)
            return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            UpdateTimerUI(timeRemaining);
        }
        else
        {
            timeRemaining = 0;
            isTimerRunning = false;
            UpdateTimerUI(0);
            GameOverTimeOut();
        }
    }

    public void TrySortTrash(TrashType binType, TrashType itemType)
    {
        totalAttempts++;

        if (binType == itemType)
        {
            correctSorts++;
            currentScore += pointsPerCorrect;
            TrashCollected();
        }
        else
        {
            currentScore = Mathf.Max(0, currentScore - pointsPerWrong);
            ShowWrongBinFeedback(itemType);
        }

        UpdateScoreUI();
        UpdateAccuracyUI();
    }

    public void TrashCollected()
    {
        collectedTrash++;
        UpdateTrashUI();

        if (AllTrashCollected())
        {
            isTimerRunning = false;

            if (cubeRenderer != null)
                cubeRenderer.material.color = Color.green;

            StageComplete();
        }
    }

    private void StageComplete()
    {
        Debug.Log("[TrashManager] Stage Complete!");

        // Play fireworks
        if (fireworksEffect != null)
        {
            fireworksEffect.Play();
        }

        // Play celebration sound
        if (celebrationAudio != null)
        {
            celebrationAudio.Play();
        }
    }

    private void GameOverTimeOut()
    {
        Debug.Log("[TrashManager] Time's Up!");
    }

    private void UpdateTrashUI()
    {
        if (trashCountText != null)
        {
            trashCountText.text = collectedTrash + " / " + totalTrash;
        }
    }

    private void UpdateTimerUI(float time)
    {
        if (timerText == null)
            return;

        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString();
        }
    }

    private void UpdateAccuracyUI()
    {
        if (accuracyText == null)
            return;

        float percentage =
            totalAttempts == 0
            ? 100f
            : (correctSorts / (float)totalAttempts) * 100f;

        accuracyText.text = Mathf.RoundToInt(percentage) + "%";
    }

    private void ShowWrongBinFeedback(TrashType itemType)
    {
        if (wrongBinCoroutine != null)
            StopCoroutine(wrongBinCoroutine);

        wrongBinCoroutine = StartCoroutine(ShowFeedbackForDuration());
    }

    private IEnumerator ShowFeedbackForDuration()
    {
        if (wrongBinPanel != null)
            wrongBinPanel.SetActive(true);

        yield return new WaitForSeconds(wrongBinDisplayDuration);

        if (wrongBinPanel != null)
            wrongBinPanel.SetActive(false);
    }

    public bool AllTrashCollected()
    {
        return collectedTrash >= totalTrash;
    }

    public int GetScore()
    {
        return currentScore;
    }

    public float GetAccuracy()
    {
        return totalAttempts == 0
            ? 100f
            : (correctSorts / (float)totalAttempts) * 100f;
    }
}