using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorScript : MonoBehaviour
{
    public Renderer doorRenderer;
    public string nextSceneName = "Stage2";

    [Header("Door Type Settings")]
    public bool isAlwaysUnlocked = false;

    private bool unlocked = false;

    void Start()
    {
        if (isAlwaysUnlocked)
        {
            unlocked = true;
            if (doorRenderer != null)
            {
                doorRenderer.material.color = Color.white;
            }

            // --- KOD TAMBAHAN: Ubah posisi pemain ke pintu sebaik sahaja scene bermula ---
            AlihkanPemainKeSpawnPoint();
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
        // Cari objek penanda posisi yang kita buat di Stage 2 tadi
        GameObject spawnPoint = GameObject.Find("SpawnPoint_DariStage1");
        // Cari watak VR pemain kamu di dalam scene
        GameObject player = GameObject.FindWithTag("Player") ?? GameObject.Find("XR Origin (XR Rig)");

        if (spawnPoint != null && player != null)
        {
            // Ubah posisi dan pusingan watak mengikut penanda pintu
            player.transform.position = spawnPoint.transform.position;
            player.transform.rotation = spawnPoint.transform.rotation;
            Debug.Log("Watak berjaya di-spawn di hadapan pintu Stage 2!");
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