//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//namespace InventoryFramework
//{
//    /// <summary>
//    /// XR-ready HotbarUI — hotbar only, no full inventory panel.
//    ///
//    /// World Space Canvas Setup (wrist):
//    ///  1. Create Canvas → Render Mode: World Space
//    ///  2. Add TrackedDeviceGraphicRaycaster (NOT the normal GraphicRaycaster)
//    ///  3. Scale to ~0.001, parent to Left Controller transform
//    ///  4. Add a child with HorizontalLayoutGroup → slotParent
//    ///  5. Fill in Inspector fields below
//    /// </summary>
//    public class HotbarUI : MonoBehaviour
//    {
//        [Header("References")]
//        public Hotbar hotbar;
//        public Transform slotParent;
//        public GameObject slotPrefab;
//        public ItemTooltip tooltip;       // optional
//        public Transform toolsParent;     // optional: 3D model spawn point

//        [Header("Drag Support")]
//        public RectTransform dragLayer;
//        public Canvas rootCanvas;

//        [Header("Pull-Out Support")]
//        [Tooltip("Assign your TrashBag so slot buttons know who to call when clicked.")]
//        public TrashBag bag;

//        private List<InventorySlotUI> slotUIs = new();
//        private int selectedIndex = 0;

//        void Start()
//        {
//            BuildSlots();
//            RefreshUI();
//        }

//        void BuildSlots()
//        {
//            foreach (Transform child in slotParent)
//                Destroy(child.gameObject);

//            slotUIs.Clear();

//            for (int i = 0; i < hotbar.size; i++)
//            {
//                var go = Instantiate(slotPrefab, slotParent);
//                var ui = go.GetComponent<InventorySlotUI>();
//                ui.tooltip = tooltip;
//                ui.SetupHotbar(hotbar, null, i, this);
//                slotUIs.Add(ui);

//                // Wire up the pull-out button so the player can click a slot
//                // with the XR ray to take the item back out of the bag
//                var btn = go.GetComponent<HotbarSlotButton>();
//                if (btn == null)
//                    btn = go.AddComponent<HotbarSlotButton>();
//                btn.slotIndex = i;
//                btn.bag = bag;
//            }
//        }

//        public void SetSelectedIndex(int index)
//        {
//            selectedIndex = Mathf.Clamp(index, 0, hotbar.size - 1);
//            RefreshUI();
//        }

//        public void SelectNext()
//        {
//            selectedIndex = (selectedIndex + 1) % hotbar.size;
//            RefreshUI();
//        }

//        public void SelectPrevious()
//        {
//            selectedIndex = (selectedIndex - 1 + hotbar.size) % hotbar.size;
//            RefreshUI();
//        }

//        public int GetSelectedIndex() => selectedIndex;

//        public void RefreshUI()
//        {
//            for (int i = 0; i < slotUIs.Count; i++)
//            {
//                if (slotUIs[i] == null || slotUIs[i].gameObject == null) continue;

//                slotUIs[i].SetSlot(hotbar.slots[i]);

//                if (slotUIs[i].transform.childCount == 0) continue;
//                var bg = slotUIs[i].transform.GetChild(0).GetComponent<Image>();
//                if (bg != null)
//                    bg.color = (i == selectedIndex) ? Color.yellow : Color.white;
//            }

//            if (toolsParent != null)
//            {
//                for (int x = toolsParent.childCount - 1; x >= 0; x--)
//                    Destroy(toolsParent.GetChild(x).gameObject);

//                var slot = (selectedIndex < hotbar.slots.Count) ? hotbar.slots[selectedIndex] : null;
//                if (slot != null && !slot.IsEmpty && slot.item?.model != null)
//                    Instantiate(slot.item.model, toolsParent);
//            }
//        }
//    }
//}


using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace InventoryFramework
{
    public class HotbarUI : MonoBehaviour
    {
        [Header("References")]
        public Hotbar hotbar;
        public Transform slotParent;
        public GameObject slotPrefab;
        public ItemTooltip tooltip;
        public Transform toolsParent;

        [Header("Drag Support")]
        public RectTransform dragLayer;
        public Canvas rootCanvas;

        [Header("Pull-Out Support")]
        public TrashBag bag;

        private List<InventorySlotUI> slotUIs = new();
        private int selectedIndex = 0;

        void Start()
        {
            BuildSlots();
            RefreshUI();
        }

        void BuildSlots()
        {
            foreach (Transform child in slotParent)
                Destroy(child.gameObject);

            slotUIs.Clear();

            for (int i = 0; i < hotbar.size; i++)
            {
                var go = Instantiate(slotPrefab, slotParent);
                var ui = go.GetComponent<InventorySlotUI>();
                ui.tooltip = tooltip;
                ui.SetupHotbar(hotbar, null, i, this);
                slotUIs.Add(ui);

                // Remove any leftover HotbarSlotButton baked into the prefab
                // to avoid the "missing script" error
                var existing = go.GetComponent<HotbarSlotButton>();
                if (existing != null)
                    Destroy(existing);

                // Add fresh and assign BEFORE Awake can fire on it
                // (AddComponent triggers Awake immediately, so we assign after)
                var btn = go.AddComponent<HotbarSlotButton>();
                btn.slotIndex = i;
                btn.bag = bag;  // assign right after AddComponent
            }
        }

        public void SetSelectedIndex(int index)
        {
            selectedIndex = Mathf.Clamp(index, 0, hotbar.size - 1);
            RefreshUI();
        }

        public void SelectNext()
        {
            selectedIndex = (selectedIndex + 1) % hotbar.size;
            RefreshUI();
        }

        public void SelectPrevious()
        {
            selectedIndex = (selectedIndex - 1 + hotbar.size) % hotbar.size;
            RefreshUI();
        }

        public int GetSelectedIndex() => selectedIndex;

        public void RefreshUI()
        {
            for (int i = 0; i < slotUIs.Count; i++)
            {
                if (slotUIs[i] == null || slotUIs[i].gameObject == null) continue;

                slotUIs[i].SetSlot(hotbar.slots[i]);

                if (slotUIs[i].transform.childCount == 0) continue;
                var bg = slotUIs[i].transform.GetChild(0).GetComponent<Image>();
                if (bg != null)
                    bg.color = (i == selectedIndex) ? Color.yellow : Color.white;
            }

            if (toolsParent != null)
            {
                for (int x = toolsParent.childCount - 1; x >= 0; x--)
                    Destroy(toolsParent.GetChild(x).gameObject);

                var slot = (selectedIndex < hotbar.slots.Count) ? hotbar.slots[selectedIndex] : null;
                if (slot != null && !slot.IsEmpty && slot.item?.model != null)
                    Instantiate(slot.item.model, toolsParent);
            }
        }
    }
}
