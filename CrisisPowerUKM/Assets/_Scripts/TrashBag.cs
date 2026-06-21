


//using System.Collections.Generic;
//using UnityEngine;
//using TMPro;
//using UnityEngine.UI;

//public class TrashBag : MonoBehaviour
//{
//    public List<TrashItem> items = new List<TrashItem>();
//    public int capacity = 20;

//    public Transform holdPoint;
//    private TrashItem heldItem;


//    public Button[] itemButtons;
//    public TextMeshProUGUI[] buttonTexts;

//    private int selectedIndex = -1;

//    void Start()
//    {
//        UpdateUI();
//    }

//    void OnTriggerEnter(Collider other)
//    {
//        TrashItem item = other.GetComponent<TrashItem>();
//        if (item == null) return;

//        AddItem(item);
//    }

//    public void AddItem(TrashItem item)
//    {
//        if (items.Count >= capacity) return;

//        items.Add(item);
//        item.gameObject.SetActive(false);

//        // --- NEW CODE ADDED HERE ---
//        // The moment the trash enters the bag, notify the manager!
//        if (TrashManager.Instance != null)
//        {
//            TrashManager.Instance.TrashCollected();
//        }
//        // ---------------------------

//        UpdateUI();
//    }

//    public void SelectItem(int index)
//    {
//        if (index < 0 || index >= items.Count) return;

//        selectedIndex = index;

//        TrashItem item = items[index];

//        // make previous held item disappear
//        if (heldItem != null)
//        {
//            heldItem.gameObject.SetActive(false);
//        }

//        // show selected item in front of player
//        heldItem = item;
//        heldItem.gameObject.SetActive(true);

//        heldItem.transform.position = holdPoint.position;
//        heldItem.transform.rotation = holdPoint.rotation;

//        Debug.Log("Selected + shown: " + item.type);
//    }

//    public TrashItem GetSelectedItem()
//    {
//        if (selectedIndex < 0 || selectedIndex >= items.Count) return null;
//        return items[selectedIndex];
//    }

//    public void RemoveSelectedItem()
//    {
//        if (selectedIndex < 0 || selectedIndex >= items.Count)
//            return;

//        TrashItem item = items[selectedIndex];

//        Debug.Log("Removing item: " + item.type);
//        Debug.Log("Inventory size: " + items.Count);

//        // Remove from inventory list
//        items.RemoveAt(selectedIndex);

//        // Destroy the physical object in the world
//        Destroy(item.gameObject);

//        selectedIndex = -1;
//        heldItem = null;

//        UpdateUI();
//    }

//    void UpdateUI()
//    {
//        for (int i = 0; i < itemButtons.Length; i++)
//        {
//            if (i < items.Count)
//            {
//                itemButtons[i].gameObject.SetActive(true);
//                buttonTexts[i].text = items[i].type.ToString();
//            }
//            else
//            {
//                itemButtons[i].gameObject.SetActive(false);
//            }
//        }
//    }
//}
//



using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using InventoryFramework;

public class TrashBag : MonoBehaviour
{
    public List<TrashItem> items = new List<TrashItem>();
    public int capacity = 20;

    public Transform holdPoint;
    private TrashItem heldItem;
    private int selectedIndex = -1;

    [Header("Old button UI (optional)")]
    public Button[] itemButtons;
    public TextMeshProUGUI[] buttonTexts;

    [Header("Hotbar (InventoryFramework)")]
    public Hotbar hotbar;
    public HotbarUI hotbarUI;

    [Header("Pull-Out Spawn Point")]
    [Tooltip("Drag in Right Controller > Attach Point transform.")]
    public Transform rightHandAttachPoint;

    private Dictionary<TrashItem, int> hotbarSlotMap = new();

    void Start()
    {
        Debug.Log("[TrashBag] Start — capacity: " + capacity);
        Debug.Log("[TrashBag] hotbar assigned: " + (hotbar != null));
        Debug.Log("[TrashBag] hotbarUI assigned: " + (hotbarUI != null));
        Debug.Log("[TrashBag] rightHandAttachPoint assigned: " + (rightHandAttachPoint != null));
        UpdateUI();
    }

    void OnTriggerEnter(Collider other)
    {
        TrashItem item = other.GetComponent<TrashItem>();
        if (item == null) return;
        Debug.Log("[TrashBag] OnTriggerEnter — detected trash: " + other.name);
        AddItem(item);
    }

    public void AddItem(TrashItem item)
    {
        if (items.Count >= capacity)
        {
            Debug.LogWarning("[TrashBag] Bag full! Cannot add: " + item.name);
            return;
        }

        items.Add(item);
        item.gameObject.SetActive(false);

        if (hotbar != null && item.itemData != null)
        {
            int slotIndex = FindNextEmptyHotbarSlot();
            bool added = hotbar.AddItem(item.itemData, 1);
            Debug.Log("[TrashBag] Added to hotbar slot " + slotIndex + " | success: " + added + " | item: " + item.itemData.itemName);
            if (added && slotIndex >= 0)
                hotbarSlotMap[item] = slotIndex;

            if (hotbarUI != null)
                hotbarUI.RefreshUI();
        }
        else if (item.itemData == null)
        {
            Debug.LogWarning("[TrashBag] ⚠️ '" + item.name + "' has no itemData assigned — icon won't show in hotbar. Assign an Item ScriptableObject to TrashItem.itemData.");
        }

        if (TrashManager.Instance != null)
            TrashManager.Instance.TrashCollected();

        Debug.Log("[TrashBag] Bag now has " + items.Count + " item(s).");
        UpdateUI();
    }

    public void PullOutItem(int slotIndex)
    {
        Debug.Log("[TrashBag] ──────────────────────────────────");
        Debug.Log("[TrashBag] PullOutItem called → slot: " + slotIndex);
        Debug.Log("[TrashBag] Items in bag: " + items.Count);
        Debug.Log("[TrashBag] hotbarSlotMap entries: " + hotbarSlotMap.Count);

        foreach (var kvp in hotbarSlotMap)
            Debug.Log("[TrashBag]   map: " + kvp.Key.name + " → slot " + kvp.Value);

        TrashItem target = null;
        foreach (var kvp in hotbarSlotMap)
        {
            if (kvp.Value == slotIndex)
            {
                target = kvp.Key;
                break;
            }
        }

        if (target == null)
        {
            Debug.LogWarning("[TrashBag] ⚠️ No item mapped to slot " + slotIndex + " — slot is empty or item was never mapped.");
            return;
        }

        Debug.Log("[TrashBag] ✅ Found: " + target.name + " (" + target.type + ")");

        target.gameObject.SetActive(true);

        if (rightHandAttachPoint != null)
        {
            Debug.Log("[TrashBag] Spawning at: " + rightHandAttachPoint.name + " | pos: " + rightHandAttachPoint.position);
            target.transform.position = rightHandAttachPoint.position;
            target.transform.rotation = rightHandAttachPoint.rotation;
            target.transform.SetParent(rightHandAttachPoint);
        }
        else
        {
            Debug.LogWarning("[TrashBag] ⚠️ rightHandAttachPoint is NULL! Assign it in TrashBag Inspector. Falling back to camera forward.");
            var cam = Camera.main;
            if (cam != null)
                target.transform.position = cam.transform.position + cam.transform.forward * 0.5f;
        }

        items.Remove(target);

        if (hotbar != null && hotbarSlotMap.ContainsKey(target))
        {
            var slot = hotbar.GetSlot(hotbarSlotMap[target]);
            if (slot != null) { slot.item = null; slot.count = 0; }
            hotbarSlotMap.Remove(target);
            Debug.Log("[TrashBag] Slot " + slotIndex + " cleared from hotbar.");
        }

        if (target == heldItem) heldItem = null;
        selectedIndex = -1;

        if (hotbarUI != null)
            hotbarUI.RefreshUI();

        UpdateUI();
        Debug.Log("[TrashBag] ✅ PullOut complete. Bag now has " + items.Count + " item(s).");
        Debug.Log("[TrashBag] ──────────────────────────────────");
    }

    public void SelectItem(int index)
    {
        if (index < 0 || index >= items.Count) return;
        selectedIndex = index;
        TrashItem item = items[index];
        if (heldItem != null) heldItem.gameObject.SetActive(false);
        heldItem = item;
        heldItem.gameObject.SetActive(true);
        heldItem.transform.position = holdPoint.position;
        heldItem.transform.rotation = holdPoint.rotation;
        if (hotbarSlotMap.TryGetValue(item, out int si) && hotbarUI != null)
            hotbarUI.SetSelectedIndex(si);
        Debug.Log("[TrashBag] SelectItem: " + item.type);
    }

    public TrashItem GetSelectedItem()
    {
        if (selectedIndex < 0 || selectedIndex >= items.Count) return null;
        return items[selectedIndex];
    }

    public void RemoveSelectedItem()
    {
        if (selectedIndex < 0 || selectedIndex >= items.Count) return;
        TrashItem item = items[selectedIndex];

        if (hotbar != null && hotbarSlotMap.TryGetValue(item, out int si))
        {
            var slot = hotbar.GetSlot(si);
            if (slot != null) { slot.item = null; slot.count = 0; }
            hotbarSlotMap.Remove(item);
            if (hotbarUI != null) hotbarUI.RefreshUI();
        }

        items.RemoveAt(selectedIndex);
        Destroy(item.gameObject);
        selectedIndex = -1;
        heldItem = null;
        UpdateUI();
        Debug.Log("[TrashBag] RemoveSelectedItem done. Bag has " + items.Count + " item(s).");
    }

    private int FindNextEmptyHotbarSlot()
    {
        if (hotbar == null) return -1;
        for (int i = 0; i < hotbar.slots.Count; i++)
            if (hotbar.slots[i].IsEmpty) return i;
        return -1;
    }

    void UpdateUI()
    {
        if (itemButtons == null) return;
        for (int i = 0; i < itemButtons.Length; i++)
        {
            if (i < items.Count)
            {
                itemButtons[i].gameObject.SetActive(true);
                buttonTexts[i].text = items[i].type.ToString();
            }
            else
            {
                itemButtons[i].gameObject.SetActive(false);
            }
        }
    }
}
