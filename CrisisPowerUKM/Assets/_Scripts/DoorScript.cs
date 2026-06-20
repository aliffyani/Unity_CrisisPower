using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorScript : MonoBehaviour
{
    public Renderer doorRenderer;
    public string nextSceneName = "Stage2";

    [Header("Door Type Settings")]
    public bool isAlwaysUnlocked = false;

    [Header("Spawn Settings")]
    [Tooltip("Check this true ONLY for the door inside Stage 2 to reposition the player on arrival.")]
    public bool alihkanPemainPadaMula = false;

    private bool unlocked = false;

    void Start()
    {
        // Automatically teleports the player if this door is flagged as the arrival door
        if (alihkanPemainPadaMula)
        {
            AlihkanPemainKeSpawnPoint();
        }

        if (isAlwaysUnlocked)
        {
            unlocked = true;
            if (doorRenderer != null)
            {
                doorRenderer.material.color = Color.white;
            }
        }
        else
        {
            if (doorRenderer != null)
            {
                doorRenderer.material.color = Color.red;
            }
        }
    }

    void AlihkanPemainKeSpawnPoint()
    {
        // Finds your marker object in Stage 2
        GameObject spawnPoint = GameObject.Find("SpawnPoint_DariStage1");

        // Finds your VR Rig. (Checks for the tag 'Player' first, then falls back to Unity XR name defaults)
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) player = GameObject.Find("XR Origin (VR)");
        if (player == null) player = GameObject.Find("XR Origin (XR Rig)");

        if (spawnPoint != null && player != null)
        {
            // Move player to the designated spot
            player.transform.position = spawnPoint.transform.position;
            player.transform.rotation = spawnPoint.transform.rotation;
            Debug.Log("Pemain berjaya di-spawn di hadapan pintu baru!");
        }
        else
        {
            if (spawnPoint == null) Debug.LogWarning("Missing 'SpawnPoint_DariStage1' object in scene!");
            if (player == null) Debug.LogWarning("Could not find XR Origin player object!");
        }
    }

    void Update()
    {
        if (isAlwaysUnlocked) return;

        if (!unlocked && TrashManager.Instance != null && TrashManager.Instance.AllTrashCollected())
        {
            unlocked = true;
            if (doorRenderer != null)
            {
                doorRenderer.material.color = Color.white;
            }
        }
    }

    public void DoorClicked()
    {
        if (unlocked)
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}