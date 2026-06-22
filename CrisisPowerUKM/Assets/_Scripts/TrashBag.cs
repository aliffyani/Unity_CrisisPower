
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;
//using UnityEngine.XR; // Ditambah untuk baca butang VR

//public class TrashBag : MonoBehaviour
//{
//    [Header("Inventory Settings")]
//    public List<TrashItem> items = new List<TrashItem>();
//    public int capacity = 20;

//    [Header("Spawn Settings (Wajib Letak Tangan Kiri)")]
//    public Transform leftHandHoldPoint;

//    [Header("UI Settings")]
//    public Button[] itemButtons;
//    public TextMeshProUGUI[] buttonTexts;

//    [Header("Juice Effects")]
//    public AudioSource collectSound;
//    public ParticleSystem collectParticle;
//    public Transform bagVisual;

//    private Vector3 originalScale;

//    void Start()
//    {
//        if (bagVisual != null) originalScale = bagVisual.localScale;
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
//        if (collectSound != null) collectSound.Play();

//        if (collectParticle != null)
//        {
//            ParticleSystem effect = Instantiate(collectParticle, item.transform.position, Quaternion.identity);
//            effect.Play();
//            Destroy(effect.gameObject, 1f);
//        }

//        if (bagVisual != null)
//        {
//            StopAllCoroutines();
//            StartCoroutine(AnimateBagPunch());
//        }

//        items.Add(item);
//        item.gameObject.SetActive(false);
//        UpdateUI();
//    }

//    public void SelectItem(int index)
//    {
//        if (index < 0 || index >= items.Count) return;

//        TrashItem storedItem = items[index];
//        if (storedItem == null) return;

//        Vector3 spawnPos = transform.position;
//        Quaternion spawnRot = Quaternion.identity;

//        if (leftHandHoldPoint != null)
//        {
//            spawnPos = leftHandHoldPoint.position;
//            spawnRot = leftHandHoldPoint.rotation;
//        }

//        TrashItem newItem = Instantiate(storedItem, spawnPos, spawnRot);
//        newItem.gameObject.SetActive(true);

//        Rigidbody rb = newItem.GetComponent<Rigidbody>();
//        if (rb != null)
//        {
//            rb.isKinematic = true;
//            rb.useGravity = false;
//        }

//        if (leftHandHoldPoint != null)
//        {
//            newItem.gameObject.AddComponent<TrashPerfectHold>().Setup(rb, leftHandHoldPoint);
//        }
//        else
//        {
//            Debug.LogWarning("[TrashBag] Sila masukkan Left Controller ke dalam slot Left Hand Hold Point!");
//        }

//        Destroy(storedItem.gameObject);
//        items.RemoveAt(index);
//        UpdateUI();
//    }

//    public TrashItem GetSelectedItem() { return null; }
//    public void RemoveItemAfterBinSort(TrashItem item) { }

//    IEnumerator AnimateBagPunch()
//    {
//        float duration = 0.15f;
//        float elapsed = 0f;
//        Vector3 punchScale = originalScale * 1.3f;

//        while (elapsed < duration)
//        {
//            elapsed += Time.deltaTime;
//            bagVisual.localScale = Vector3.Lerp(originalScale, punchScale, elapsed / duration);
//            yield return null;
//        }

//        elapsed = 0f;
//        while (elapsed < duration)
//        {
//            elapsed += Time.deltaTime;
//            bagVisual.localScale = Vector3.Lerp(punchScale, originalScale, elapsed / duration);
//            yield return null;
//        }

//        if (bagVisual != null) bagVisual.localScale = originalScale;
//    }

//    void UpdateUI()
//    {
//        if (itemButtons == null || buttonTexts == null) return;

//        for (int i = 0; i < itemButtons.Length; i++)
//        {
//            if (i >= buttonTexts.Length) break;
//            if (itemButtons[i] == null) continue;

//            if (i < items.Count)
//            {
//                itemButtons[i].gameObject.SetActive(true);
//                if (buttonTexts[i] != null)
//                    buttonTexts[i].text = items[i].type.ToString();
//            }
//            else
//            {
//                itemButtons[i].gameObject.SetActive(false);
//            }
//        }
//    }
//}

//// ── SKRIP GENGGAMAN MANUAL ──
//public class TrashPerfectHold : MonoBehaviour
//{
//    private Rigidbody targetRb;
//    private Transform handTransform;
//    private float holdSafetyTimer = 0.5f;

//    public void Setup(Rigidbody rb, Transform hand)
//    {
//        targetRb = rb;
//        handTransform = hand;
//    }

//    void Update()
//    {
//        if (targetRb == null || handTransform == null) return;

//        // Paksa botol lekat tepat 100% pada kedudukan tangan
//        transform.position = handTransform.position;
//        transform.rotation = handTransform.rotation;

//        if (holdSafetyTimer > 0)
//        {
//            holdSafetyTimer -= Time.deltaTime;
//            return;
//        }

//        // BACA INPUT VR: Semak jika pemain menekan butang pada Tangan Kiri
//        InputDevice leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
//        bool isTriggerPressed = false;
//        bool isGripPressed = false;

//        leftHand.TryGetFeatureValue(CommonUsages.triggerButton, out isTriggerPressed);
//        leftHand.TryGetFeatureValue(CommonUsages.gripButton, out isGripPressed);

//        // Jika pemain tekan Trigger (jari telunjuk), Grip (genggaman), ATAU Spacebar di PC (Simulator)
//        if (isTriggerPressed || isGripPressed || Input.GetKeyDown(KeyCode.Space))
//        {
//            DropTrash();
//        }
//    }

//    public void DropTrash()
//    {
//        if (targetRb != null)
//        {
//            targetRb.isKinematic = false; // Aktifkan fizik
//            targetRb.useGravity = true;   // Aktifkan graviti supaya jatuh ke bawah
//        }

//        Debug.Log("[TrashPerfectHold] Sampah dilepaskan manual oleh pemain!");
//        Destroy(this); // Padam skrip lekatan
//    }
//}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class TrashBag : MonoBehaviour
{
    [Header("Inventory Settings")]
    public List<TrashItem> items = new List<TrashItem>();
    public int capacity = 20;

    [Header("Spawn Settings")]
    public Transform leftHandHoldPoint;

    [Header("UI Settings")]
    public Button[] itemButtons;
    public TextMeshProUGUI[] buttonTexts;
    public Image[] buttonIcons; // <-- Add an array for your UI Images!

    [Header("Juice Effects")]
    public AudioSource collectSound;
    public ParticleSystem collectParticle;
    public Transform bagVisual;

    private Vector3 originalScale;

    void Start()
    {
        if (bagVisual != null) originalScale = bagVisual.localScale;
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

        // Stop highlight when item enters bag
        TrashHighlight highlight = item.GetComponent<TrashHighlight>();
        if (highlight != null) highlight.StopHighlight();

        if (collectSound != null) collectSound.Play();

        if (collectParticle != null)
        {
            ParticleSystem effect = Instantiate(collectParticle, item.transform.position, Quaternion.identity);
            effect.Play();
            Destroy(effect.gameObject, 1f);
        }

        if (bagVisual != null)
        {
            StopAllCoroutines();
            StartCoroutine(AnimateBagPunch());
        }

        items.Add(item);
        item.gameObject.SetActive(false);
        UpdateUI();
    }

    public void SelectItem(int index)
    {
        if (index < 0 || index >= items.Count) return;

        TrashItem storedItem = items[index];
        if (storedItem == null) return;

        Vector3 spawnPos = leftHandHoldPoint != null ? leftHandHoldPoint.position : transform.position;
        Quaternion spawnRot = leftHandHoldPoint != null ? leftHandHoldPoint.rotation : Quaternion.identity;

        // Instantiate inactive so we can configure it before Awake runs
        TrashItem newItem = Instantiate(storedItem, spawnPos, spawnRot);
        newItem.gameObject.SetActive(false);

        // Disable highlight BEFORE SetActive(true) so Awake sees startHighlighted=false
        // and never creates the outline — prevents any flash of yellow
        TrashHighlight newHighlight = newItem.GetComponent<TrashHighlight>();
        if (newHighlight != null) newHighlight.startHighlighted = false;

        newItem.gameObject.SetActive(true);

        Rigidbody rb = newItem.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        if (leftHandHoldPoint != null)
        {
            newItem.gameObject.AddComponent<TrashPerfectHold>().Setup(rb, leftHandHoldPoint);
        }
        else
        {
            Debug.LogWarning("[TrashBag] Assign Left Controller to Left Hand Hold Point slot!");
        }

        Destroy(storedItem.gameObject);
        items.RemoveAt(index);
        UpdateUI();
    }

    public TrashItem GetSelectedItem() { return null; }
    public void RemoveItemAfterBinSort(TrashItem item) { }

    IEnumerator AnimateBagPunch()
    {
        float duration = 0.15f;
        float elapsed = 0f;
        Vector3 punchScale = originalScale * 1.3f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            bagVisual.localScale = Vector3.Lerp(originalScale, punchScale, elapsed / duration);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            bagVisual.localScale = Vector3.Lerp(punchScale, originalScale, elapsed / duration);
            yield return null;
        }

        if (bagVisual != null) bagVisual.localScale = originalScale;
    }

    //void UpdateUI()
    //{
    //    if (itemButtons == null || buttonTexts == null) return;

    //    for (int i = 0; i < itemButtons.Length; i++)
    //    {
    //        if (i >= buttonTexts.Length) break;
    //        if (itemButtons[i] == null) continue;

    //        if (i < items.Count)
    //        {
    //            itemButtons[i].gameObject.SetActive(true);
    //            if (buttonTexts[i] != null)
    //                buttonTexts[i].text = items[i].itemName; // New code
    //        }
    //        else
    //        {
    //            itemButtons[i].gameObject.SetActive(false);
    //        }
    //    }
    //}

    void UpdateUI()
    {
        if (itemButtons == null) return;

        for (int i = 0; i < itemButtons.Length; i++)
        {
            if (itemButtons[i] == null) continue;

            if (i < items.Count)
            {
                itemButtons[i].gameObject.SetActive(true);

                // Update Text (Optional: you can empty this if you only want the picture)
                if (buttonTexts != null && i < buttonTexts.Length && buttonTexts[i] != null)
                {
                    buttonTexts[i].text = items[i].itemName;
                }

                // Update Image Icon
                if (buttonIcons != null && i < buttonIcons.Length && buttonIcons[i] != null)
                {
                    if (items[i].itemIcon != null)
                    {
                        buttonIcons[i].gameObject.SetActive(true);
                        buttonIcons[i].sprite = items[i].itemIcon; // Assign item sprite
                    }
                    else
                    {
                        buttonIcons[i].gameObject.SetActive(false); // Hide image if none assigned
                    }
                }
            }
            else
            {
                itemButtons[i].gameObject.SetActive(false);
            }
        }
    }
}

public class TrashPerfectHold : MonoBehaviour
{
    private Rigidbody targetRb;
    private Transform handTransform;
    private float holdSafetyTimer = 0.5f;

    public void Setup(Rigidbody rb, Transform hand)
    {
        targetRb = rb;
        handTransform = hand;
    }

    void Update()
    {
        if (targetRb == null || handTransform == null) return;

        transform.position = handTransform.position;
        transform.rotation = handTransform.rotation;

        if (holdSafetyTimer > 0)
        {
            holdSafetyTimer -= Time.deltaTime;
            return;
        }

        InputDevice leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        bool isTriggerPressed = false;
        bool isGripPressed = false;

        leftHand.TryGetFeatureValue(CommonUsages.triggerButton, out isTriggerPressed);
        leftHand.TryGetFeatureValue(CommonUsages.gripButton, out isGripPressed);

        if (isTriggerPressed || isGripPressed || Input.GetKeyDown(KeyCode.Space))
        {
            DropTrash();
        }
    }

    public void DropTrash()
    {
        if (targetRb != null)
        {
            targetRb.isKinematic = false;
            targetRb.useGravity = true;
        }
        Debug.Log("[TrashPerfectHold] Item dropped!");
        Destroy(this);
    }
}