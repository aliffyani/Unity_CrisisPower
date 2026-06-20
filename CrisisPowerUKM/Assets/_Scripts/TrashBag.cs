
//using System.Collections.Generic;
//using UnityEngine;
//using TMPro;

//public class TrashBag : MonoBehaviour
//{
//    public List<TrashItem> items = new List<TrashItem>();
//    public int capacity = 20;

//    public TextMeshProUGUI itemListText;

//    void OnTriggerEnter(Collider other)
//    {
//        TrashItem item = other.GetComponent<TrashItem>();

//        if (item == null) return;

//        AddItem(item);
//    }

//    public bool AddItem(TrashItem item)
//    {
//        if (items.Count >= capacity)
//        {
//            Debug.Log("Bag full!");
//            return false;
//        }

//        items.Add(item);
//        item.gameObject.SetActive(false);

//        UpdateUI();
//        return true;
//    }

//    void UpdateUI()
//    {
//        itemListText.text = "";

//        for (int i = 0; i < items.Count; i++)
//        {
//            itemListText.text += i + ". " + items[i].type + "\n";
//        }
//    }
//}

using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TrashBag : MonoBehaviour
{
    public List<TrashItem> items = new List<TrashItem>();
    public int capacity = 20;

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

        UpdateUI();
    }

    public void SelectItem(int index)
    {
        if (index < 0 || index >= items.Count) return;

        selectedIndex = index;
        Debug.Log("Selected: " + items[index].type);
    }

    public TrashItem GetSelectedItem()
    {
        if (selectedIndex < 0 || selectedIndex >= items.Count) return null;
        return items[selectedIndex];
    }

    public void RemoveSelectedItem()
    {
        if (selectedIndex < 0 || selectedIndex >= items.Count) return;

        items.RemoveAt(selectedIndex);
        selectedIndex = -1;

        UpdateUI();
    }

    void UpdateUI()
    {
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