using UnityEngine;

public class TrashManager : MonoBehaviour
{
    public static TrashManager Instance;

    [Header("Trash Settings")]
    public int totalTrash = 5;

    // Made public so your other scripts can read the score easily
    public int collectedTrash = 0;

    [Header("Stage 3 Custom Settings")]
    [Tooltip("Drag your custom cube box here in Stage 3.")]
    public Renderer cubeRenderer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Optional: Turn the cube red on start if it exists in the scene
        if (cubeRenderer != null)
        {
            cubeRenderer.material.color = Color.red;
        }
    }

    public void TrashCollected()
    {
        collectedTrash++;
        Debug.Log("Trash Collected: " + collectedTrash + "/" + totalTrash);

        // --- NEW BOX COLOR CHECK ---
        // If all trash is collected and you assigned a cube box, turn it green!
        if (AllTrashCollected() && cubeRenderer != null)
        {
            cubeRenderer.material.color = Color.green;
            Debug.Log("Stage 3 Box turned GREEN!");
        }
    }

    public bool AllTrashCollected()
    {
        return collectedTrash >= totalTrash;
    }
}