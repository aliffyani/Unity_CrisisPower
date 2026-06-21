using UnityEngine;
using UnityEngine.SceneManagement;

public class VRSceneLoader : MonoBehaviour
{
    [Header("Scene Configuration")]
    public string nextSceneName = "Scene2";

    private bool canLoadScene = false;

    void Start()
    {
        canLoadScene = false;

        // Also auto-activate if trash is already all collected when scene starts
        if (TrashManager.Instance != null && TrashManager.Instance.AllTrashCollected())
            AktifkanPintu();
    }

    // Called by trash script when all trash is collected
    public void AktifkanPintu()
    {
        canLoadScene = true;
        Debug.Log("[VRSceneLoader] Door activated! Player can now proceed.");
    }

    // Hook this into your VR controller interaction event
    public void LoadNextScene()
    {
        if (!canLoadScene)
        {
            Debug.Log("[VRSceneLoader] Sampah belum habis dikutip! Pintu masih terkunci.");
            return;
        }

        // Tell StageManager the stage is done before loading next scene
        if (StageManager.Instance != null)
            StageManager.Instance.CompleteCurrentStage(nextSceneName);
        else
            SceneManager.LoadScene(nextSceneName); // fallback if no StageManager
    }
}