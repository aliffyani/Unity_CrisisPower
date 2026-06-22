using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Audio Settings")]
    [Tooltip("Tarik objek Audio Source yang mengandungi lagu latar ke sini")]
    public AudioSource startMenuAudio;

    [Tooltip("Berapa saat nak tunggu selepas klik sebelum scene bertukar")]
    public float delayBeforeLoad = 0.2f;

    public void PlayGame()
    {
        Debug.Log("Butang Play diklik! Memastikan lagu terus bermain...");

        if (startMenuAudio != null)
        {
            // PENTING: Mengarahkan Unity supaya JANGAN padam objek audio ini apabila scene bertukar
            DontDestroyOnLoad(startMenuAudio.gameObject);

            // Mainkan lagu jika ia belum dimainkan
            if (!startMenuAudio.isPlaying)
            {
                startMenuAudio.Play();
            }
        }
        else
        {
            Debug.LogWarning("[MainMenuManager] Audio Source kosong!");
        }

        // Jalankan pertukaran scene dengan sedikit delay
        StartCoroutine(LoadSceneWithDelay());
    }

    private IEnumerator LoadSceneWithDelay()
    {
        yield return new WaitForSeconds(delayBeforeLoad);

        Debug.Log("Loading Level 1...");
        SceneManager.LoadScene("aliff_Stage1");
    }

    public void HowToPlay()
    {
        Debug.Log("How to Play diklik");
    }
}