using System.Collections;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement; // Diperlukan untuk fungsi restart scene

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

    [Header("Win Panel Settings")]
    [Tooltip("Tarik Canvas/Panel kemenangan anda ke sini. Ia akan aktif pada posisi asalnya.")]
    public GameObject winPanel;

    [Header("Game Over / Retry Settings")]
    [Tooltip("Tarik Canvas/Panel kekalahan (Retry Panel) anda ke sini.")]
    public GameObject retryPanel;
    [Tooltip("Jarak panel Retry muncul di hadapan muka user (meter)")]
    public float panelDistanceFromUser = 1.5f;
    [Tooltip("Larasan tinggi/rendah panel dari paras mata user")]
    public float panelHeightOffset = -0.2f;

    [Header("Trophy Settings (Static)")]
    [Tooltip("Tarik objek Trofi yang ada di dalam Hierarchy ke sini")]
    public GameObject staticTrophy;

    [Header("Player Teleport / Spawn Settings")]
    [Tooltip("Tarik objek XR Origin / XR Rig (Pemain) anda ke sini")]
    public GameObject playerRig;
    [Tooltip("Buat satu Empty GameObject di Scene sebagai penanda lokasi baharu, dan tarik ke sini")]
    public Transform newSpawnPoint;

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
    private Transform mainCameraTransform;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // Cari komponen kamera utama VR secara automatik
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }

        if (cubeRenderer != null)
            cubeRenderer.material.color = Color.red;

        if (wrongBinPanel != null)
            wrongBinPanel.SetActive(false);

        // Pastikan Win Panel tertutup pada awal permainan
        if (winPanel != null)
            winPanel.SetActive(false);

        // Pastikan Retry Panel tertutup pada awal permainan
        if (retryPanel != null)
            retryPanel.SetActive(false);

        // Pastikan Trofi tersembunyi pada awal permainan
        if (staticTrophy != null)
            staticTrophy.SetActive(false);

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

        // 1. Teleportasi pemain ke lokasi spawn yang baharu
        TeleportPlayerToNewSpawn();

        // 2. Aktifkan Win Panel pada posisi statik asal
        if (winPanel != null)
        {
            winPanel.SetActive(true);
        }

        // 3. Aktifkan Trofi pada posisi asal yang anda susun di scene
        if (staticTrophy != null)
        {
            staticTrophy.SetActive(true);
            Debug.Log("[TrashManager] Trofi diaktifkan pada posisi asalnya.");
        }

        // Mainkan kesan bunga api
        if (fireworksEffect != null)
        {
            fireworksEffect.Play();
        }

        // Mainkan bunyi sambutan
        if (celebrationAudio != null)
        {
            celebrationAudio.Play();
        }
    }

    private void TeleportPlayerToNewSpawn()
    {
        if (playerRig != null && newSpawnPoint != null)
        {
            playerRig.transform.position = newSpawnPoint.position;
            playerRig.transform.rotation = newSpawnPoint.rotation;

            Debug.Log("[TrashManager] Player berjaya di-spawn semula pada kedudukan baharu!");
        }
        else
        {
            Debug.LogWarning("[TrashManager] Rujukan Player Rig atau New Spawn Point hilang di Inspector!");
        }
    }

    private void GameOverTimeOut()
    {
        Debug.Log("[TrashManager] Time's Up!");

        if (retryPanel != null)
        {
            // Paksa kedudukan retryPanel pergi tepat ke hadapan muka pemain sebelum diaktifkan
            PositionPanelInFrontOfUser(retryPanel);

            retryPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("[TrashManager] Sila masukkan objek Retry Panel ke dalam Inspector!");
        }
    }

    private void PositionPanelInFrontOfUser(GameObject panel)
    {
        // Cuba dapatkan semula kamera jika rujukan kosong
        if (mainCameraTransform == null && Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }

        if (mainCameraTransform != null)
        {
            Vector3 cameraPos = mainCameraTransform.position;
            Vector3 forwardDirection = mainCameraTransform.forward;

            // Pastikan panel tegak lurus (abaikan dongakan kepala ke atas/bawah)
            forwardDirection.y = 0;
            forwardDirection.Normalize();

            // Kira kedudukan baharu mengikut parameter yang ditetapkan
            Vector3 targetPosition = cameraPos + (forwardDirection * panelDistanceFromUser);
            targetPosition.y += panelHeightOffset;

            panel.transform.position = targetPosition;

            // Paksa panel menghadap muka pemain tanpa senget
            panel.transform.LookAt(new Vector3(cameraPos.x, panel.transform.position.y, cameraPos.z));
            panel.transform.Rotate(0, 180, 0); // Pusing semula 180 darjah supaya text UI tidak terbalik

            Debug.Log("[TrashManager] Retry Panel berjaya dilaras di hadapan muka VR pemain.");
        }
    }

    // --- FUNGSI KLIK BUTANG (BUTTON CLICK FUNCTION) ---
    [ContextMenu("Restart Stage")]
    public void RestartStage()
    {
        Debug.Log("[TrashManager] Memuatkan semula stage...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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