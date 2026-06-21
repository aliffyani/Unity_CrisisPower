using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class VRMenuController : MonoBehaviour
{
    [Header("Panel Settings")]
    public GameObject htpInfoPanel; // Tarik objek HTPInfo ke sini

    [Header("Scene Settings")]
    public string firstStageSceneName = "aliff_Stage1";

    void Start()
    {
        // Pastikan panel HTPInfo tertutup pada awal permainan
        if (htpInfoPanel != null)
        {
            htpInfoPanel.SetActive(false);
        }
    }

    // Fungsi 1: Dipanggil oleh butang START MISSION
    public void ClickStartMission()
    {
        Debug.Log("Memuatkan Scene: " + firstStageSceneName);
        SceneManager.LoadScene(firstStageSceneName);
    }

    // Fungsi 2: Dipanggil oleh butang HOW TO PLAY
    public void ClickHowToPlay()
    {
        if (htpInfoPanel != null)
        {
            htpInfoPanel.SetActive(true);
            Debug.Log("Panel HTPInfo dibuka.");
        }
    }

    // Fungsi 3: Dipanggil oleh butang BACK
    public void ClickBackButton()
    {
        if (htpInfoPanel != null)
        {
            htpInfoPanel.SetActive(false);
            Debug.Log("Panel HTPInfo ditutup.");
        }
    }
}