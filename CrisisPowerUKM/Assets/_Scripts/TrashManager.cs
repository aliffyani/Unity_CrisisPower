using UnityEngine;
using TMPro; // Supported for TextMeshPro UI

public class TrashManager : MonoBehaviour
{
    public static TrashManager Instance;

    [Header("Trash Settings")]
    public int totalTrash = 5;
    public int collectedTrash = 0;

    [Header("UI Settings")]
    [Tooltip("Drag your TextMeshPro UI text for remaining trash here.")]
    public TextMeshProUGUI trashRemainingText;

    [Tooltip("Drag your TextMeshPro UI text for the timer here.")]
    public TextMeshProUGUI timerText; // Added Timer UI slot

    [Header("Timer Settings")]
    [Tooltip("Time limit in seconds (e.g., 120 for 2 minutes).")]
    public float timeRemaining = 120f;
    private bool isTimerRunning = true;

    [Header("Stage 3 Custom Settings")]
    [Tooltip("Drag your custom cube box here in Stage 3.")]
    public Renderer cubeRenderer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (cubeRenderer != null)
        {
            cubeRenderer.material.color = Color.red;
        }

        UpdateTrashUI();
    }

    private void Update()
    {
        if (isTimerRunning)
        {
            if (timeRemaining > 0)
            {
                // Subtract the time spent during the last frame
                timeRemaining -= Time.deltaTime;
                UpdateTimerUI(timeRemaining);
            }
            else
            {
                timeRemaining = 0;
                isTimerRunning = false;
                UpdateTimerUI(timeRemaining);
                GameOverTimeOut();
            }
        }
    }

    public void TrashCollected()
    {
        collectedTrash++;
        Debug.Log("Trash Collected: " + collectedTrash + "/" + totalTrash);

        UpdateTrashUI();

        if (AllTrashCollected())
        {
            isTimerRunning = false; // Stop the timer when they win!

            if (cubeRenderer != null)
            {
                cubeRenderer.material.color = Color.green;
                Debug.Log("Stage 3 Box turned GREEN!");
            }
        }
    }

    private void UpdateTrashUI()
    {
        if (trashRemainingText != null)
        {
            int trashLeft = totalTrash - collectedTrash;
            trashRemainingText.text = "Trash Left: " + trashLeft;
        }
    }

    private void UpdateTimerUI(float timeToDisplay)
    {
        if (timerText != null)
        {
            // Floor the values to avoid floating-point decimals on screen
            float minutes = Mathf.FloorToInt(timeToDisplay / 60);
            float seconds = Mathf.FloorToInt(timeToDisplay % 60);

            // Formats it to look like a clean digital clock "02:05" instead of "2:5"
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    private void GameOverTimeOut()
    {
        Debug.Log("Time ran out! Game Over!");
        // Add your lose-condition logic here (like triggering a game over screen panel)
    }

    public bool AllTrashCollected()
    {
        return collectedTrash >= totalTrash;
    }
}