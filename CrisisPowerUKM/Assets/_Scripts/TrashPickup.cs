using UnityEngine;

public class TrashPickup : MonoBehaviour
{
    public TrashBag bag;

    private void OnTriggerEnter(Collider other)
    {
        TrashItem item = other.GetComponent<TrashItem>();

        if (item != null)
        {
            bag.AddItem(item);
        }
    }
}