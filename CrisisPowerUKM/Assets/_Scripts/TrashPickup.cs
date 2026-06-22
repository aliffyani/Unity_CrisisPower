//using UnityEngine;

//public class TrashPickup : MonoBehaviour
//{
//    public TrashBag bag;

//    private void OnTriggerEnter(Collider other)
//    {
//        TrashItem item = other.GetComponent<TrashItem>();

//        if (item != null)
//        {
//            bag.AddItem(item);
//        }
//    }
//}

using UnityEngine;

public class TrashPickup : MonoBehaviour
{
    public TrashBag bag;

    private void OnTriggerEnter(Collider other)
    {
        TrashItem item = other.GetComponent<TrashItem>();
        if (item == null) return;

        // Stop highlight as soon as item is collected into bag
        TrashHighlight highlight = item.GetComponent<TrashHighlight>();
        if (highlight != null) highlight.StopHighlight();

        bag.AddItem(item);
    }
}