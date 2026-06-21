//using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.UI;

//[RequireComponent(typeof(Button))]
//public class HotbarSlotButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
//{
//    [HideInInspector] public int slotIndex;
//    [HideInInspector] public TrashBag bag;

//    private Button button;
//    private Image backgroundImage;
//    private Color normalColor;

//    void Awake()
//    {
//        button = GetComponent<Button>();
//        button.onClick.AddListener(OnSlotClicked);

//        backgroundImage = GetComponent<Image>();
//        if (backgroundImage != null)
//            normalColor = backgroundImage.color;

//        Debug.Log("[HotbarSlotButton] Awake — slot " + slotIndex + " | Button found: " + (button != null) + " | Bag assigned: " + (bag != null));
//    }

//    void Start()
//    {
//        // By Start(), HotbarUI has already assigned slotIndex and bag
//        Debug.Log("[HotbarSlotButton] Start — slot " + slotIndex + " | Bag: " + (bag != null ? bag.name : "NULL"));

//        if (button == null)
//            Debug.LogError("[HotbarSlotButton] ❌ No Button component on slot " + slotIndex + "! Add a Button component to the slot prefab.");

//        if (bag == null)
//            Debug.LogWarning("[HotbarSlotButton] ⚠️ Slot " + slotIndex + " has no TrashBag assigned. Assign it in HotbarUI Inspector → Bag field.");
//    }

//    void OnSlotClicked()
//    {
//        Debug.Log("[HotbarSlotButton] ✅ CLICKED slot " + slotIndex);

//        if (bag == null)
//        {
//            Debug.LogError("[HotbarSlotButton] ❌ Click received but TrashBag is null on slot " + slotIndex + ". Check HotbarUI Inspector → Bag field.");
//            return;
//        }

//        int itemCount = bag.items.Count;
//        Debug.Log("[HotbarSlotButton] Bag has " + itemCount + " item(s). Calling PullOutItem(" + slotIndex + ")...");

//        if (itemCount == 0)
//        {
//            Debug.LogWarning("[HotbarSlotButton] ⚠️ Bag is empty — nothing to pull out.");
//            return;
//        }

//        bag.PullOutItem(slotIndex);
//    }

//    public void OnPointerEnter(PointerEventData eventData)
//    {
//        Debug.Log("[HotbarSlotButton] 👆 Ray hovering slot " + slotIndex);
//        if (backgroundImage != null)
//            backgroundImage.color = new Color(1f, 1f, 0.5f, 1f);
//    }

//    public void OnPointerExit(PointerEventData eventData)
//    {
//        Debug.Log("[HotbarSlotButton] Ray left slot " + slotIndex);
//        if (backgroundImage != null)
//            backgroundImage.color = normalColor;
//    }
//}


using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



[RequireComponent(typeof(Button))]
public class HotbarSlotButton : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler
{
    [HideInInspector] public int slotIndex;
    [HideInInspector] public TrashBag bag;

    private Button button;
    private Image backgroundImage;
    private Color normalColor;

    void Start()
    {
        button = GetComponent<Button>();

        Debug.Log("[HotbarSlotButton] Ready — slot " + slotIndex +
                  " | Bag: " + (bag != null ? bag.name : "NULL"));

        button.onClick.RemoveAllListeners();

        button.onClick.AddListener(() =>
        {
            Debug.Log("[HotbarSlotButton] UNITY CLICK RECEIVED on slot " + slotIndex);
        });

        button.onClick.AddListener(OnSlotClicked);

        backgroundImage = GetComponent<Image>();
        if (backgroundImage != null)
            normalColor = backgroundImage.color;
    }

    void OnSlotClicked()
    {
        Debug.Log("[HotbarSlotButton] ✅ CLICKED slot " + slotIndex);

        if (bag == null)
        {
            Debug.LogError("[HotbarSlotButton] ❌ No TrashBag on slot " + slotIndex);
            return;
        }

        if (bag.items.Count == 0)
        {
            Debug.LogWarning("[HotbarSlotButton] ⚠️ Bag is empty.");
            return;
        }

        bag.PullOutItem(slotIndex);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("[HotbarSlotButton] 👆 Hovering slot " + slotIndex);
        if (backgroundImage != null)
            backgroundImage.color = new Color(1f, 1f, 0.5f, 1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (backgroundImage != null)
            backgroundImage.color = normalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("[HotbarSlotButton] POINTER CLICK on slot " + slotIndex);
        OnSlotClicked();
    }
}
