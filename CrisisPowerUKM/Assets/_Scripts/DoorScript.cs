//using UnityEngine;
//using UnityEngine.SceneManagement;
//using System.Collections;

//public class DoorScript : MonoBehaviour
//{
//    public Renderer doorRenderer;
//    public string nextSceneName = "aliff_Stage2";

//    [Header("Door Type Settings")]
//    public bool isAlwaysUnlocked = false;

//    [Header("Spawn Settings")]
//    [Tooltip("Check this TRUE for the door in the scene you are ENTERING so it moves the local player.")]
//    public bool alihkanPemainPadaMula = false;

//    [Tooltip("Type the exact name of the SpawnPoint GameObject located in THIS scene.")]
//    public string spawnPointName = "SpawnPoint_DariStage1";

//    private bool unlocked = false;

//    void Start()
//    {
//        // Setup initial door colors
//        if (isAlwaysUnlocked)
//        {
//            unlocked = true;
//            if (doorRenderer != null) doorRenderer.material.color = Color.white;
//        }
//        else
//        {
//            if (doorRenderer != null) doorRenderer.material.color = Color.red;
//        }

//        // Wait a small moment for XR Rig simulation configurations to settle before moving
//        if (alihkanPemainPadaMula)
//        {
//            StartCoroutine(WaitAndMovePlayer());
//        }
//    }

//    IEnumerator WaitAndMovePlayer()
//    {
//        yield return new WaitForSeconds(0.1f);
//        AlihkanPemainKeSpawnPoint();
//    }

//    void AlihkanPemainKeSpawnPoint()
//    {
//        // Finds the specific spawn point configured for THIS scene
//        GameObject spawnPoint = GameObject.Find(spawnPointName);
//        GameObject player = GameObject.FindWithTag("Player");
//        if (player == null) player = GameObject.Find("XR Origin (VR)");
//        if (player == null) player = GameObject.Find("XR Origin (XR Rig)");

//        if (spawnPoint != null && player != null)
//        {
//            CharacterController cc = player.GetComponentInChildren<CharacterController>();
//            if (cc != null) cc.enabled = false;

//            // 1. Move the root player object
//            player.transform.position = spawnPoint.transform.position;
//            player.transform.rotation = spawnPoint.transform.rotation;

//            // 2. XR Simulator Tracking Fix
//            Transform cameraOffset = player.transform.Find("Camera Offset");
//            if (cameraOffset != null)
//            {
//                cameraOffset.localPosition = Vector3.zero;
//            }

//            if (cc != null) cc.enabled = true;
//            Debug.Log("Successfully moved player to spawn point: " + spawnPointName);
//        }
//        else
//        {
//            if (spawnPoint == null) Debug.LogWarning("Could not find Spawn Point GameObject named: " + spawnPointName);
//            if (player == null) Debug.LogWarning("Could not find XR Origin player object!");
//        }
//    }

//    void Update()
//    {
//        if (isAlwaysUnlocked) return;

//        if (!unlocked && TrashManager.Instance != null && TrashManager.Instance.AllTrashCollected())
//        {
//            unlocked = true;
//            if (doorRenderer != null) doorRenderer.material.color = Color.white;
//        }
//    }

//    public void DoorClicked()
//    {
//        if (unlocked)
//        {
//            SceneManager.LoadScene(nextSceneName);
//        }
//    }
//}

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class DoorScript : MonoBehaviour
{
    public Renderer doorRenderer;
    public string nextSceneName = "aliff_Stage2";

    [Header("Door Type Settings")]
    public bool isAlwaysUnlocked = false;

    [Header("Spawn Settings")]
    [Tooltip("Check TRUE for the door in the scene you are ENTERING so it moves the local player.")]
    public bool alihkanPemainPadaMula = false;

    [Tooltip("Exact name of the SpawnPoint GameObject in THIS scene if returning from another stage.")]
    public string spawnPointName = "SpawnPoint_DariStage2";

    [Tooltip("Exact name of the SpawnPoint used ONLY when starting fresh from MainMenu.")]
    public string mainMenuSpawnPointName = "SpawnPoint_MulaGame";

    // Menyimpan rekod nama scene sebelum ini secara global
    public static string previousSceneName = "";

    private bool unlocked = false;

    void Start()
    {
        if (isAlwaysUnlocked)
        {
            unlocked = true;
            if (doorRenderer != null) doorRenderer.material.color = Color.white;
        }
        else
        {
            if (doorRenderer != null) doorRenderer.material.color = Color.red;
        }

        if (alihkanPemainPadaMula)
            StartCoroutine(WaitAndMovePlayer());
    }

    IEnumerator WaitAndMovePlayer()
    {
        yield return new WaitForSeconds(0.1f);
        AlihkanPemainKeSpawnPoint();
    }

    void AlihkanPemainKeSpawnPoint()
    {
        // LOGIK DINAMIK: Jika datang dari MainMenu (atau nama scene sebelum ini kosong),
        // gunakan Main Menu Spawn Point. Jika tidak, gunakan spawn point biasa (Dari Stage 2).
        string targetSpawnName = spawnPointName;

        if (string.IsNullOrEmpty(previousSceneName) || previousSceneName == "MainMenu1")
        {
            targetSpawnName = mainMenuSpawnPointName;
            Debug.Log("[DoorScript] Pemain dikesan datang dari MainMenu. Menggunakan: " + targetSpawnName);
        }
        else
        {
            Debug.Log("[DoorScript] Pemain datang dari scene: " + previousSceneName + ". Menggunakan: " + targetSpawnName);
        }

        GameObject spawnPoint = GameObject.Find(targetSpawnName);
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) player = GameObject.Find("XR Origin (VR)");
        if (player == null) player = GameObject.Find("XR Origin (XR Rig)");

        if (spawnPoint != null && player != null)
        {
            CharacterController cc = player.GetComponentInChildren<CharacterController>();
            if (cc != null) cc.enabled = false;

            player.transform.position = spawnPoint.transform.position;
            player.transform.rotation = spawnPoint.transform.rotation;

            Transform cameraOffset = player.transform.Find("Camera Offset");
            if (cameraOffset != null) cameraOffset.localPosition = Vector3.zero;

            if (cc != null) cc.enabled = true;
            Debug.Log("Moved player to: " + targetSpawnName);
        }
        else
        {
            if (spawnPoint == null) Debug.LogWarning("Cannot find SpawnPoint: " + targetSpawnName);
            if (player == null) Debug.LogWarning("Cannot find player object!");
        }
    }

    void Update()
    {
        if (isAlwaysUnlocked) return;

        if (!unlocked && TrashManager.Instance != null && TrashManager.Instance.AllTrashCollected())
        {
            unlocked = true;
            if (doorRenderer != null) doorRenderer.material.color = Color.green;
            Debug.Log("[DoorScript] Door unlocked!");
        }
    }

    public void DoorClicked()
    {
        if (!unlocked)
        {
            Debug.Log("[DoorScript] Door is still locked — collect all trash first!");
            return;
        }

        // SIMPAN NAMA SCENE SEMASA SEBELUM BERPINDAH
        previousSceneName = SceneManager.GetActiveScene().name;

        if (StageManager.Instance != null)
            StageManager.Instance.CompleteCurrentStage(nextSceneName);
        else
            SceneManager.LoadScene(nextSceneName);
    }
}