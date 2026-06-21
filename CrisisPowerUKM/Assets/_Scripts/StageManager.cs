using UnityEngine;
using UnityEngine.SceneManagement;

// Place this on a GameObject in your FIRST scene (Stage Select or Stage 1).
// It survives all scene loads via DontDestroyOnLoad.
public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    [Header("Scene Names — must match Build Settings exactly")]
    public string stageSelectScene = "StageSelect";
    public string stage1Scene = "Stage1";
    public string stage2Scene = "Stage2";
    public string stage3Scene = "Stage3";

    private int highestUnlockedStage = 1;
    private int currentStage = 1;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadProgress();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ── Called by StageSelectButton ───────────────────────────────
    public void PlayStage(int stage)
    {
        if (!IsStageUnlocked(stage))
        {
            Debug.Log("[StageManager] Stage " + stage + " is locked!");
            return;
        }

        currentStage = stage;
        Debug.Log("[StageManager] Playing stage " + stage);

        switch (stage)
        {
            case 1: SceneManager.LoadScene(stage1Scene); break;
            case 2: SceneManager.LoadScene(stage2Scene); break;
            case 3: SceneManager.LoadScene(stage3Scene); break;
        }
    }

    // ── Called by DoorScript and VRSceneLoader when door is used ──
    // Unlocks the next stage then loads whatever scene the door points to
    public void CompleteCurrentStage(string nextSceneName)
    {
        Debug.Log("[StageManager] Stage " + currentStage + " complete!");

        // Unlock next stage if there is one
        if (currentStage < 3 && currentStage + 1 > highestUnlockedStage)
        {
            highestUnlockedStage = currentStage + 1;
            SaveProgress();
            Debug.Log("[StageManager] Stage " + highestUnlockedStage + " unlocked!");
        }

        // Load whatever scene the door says (could be next stage or a cutscene)
        SceneManager.LoadScene(nextSceneName);
    }

    // ── Called when returning to Stage Select after a win ─────────
    public void GoToStageSelect()
    {
        SceneManager.LoadScene(stageSelectScene);
    }

    public bool IsStageUnlocked(int stage) => stage <= highestUnlockedStage;
    public int GetHighestUnlocked() => highestUnlockedStage;
    public int GetCurrentStage() => currentStage;

    private void SaveProgress()
    {
        PlayerPrefs.SetInt("HighestUnlockedStage", highestUnlockedStage);
        PlayerPrefs.Save();
    }

    private void LoadProgress()
    {
        highestUnlockedStage = PlayerPrefs.GetInt("HighestUnlockedStage", 1);
        Debug.Log("[StageManager] Progress loaded — highest unlocked: Stage " + highestUnlockedStage);
    }

    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey("HighestUnlockedStage");
        highestUnlockedStage = 1;
        currentStage = 1;
        Debug.Log("[StageManager] Progress reset.");
    }
}