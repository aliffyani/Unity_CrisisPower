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

/// <summary>
/// Attach to every trash object in the scene.
/// 
/// - "type"     is used by TrashBin to check if it's the right bin (your existing logic).
/// - "itemData" is the InventoryFramework Item ScriptableObject — used to show the icon
///              in the hotbar when this piece of trash enters the bag.
///
/// Create Item ScriptableObjects via:
///   Right-click in Project → Create → Inventory → Item
/// Give each one a name, icon sprite, and leave maxStack = 1 for trash.
/// </summary>using UnityEngine;

/// <summary>
/// Attach to every trash object in the scene.
/// 
/// - "type"     is used by TrashBin to check if it's the right bin (your existing logic).
/// - "itemData" is the InventoryFramework Item ScriptableObject — used to show the icon
///              in the hotbar when this piece of trash enters the bag.
///
/// Create Item ScriptableObjects via:
///   Right-click in Project → Create → Inventory → Item
/// Give each one a name, icon sprite, and leave maxStack = 1 for trash.
/// </summary>
public class TrashItem : MonoBehaviour
{
    [Header("Trash Type (for bin sorting)")]
    public TrashType type;

    [Header("Hotbar Icon Data")]
    [Tooltip("Assign the matching InventoryFramework Item ScriptableObject so its icon shows in the hotbar.")]
    public InventoryFramework.Item itemData;
}

public enum TrashType
{
    Paper,
    Plastic,
    Glass
}
