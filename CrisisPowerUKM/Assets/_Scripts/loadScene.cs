using UnityEngine;
using UnityEngine.SceneManagement;

public class VRSceneLoader : MonoBehaviour
{
    [Header("Scene Configuration")]
    public string nextSceneName = "Scene2"; // Masukkan nama Scene 2 kamu di sini

    private bool canLoadScene = false;

    void Start()
    {
        // Pastikan fungsi interaksi dikunci pada awal game
        canLoadScene = false;
    }

    // Fungsi ini AKAN DIPANGGIL oleh script sampah apabila sampah selamat masuk beg
    public void AktifkanPintu()
    {
        canLoadScene = true; // <-- DISINI TADI TYPO. Dah dibetulkan ke canLoadScene

        Debug.Log("Pintu aktif! Pemain kini boleh menyentuh pintu untuk ke Scene seterusnya.");
    }

    // Fungsi utama untuk tukar scene yang akan ditarik ke dalam Event VR Controller
    public void LoadNextScene()
    {
        // Hanya benarkan tukar scene JIKA sampah sudah selesai dikutip (canLoadScene == true)
        if (canLoadScene)
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.Log("Sampah belum habis dikutip! Pintu masih terkunci.");
        }
    }
}