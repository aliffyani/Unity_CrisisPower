//using UnityEngine;

//public class TrashItem : MonoBehaviour
//{
//    public TrashType type;
//}

//public enum TrashType
//{
//    Paper,
//    Plastic,
//    Glass
//}

using UnityEngine;

public class TrashItem : MonoBehaviour
{
    public string itemName; // <-- Add this! (e.g., "Soda Can", "Water Bottle")\
    public Sprite itemIcon;
    public TrashType type;
}

public enum TrashType
{
    Paper,
    Plastic,
    Glass
}