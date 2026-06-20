using UnityEngine;

public class TrashBin : MonoBehaviour
{
    public TrashType acceptedType;

    private void OnTriggerEnter(Collider other)
    {
        TrashItem item = other.GetComponent<TrashItem>();

        if (item == null) return;

        if (item.type == acceptedType)
        {
            Debug.Log("Correct bin!");
            Destroy(item.gameObject);
        }
        else
        {
            Debug.Log("Wrong bin!");
        }
    }
}