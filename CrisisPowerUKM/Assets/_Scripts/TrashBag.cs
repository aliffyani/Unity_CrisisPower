using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TrashBag : MonoBehaviour
{
    public List<TrashItem> items = new List<TrashItem>();
    public int capacity = 20;

    public Transform holdPoint;
    private TrashItem heldItem;

    [Header("UI Arrays")]
    public Button[] itemButtons;
    public TextMeshProUGUI[] buttonTexts;

    private int selectedIndex = -1;

    void Start()
    {
        UpdateUI();
    }

    void OnTriggerEnter(Collider other)
    {
        TrashItem item = other.GetComponent<TrashItem>();
        if (item == null) return;

        AddItem(item);
    }

    public void AddItem(TrashItem item)
    {
        if (items.Count >= capacity) return;

        items.Add(item);
        item.gameObject.SetActive(false);

        // --- NEW CODE ADDED HERE ---
        // The moment the trash enters the bag, notify the manager!
        if (TrashManager.Instance != null)
        {
            TrashManager.Instance.TrashCollected();
        }
        // ---------------------------

        UpdateUI();
    }

    public void SelectItem(int index)
    {
        if (index < 0 || index >= items.Count) return;

        selectedIndex = index;

        TrashItem item = items[index];

        // make previous held item disappear
        if (heldItem != null)
        {
            heldItem.gameObject.SetActive(false);
        }

        // show selected item in front of player
        heldItem = item;
        heldItem.gameObject.SetActive(true);

        heldItem.transform.position = holdPoint.position;
        heldItem.transform.rotation = holdPoint.rotation;

        Debug.Log("Selected + shown: " + item.type);
    }

    public TrashItem GetSelectedItem()
    {
        if (selectedIndex < 0 || selectedIndex >= items.Count) return null;
        return items[selectedIndex];
    }

    public void RemoveSelectedItem()
    {
        if (selectedIndex < 0 || selectedIndex >= items.Count)
            return;

        TrashItem item = items[selectedIndex];

        Debug.Log("Removing item: " + item.type);
        Debug.Log("Inventory size: " + items.Count);

        // Remove from inventory list
        items.RemoveAt(selectedIndex);

        // Destroy the physical object in the world
        Destroy(item.gameObject);

        selectedIndex = -1;
        heldItem = null;

        UpdateUI();
    }

    void UpdateUI()
    {
        // Safety check 1: Exit early if arrays are not assigned at all in the inspector
        if (itemButtons == null || buttonTexts == null) return;

        for (int i = 0; i < itemButtons.Length; i++)
        {
            // Safety check 2: Avoid out-of-bounds errors if buttonTexts array is shorter than itemButtons
            if (i >= buttonTexts.Length) break;

            if (i < items.Count)
            {
                // Safety check 3: Make sure individual elements aren't empty (None)
                if (itemButtons[i] != null) itemButtons[i].gameObject.SetActive(true);
                if (buttonTexts[i] != null) buttonTexts[i].text = items[i].type.ToString();
            }
            else
            {
                if (itemButtons[i] != null) itemButtons[i].gameObject.SetActive(false);
            }
        }
    }
}