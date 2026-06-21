//using UnityEngine;
//using TMPro; // Supported for TextMeshPro UI

//public class TrashManager : MonoBehaviour
//{
//    public static TrashManager Instance;

//    [Header("Trash Settings")]
//    public int totalTrash = 5;
//    public int collectedTrash = 0;

//    [Header("UI Settings")]
//    [Tooltip("Drag your TextMeshPro UI text for remaining trash here.")]
//    public TextMeshProUGUI trashRemainingText;

//    [Tooltip("Drag your TextMeshPro UI text for the timer here.")]
//    public TextMeshProUGUI timerText; // Added Timer UI slot

//    [Header("Timer Settings")]
//    [Tooltip("Time limit in seconds (e.g., 120 for 2 minutes).")]
//    public float timeRemaining = 120f;
//    private bool isTimerRunning = true;

//    [Header("Stage 3 Custom Settings")]
//    [Tooltip("Drag your custom cube box here in Stage 3.")]
//    public Renderer cubeRenderer;

//    private void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    private void Start()
//    {
//        if (cubeRenderer != null)
//        {
//            cubeRenderer.material.color = Color.red;
//        }

//        UpdateTrashUI();
//    }

//    private void Update()
//    {
//        if (isTimerRunning)
//        {
//            if (timeRemaining > 0)
//            {
//                // Subtract the time spent during the last frame
//                timeRemaining -= Time.deltaTime;
//                UpdateTimerUI(timeRemaining);
//            }
//            else
//            {
//                timeRemaining = 0;
//                isTimerRunning = false;
//                UpdateTimerUI(timeRemaining);
//                GameOverTimeOut();
//            }
//        }
//    }

//    public void TrashCollected()
//    {
//        collectedTrash++;
//        Debug.Log("Trash Collected: " + collectedTrash + "/" + totalTrash);

//        UpdateTrashUI();

//        if (AllTrashCollected())
//        {
//            isTimerRunning = false; // Stop the timer when they win!

//            if (cubeRenderer != null)
//            {
//                cubeRenderer.material.color = Color.green;
//                Debug.Log("Stage 3 Box turned GREEN!");
//            }
//        }
//    }

//    private void UpdateTrashUI()
//    {
//        if (trashRemainingText != null)
//        {
//            int trashLeft = totalTrash - collectedTrash;
//            trashRemainingText.text = "Trash Left: " + trashLeft;
//        }
//    }

//    private void UpdateTimerUI(float timeToDisplay)
//    {
//        if (timerText != null)
//        {
//            // Floor the values to avoid floating-point decimals on screen
//            float minutes = Mathf.FloorToInt(timeToDisplay / 60);
//            float seconds = Mathf.FloorToInt(timeToDisplay % 60);

//            // Formats it to look like a clean digital clock "02:05" instead of "2:5"
//            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
//        }
//    }

//    private void GameOverTimeOut()
//    {
//        Debug.Log("Time ran out! Game Over!");
//        // Add your lose-condition logic here (like triggering a game over screen panel)
//    }

//    public bool AllTrashCollected()
//    {
//        return collectedTrash >= totalTrash;
//    }
//}

//using System.Collections;
//using UnityEngine;
//using TMPro;

//public class TrashManager : MonoBehaviour
//{
//    public static TrashManager Instance;

//    // ── Bin types matching your 3 physical bins ───────────────────
//    public enum BinType
//    {
//        BlueBin,    // Paper: newspaper, magazines, cardboard
//        BrownBin,   // Glass: bottles, jars
//        OrangeBin   // Plastics & Metals: beverage cans, food tins, plastic bottles
//    }

//    [Header("Trash Settings")]
//    public int totalTrash = 5;
//    public int collectedTrash = 0;

//    [Header("UI References")]
//    [Tooltip("TMP text showing trash left e.g. '8 / 20'")]
//    public TextMeshProUGUI trashRemainingText;
//    [Tooltip("TMP text showing countdown timer e.g. '02:34'")]
//    public TextMeshProUGUI timerText;
//    [Tooltip("TMP text showing current score")]
//    public TextMeshProUGUI scoreText;
//    [Tooltip("TMP text showing sort accuracy percentage")]
//    public TextMeshProUGUI accuracyText;
//    [Tooltip("The entire wrong-bin feedback panel GameObject")]
//    public GameObject wrongBinPanel;
//    [Tooltip("TMP text inside the wrong-bin feedback panel")]
//    public TextMeshProUGUI wrongBinText;

//    [Header("Timer Settings")]
//    [Tooltip("Time limit in seconds (120 = 2 minutes)")]
//    public float timeRemaining = 120f;
//    private bool isTimerRunning = true;

//    [Header("Score Settings")]
//    [Tooltip("Points awarded for a correct sort")]
//    public int pointsPerCorrect = 100;
//    [Tooltip("Points deducted for dropping in the wrong bin")]
//    public int pointsPerWrong = 50;
//    [Tooltip("How long the wrong-bin feedback panel stays on screen")]
//    public float wrongBinDisplayDuration = 2f;

//    [Header("Stage 3 Custom Settings")]
//    [Tooltip("Drag your Stage 3 cube Renderer here")]
//    public Renderer cubeRenderer;

//    // ── Private state ─────────────────────────────────────────────
//    private int currentScore = 0;
//    private int totalAttempts = 0;
//    private int correctSorts = 0;
//    private Coroutine wrongBinCoroutine;

//    // ─────────────────────────────────────────────────────────────
//    private void Awake()
//    {
//        if (Instance == null)
//            Instance = this;
//        else
//            Destroy(gameObject);
//    }

//    private void Start()
//    {
//        if (cubeRenderer != null)
//            cubeRenderer.material.color = Color.red;

//        if (wrongBinPanel != null)
//            wrongBinPanel.SetActive(false);

//        UpdateTrashUI();
//        UpdateScoreUI();
//        UpdateAccuracyUI();
//    }

//    private void Update()
//    {
//        if (!isTimerRunning) return;

//        if (timeRemaining > 0)
//        {
//            timeRemaining -= Time.deltaTime;
//            UpdateTimerUI(timeRemaining);
//        }
//        else
//        {
//            timeRemaining = 0;
//            isTimerRunning = false;
//            UpdateTimerUI(0);
//            GameOverTimeOut();
//        }
//    }

//    // ─────────────────────────────────────────────────────────────
//    // Call this from your Bin script when a trash item is dropped.
//    //
//    //   binUsed    = which bin the player dropped it into
//    //   correctBin = the bin that trash item actually belongs in
//    //
//    // Example (in your Bin.cs OnTriggerEnter):
//    //   TrashManager.Instance.TrySortTrash(BinType.BlueBin, droppedItem.correctBin);
//    // ─────────────────────────────────────────────────────────────
//    public void TrySortTrash(BinType binUsed, BinType correctBin)
//    {
//        totalAttempts++;

//        if (binUsed == correctBin)
//        {
//            correctSorts++;
//            currentScore += pointsPerCorrect;
//            TrashCollected();
//        }
//        else
//        {
//            currentScore = Mathf.Max(0, currentScore - pointsPerWrong);
//            ShowWrongBinFeedback(correctBin);
//        }

//        UpdateScoreUI();
//        UpdateAccuracyUI();
//    }

//    // Call this directly if you handle collision per-bin and just
//    // need to register a confirmed correct sort.
//    public void TrashCollected()
//    {
//        collectedTrash++;
//        Debug.Log("Trash collected: " + collectedTrash + "/" + totalTrash);
//        UpdateTrashUI();

//        if (AllTrashCollected())
//        {
//            isTimerRunning = false;

//            if (cubeRenderer != null)
//            {
//                cubeRenderer.material.color = Color.green;
//                Debug.Log("Stage 3 Box turned GREEN!");
//            }

//            StageComplete();
//        }
//    }

//    // ── UI helpers ────────────────────────────────────────────────
//    private void UpdateTrashUI()
//    {
//        if (trashRemainingText != null)
//            trashRemainingText.text = (totalTrash - collectedTrash) + " / " + totalTrash;
//    }

//    private void UpdateTimerUI(float time)
//    {
//        if (timerText == null) return;
//        int minutes = Mathf.FloorToInt(time / 60);
//        int seconds = Mathf.FloorToInt(time % 60);
//        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
//        timerText.color = (time <= 30f) ? Color.red : Color.white;
//    }

//    private void UpdateScoreUI()
//    {
//        if (scoreText != null)
//            scoreText.text = currentScore.ToString("N0");
//    }

//    private void UpdateAccuracyUI()
//    {
//        if (accuracyText == null) return;
//        float pct = (totalAttempts == 0) ? 100f : (correctSorts / (float)totalAttempts) * 100f;
//        accuracyText.text = Mathf.RoundToInt(pct) + "%";
//        accuracyText.color = pct >= 70f ? Color.green : pct >= 40f ? Color.yellow : Color.red;
//    }

//    private void ShowWrongBinFeedback(BinType correctBin)
//    {
//        if (wrongBinPanel == null) return;

//        // Human-readable hint matching your exact bin colours and contents
//        string hint = correctBin switch
//        {
//            BinType.BlueBin => "Blue Bin — Paper\n(newspaper, magazines, cardboard)",
//            BinType.BrownBin => "Brown Bin — Glass\n(bottles, jars)",
//            BinType.OrangeBin => "Orange Bin — Plastics & Metals\n(cans, tins, plastic bottles)",
//            _ => correctBin.ToString()
//        };

//        if (wrongBinText != null)
//            wrongBinText.text = "Wrong bin!\nShould go in: " + hint;

//        if (wrongBinCoroutine != null)
//            StopCoroutine(wrongBinCoroutine);

//        wrongBinCoroutine = StartCoroutine(ShowFeedbackForDuration());
//    }

//    private IEnumerator ShowFeedbackForDuration()
//    {
//        wrongBinPanel.SetActive(true);
//        yield return new WaitForSeconds(wrongBinDisplayDuration);
//        wrongBinPanel.SetActive(false);
//        wrongBinCoroutine = null;
//    }

//    // ── Game state ────────────────────────────────────────────────
//    private void StageComplete()
//    {
//        Debug.Log("Stage complete! Score: " + currentScore + " | Accuracy: " + Mathf.RoundToInt(GetAccuracy()) + "%");
//        // TODO: show stage complete panel, save progress, unlock next stage
//    }

//    private void GameOverTimeOut()
//    {
//        Debug.Log("Time's up! Final score: " + currentScore);
//        // TODO: show game over screen
//    }

//    public bool AllTrashCollected() => collectedTrash >= totalTrash;
//    public int GetScore() => currentScore;
//    public float GetAccuracy() => (totalAttempts == 0) ? 100f : (correctSorts / (float)totalAttempts) * 100f;
//}

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
    [Tooltip("Shows collected count e.g. '0 / 2' up to '2 / 2'")]
    public TextMeshProUGUI trashCountText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI accuracyText;
    public GameObject wrongBinPanel;
    public TextMeshProUGUI wrongBinText;

    [Header("Timer Settings")]
    public float timeRemaining = 120f;
    private bool isTimerRunning = true;

    [Header("Score Settings")]
    public int pointsPerCorrect = 100;
    public int pointsPerWrong = 50;
    public float wrongBinDisplayDuration = 2f;

    [Header("Stage 3 Custom Settings")]
    public Renderer cubeRenderer;

    // private
    private int currentScore = 0;
    private int totalAttempts = 0;
    private int correctSorts = 0;
    private Coroutine wrongBinCoroutine;

    // ─────────────────────────────────────────────────────────────
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
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

        // Warn about any missing UI slots so you can spot them fast
        if (trashCountText == null) Debug.LogWarning("[TrashManager] trashCountText not assigned!");
        if (timerText == null) Debug.LogWarning("[TrashManager] timerText not assigned!");
        if (scoreText == null) Debug.LogWarning("[TrashManager] scoreText not assigned!");
        if (accuracyText == null) Debug.LogWarning("[TrashManager] accuracyText not assigned!");
        if (wrongBinPanel == null) Debug.LogWarning("[TrashManager] wrongBinPanel not assigned!");
        if (wrongBinText == null) Debug.LogWarning("[TrashManager] wrongBinText not assigned!");
    }

    private void Update()
    {
        if (!isTimerRunning) return;

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

    // ─────────────────────────────────────────────────────────────
    // Called by RecycleBin.cs automatically.
    //   binType  = what this bin accepts (set on the bin)
    //   itemType = what the dropped trash actually is (set on the prefab)
    // ─────────────────────────────────────────────────────────────
    public void TrySortTrash(TrashType binType, TrashType itemType)
    {
        totalAttempts++;
        Debug.Log("[TrashManager] TrySortTrash — bin accepts: " + binType + " | item is: " + itemType);

        if (binType == itemType)
        {
            correctSorts++;
            currentScore += pointsPerCorrect;
            collectedTrash++;
            Debug.Log("[TrashManager] Correct! Score=" + currentScore + " Collected=" + collectedTrash + "/" + totalTrash);

            UpdateTrashUI();

            if (collectedTrash >= totalTrash)
            {
                isTimerRunning = false;
                if (cubeRenderer != null) cubeRenderer.material.color = Color.green;
                StageComplete();
            }
        }
        else
        {
            currentScore = Mathf.Max(0, currentScore - pointsPerWrong);
            Debug.Log("[TrashManager] Wrong bin! Score=" + currentScore);
            ShowWrongBinFeedback(itemType);
        }

        UpdateScoreUI();
        UpdateAccuracyUI();
    }

    // ── UI updates ────────────────────────────────────────────────
    private void UpdateTrashUI()
    {
        if (trashCountText != null)
            trashCountText.text = collectedTrash + " / " + totalTrash;
    }

    private void UpdateTimerUI(float time)
    {
        if (timerText == null) return;
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        timerText.color = time <= 30f ? Color.red : Color.white;
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = currentScore.ToString();
    }

    private void UpdateAccuracyUI()
    {
        if (accuracyText == null) return;
        float pct = totalAttempts == 0 ? 100f : (correctSorts / (float)totalAttempts) * 100f;
        accuracyText.text = Mathf.RoundToInt(pct) + "%";
        accuracyText.color = pct >= 70f ? Color.green : pct >= 40f ? Color.yellow : Color.red;
    }

    // Shows which bin the item actually belongs in
    private void ShowWrongBinFeedback(TrashType itemType)
    {
        if (wrongBinPanel == null)
        {
            Debug.LogWarning("[TrashManager] wrongBinPanel not assigned — panel won't show!");
            return;
        }

        string hint = itemType switch
        {
            TrashType.Paper => "Blue Bin — Paper\n(newspaper, magazines, cardboard)",
            TrashType.Glass => "Brown Bin — Glass\n(bottles, jars)",
            TrashType.Plastic => "Orange Bin — Plastics & Metals\n(cans, tins, plastic bottles)",
            _ => itemType.ToString()
        };

        if (wrongBinText != null)
            wrongBinText.text = "Wrong bin!\nShould go in: " + hint;

        if (wrongBinCoroutine != null) StopCoroutine(wrongBinCoroutine);
        wrongBinCoroutine = StartCoroutine(ShowFeedbackForDuration());
    }

    private IEnumerator ShowFeedbackForDuration()
    {
        wrongBinPanel.SetActive(true);
        yield return new WaitForSeconds(wrongBinDisplayDuration);
        wrongBinPanel.SetActive(false);
        wrongBinCoroutine = null;

    }


    // ── Game state ────────────────────────────────────────────────
    private void StageComplete()
    {
        Debug.Log("[TrashManager] Stage complete! Score=" + currentScore + " Accuracy=" + Mathf.RoundToInt(GetAccuracy()) + "%");
        // TODO: show stage complete panel
    }

    private void GameOverTimeOut()
    {
        Debug.Log("[TrashManager] Time's up! Score=" + currentScore);
        // TODO: show game over panel
    }

    public bool AllTrashCollected() => collectedTrash >= totalTrash;
    public int GetScore() => currentScore;
    public float GetAccuracy() => totalAttempts == 0 ? 100f : (correctSorts / (float)totalAttempts) * 100f;

    internal void TrashCollected()
    {
        throw new NotImplementedException();
    }
}