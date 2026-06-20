using UnityEngine;

public class TrashManager : MonoBehaviour
{
    public static TrashManager Instance;

    [Header("Trash Settings")]
    public int totalTrash = 5;

    // Made public so your other scripts can read the score easily
    public int collectedTrash = 0;

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

    public void TrashCollected()
    {
        collectedTrash++;
        Debug.Log("Trash Collected: " + collectedTrash + "/" + totalTrash);
    }

    public bool AllTrashCollected()
    {
        return collectedTrash >= totalTrash;
    }
}