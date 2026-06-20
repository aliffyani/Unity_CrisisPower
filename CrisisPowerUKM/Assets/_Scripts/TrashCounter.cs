using UnityEngine;
using TMPro; // Comment this out if you are using Unity's old legacy UI Text

public class TrashCounter : MonoBehaviour
{
    public TextMeshProUGUI counterText; // Drag your UI Text element here

    void Update()
    {
        if (TrashManager.Instance != null && counterText != null)
        {
            // Displays "Trash: 0 / 5" on your screen dynamically
            counterText.text = "Trash: " + TrashManager.Instance.collectedTrash + " / " + TrashManager.Instance.totalTrash;
        }
    }
}