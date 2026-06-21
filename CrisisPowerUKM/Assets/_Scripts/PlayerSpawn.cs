using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public Transform spawnPoint;
    public GameObject xrOrigin;

    void Start()
    {
        xrOrigin.transform.position = spawnPoint.position;
        xrOrigin.transform.rotation = spawnPoint.rotation;
    }
}