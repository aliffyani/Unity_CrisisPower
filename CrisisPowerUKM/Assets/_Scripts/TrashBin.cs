//using UnityEngine;

//public class TrashBin : MonoBehaviour
//{
//    public TrashType acceptedType;

//    private void OnTriggerEnter(Collider other)
//    {
//        TrashItem item = other.GetComponent<TrashItem>();

//        if (item == null) return;

//        if (item.type == acceptedType)
//        {
//            Debug.Log("Correct bin!");
//            Destroy(item.gameObject);
//        }
//        else
//        {
//            Debug.Log("Wrong bin!");
//        }
//    }
//}
using UnityEngine;

public class TrashBin : MonoBehaviour
{
    public TrashType acceptedType;
    public TrashBag bag;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something entered the bin: " + other.gameObject.name);

        TrashItem physicalItem = other.GetComponent<TrashItem>();

        if (physicalItem == null)
        {
            Debug.Log("Object has no TrashItem component.");
            return;
        }

        TrashItem selectedItem = bag.GetSelectedItem();

        if (selectedItem == null)
        {
            Debug.Log("No item selected in inventory!");
            return;
        }

        if (physicalItem != selectedItem)
        {
            Debug.Log("This is not the currently selected item.");
            return;
        }

        Debug.Log("Selected item type: " + selectedItem.type);
        Debug.Log("This bin accepts: " + acceptedType);

        if (selectedItem.type == acceptedType)
        {
            Debug.Log("CORRECT BIN!");

            // 1. Tell the manager to add 1 to the score
            //TrashManager.Instance.TrashCollected();

            // 2. Remove it from your UI inventory system data
            bag.RemoveSelectedItem();

            // 3. FIX: Destroy the physical object in the 3D world so it disappears!
            Destroy(other.gameObject);
        }
        else
        {
            Debug.Log("WRONG BIN!");
        }
    }
}