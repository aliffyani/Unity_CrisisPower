
//using System.Collections.Generic;
//using UnityEngine;
//using TMPro;
//using UnityEngine.UI;

//// Namespace wajib untuk XR Interaction Toolkit Unity 6
//using UnityEngine.XR.Interaction.Toolkit;
//using UnityEngine.XR.Interaction.Toolkit.Interactors;
//using UnityEngine.XR.Interaction.Toolkit.Interactables;

//public class TrashBag : MonoBehaviour
//{
//    [Header("Inventory Settings")]
//    public List<TrashItem> items = new List<TrashItem>();
//    public int capacity = 20;

//    public Transform holdPoint;
//    private TrashItem heldItem;

//    [Header("UI Settings")]
//    public Button[] itemButtons;
//    public TextMeshProUGUI[] buttonTexts;

//    [Header("Juice Effects (Kesan Impak)")]
//    public AudioSource collectSound;
//    public ParticleSystem collectParticle;
//    public Transform bagVisual;

//    [Header("XR Settings (Sistem Paksa Pegang)")]
//    public XRInteractionManager interactionManager;
//    public XRBaseInteractor handInteractor; // Boleh terima mana-mana jenis interactor tangan

//    private int selectedIndex = -1;
//    private Vector3 originalScale;

//    void Start()
//    {
//        UpdateUI();
//        if (bagVisual != null) originalScale = bagVisual.localScale;

//        if (interactionManager == null)
//        {
//            interactionManager = FindFirstObjectByType<XRInteractionManager>();
//        }
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

//        if (TrashManager.Instance != null)
//        {
//            TrashManager.Instance.TrashCollected();
//        }

//        UpdateUI();
//    }

//    // === DIUBAH: Menggunakan SelectEnter mengikut permintaan Unity 6 ===
//    public void SelectItem(int index)
//    {
//        if (index < 0 || index >= items.Count) return;

//        selectedIndex = index;
//        TrashItem itemToOutput = items[index];

//        // 1. Aktifkan semula objek sampah
//        itemToOutput.gameObject.SetActive(true);

//        if (handInteractor != null)
//        {
//            itemToOutput.transform.position = handInteractor.transform.position;
//        }
//        else
//        {
//            itemToOutput.transform.position = holdPoint.position;
//        }

//        // 2. Trik Unity 6: Guna SelectEnter dengan cara penukaran (Casting) Interface yang betul
//        XRGrabInteractable grabableTrash = itemToOutput.GetComponent<XRGrabInteractable>();
//        if (grabableTrash != null && handInteractor != null && interactionManager != null)
//        {
//            // Unity 6 memerlukan penukaran jenis ke IXRSelectInteractor & IXRSelectInteractable
//            IXRSelectInteractor interactorRef = handInteractor.GetComponent<IXRSelectInteractor>();
//            IXRSelectInteractable interactableRef = grabableTrash.GetComponent<IXRSelectInteractable>();

//            if (interactorRef != null && interactableRef != null)
//            {
//                interactionManager.SelectEnter(interactorRef, interactableRef);
//            }
//        }

//        // 3. Keluarkan dari data list beg
//        items.RemoveAt(index);
//        selectedIndex = -1;
//        UpdateUI();
//    }

//    System.Collections.IEnumerator AnimateBagPunch()
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
//        bagVisual.localScale = originalScale;
//    }

//    public TrashItem GetSelectedItem() { return null; }
//    public void RemoveSelectedItem() { }

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


//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using TMPro;
//using UnityEngine.UI;
//using UnityEngine.XR.Interaction.Toolkit;
//using UnityEngine.XR.Interaction.Toolkit.Interactors;
//using UnityEngine.XR.Interaction.Toolkit.Interactables;

//public class TrashBag : MonoBehaviour
//{
//    [Header("Inventory Settings")]
//    public List<TrashItem> items = new List<TrashItem>();
//    public int capacity = 20;

//    public Transform holdPoint;
//    private TrashItem heldItem;
//    private int selectedIndex = -1;

//    [Header("UI Settings")]
//    public Button[] itemButtons;
//    public TextMeshProUGUI[] buttonTexts;

//    [Header("Juice Effects")]
//    public AudioSource collectSound;
//    public ParticleSystem collectParticle;
//    public Transform bagVisual;

//    [Header("XR Settings")]
//    public XRInteractionManager interactionManager;
//    public XRBaseInteractor handInteractor;

//    private Vector3 originalScale;

//    // ─────────────────────────────────────────────────────────────
//    void Start()
//    {
//        if (bagVisual != null) originalScale = bagVisual.localScale;

//        if (interactionManager == null)
//            interactionManager = FindFirstObjectByType<XRInteractionManager>();

//        UpdateUI();
//    }

//    // ── Player walks over trash → goes into bag ───────────────────
//    void OnTriggerEnter(Collider other)
//    {
//        TrashItem item = other.GetComponent<TrashItem>();
//        if (item == null) return;
//        AddItem(item);
//    }

//    public void AddItem(TrashItem item)
//    {
//        if (items.Count >= capacity)
//        {
//            Debug.Log("[TrashBag] Bag full!");
//            return;
//        }

//        // Sound + particle on pickup (from friend's script)
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
//        item.gameObject.SetActive(false); // hide in world, stored in bag

//        // Do NOT count here — scoring happens when thrown into the correct bin
//        Debug.Log("[TrashBag] Picked up: " + item.type + " | Bag: " + items.Count + "/" + capacity);

//        UpdateUI();
//    }

//    // ── Player selects item from bag UI → appears in hand ─────────
//    public void SelectItem(int index)
//    {
//        if (index < 0 || index >= items.Count) return;

//        selectedIndex = index;
//        TrashItem item = items[index];

//        // Hide previously held item
//        if (heldItem != null)
//            heldItem.gameObject.SetActive(false);

//        heldItem = item;
//        heldItem.gameObject.SetActive(true);

//        // Position at hand or holdPoint
//        if (handInteractor != null)
//            heldItem.transform.position = handInteractor.transform.position;
//        else if (holdPoint != null)
//            heldItem.transform.position = holdPoint.position;

//        // Reset physics so it doesn't fly off
//        Rigidbody rb = heldItem.GetComponent<Rigidbody>();
//        if (rb != null)
//        {
//            rb.isKinematic = false;
//            rb.useGravity = true;
//            rb.linearVelocity = Vector3.zero;
//            rb.angularVelocity = Vector3.zero;
//        }

//        // Force XR grab so the controller physically holds it (from friend's script)
//        XRGrabInteractable grabable = heldItem.GetComponent<XRGrabInteractable>();
//        if (grabable != null && handInteractor != null && interactionManager != null)
//        {
//            IXRSelectInteractor interactorRef = handInteractor.GetComponent<IXRSelectInteractor>();
//            IXRSelectInteractable interactableRef = grabable.GetComponent<IXRSelectInteractable>();

//            if (interactorRef != null && interactableRef != null)
//                interactionManager.SelectEnter(interactorRef, interactableRef);
//        }

//        Debug.Log("[TrashBag] Selected + shown in hand: " + item.type);
//    }

//    public TrashItem GetSelectedItem()
//    {
//        if (selectedIndex < 0 || selectedIndex >= items.Count) return null;
//        return items[selectedIndex];
//    }

//    // ── Called by TrashBin after a correct OR wrong sort ──────────
//    // Correct  → TrashBin already called TrySortTrash, just clean up the bag
//    // Wrong    → item stays alive in world (TrashBin bounces it back), remove from bag
//    public void RemoveItemAfterBinSort(TrashItem item)
//    {
//        int index = items.IndexOf(item);
//        if (index < 0) return;

//        items.RemoveAt(index);

//        if (selectedIndex == index)
//        {
//            selectedIndex = -1;
//            heldItem = null;
//        }

//        Debug.Log("[TrashBag] Removed from bag after sort: " + item.type + " | Remaining: " + items.Count);
//        UpdateUI();
//    }

//    // ── Manual discard from bag UI ────────────────────────────────
//    public void RemoveSelectedItem()
//    {
//        if (selectedIndex < 0 || selectedIndex >= items.Count) return;

//        TrashItem item = items[selectedIndex];
//        Debug.Log("[TrashBag] Discarding: " + item.type);

//        items.RemoveAt(selectedIndex);
//        Destroy(item.gameObject);

//        selectedIndex = -1;
//        heldItem = null;
//        UpdateUI();
//    }

//    // ── Bag punch animation (from friend's script) ────────────────
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

//        bagVisual.localScale = originalScale;
//    }

//    // ── UI update ─────────────────────────────────────────────────
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


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class TrashBag : MonoBehaviour
{
    [Header("Inventory Settings")]
    public List<TrashItem> items = new List<TrashItem>();
    public int capacity = 20;

    [Header("Spawn Settings")]
    public Transform handHoldPoint; // TARIK OBJEK KOSONG DI TANGAN (TEMPAT SPARK SAMPAH) KE SINI

    private TrashItem heldItem;
    private int selectedIndex = -1;

    [Header("UI Settings")]
    public Button[] itemButtons;
    public TextMeshProUGUI[] buttonTexts;

    [Header("Juice Effects")]
    public AudioSource collectSound;
    public ParticleSystem collectParticle;
    public Transform bagVisual;

    private Vector3 originalScale;

    // ─────────────────────────────────────────────────────────────
    void Start()
    {
        if (bagVisual != null) originalScale = bagVisual.localScale;
        UpdateUI();
    }

    // ── Player walks over trash → goes into bag ───────────────────
    void OnTriggerEnter(Collider other)
    {
        TrashItem item = other.GetComponent<TrashItem>();
        if (item == null) return;
        AddItem(item);
    }

    public void AddItem(TrashItem item)
    {
        if (items.Count >= capacity)
        {
            Debug.Log("[TrashBag] Bag full!");
            return;
        }

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
        item.gameObject.SetActive(false); // Sembunyikan objek dalam dunia game

        Debug.Log("[TrashBag] Picked up: " + item.type + " | Bag: " + items.Count + "/" + capacity);

        UpdateUI();
    }

    // ── DIUBAH: Mengeluarkan sampah secara natural ke posisi tangan ─────────
    public void SelectItem(int index)
    {
        if (index < 0 || index >= items.Count) return;

        selectedIndex = index;
        TrashItem item = items[index];

        // Sembunyikan sampah lama jika masih ada terpaku di tangan
        if (heldItem != null)
            heldItem.gameObject.SetActive(false);

        heldItem = item;
        heldItem.gameObject.SetActive(true);

        // Letakkan objek tepat pada rujukan titik tangan (handHoldPoint) supaya tidak lekat terlalu dekat/herot
        if (handHoldPoint != null)
        {
            heldItem.transform.position = handHoldPoint.position;
            heldItem.transform.rotation = handHoldPoint.rotation;
        }

        // Reset fizik supaya objek tidak meluncur laju atau jatuh terus
        Rigidbody rb = heldItem.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // NOTA: Kod paksaan SelectEnter dibuang agar pemain boleh klik grip controller sendiri secara natural untuk ambil & release.

        Debug.Log("[TrashBag] Selected + shown in hand: " + item.type);
    }

    public TrashItem GetSelectedItem()
    {
        if (selectedIndex < 0 || selectedIndex >= items.Count) return null;
        return items[selectedIndex];
    }

    // ── Called by TrashBin after a correct OR wrong sort ──────────
    public void RemoveItemAfterBinSort(TrashItem item)
    {
        int index = items.IndexOf(item);
        if (index < 0) return;

        items.RemoveAt(index);

        if (selectedIndex == index)
        {
            selectedIndex = -1;
            heldItem = null;
        }

        Debug.Log("[TrashBag] Removed from bag after sort: " + item.type + " | Remaining: " + items.Count);
        UpdateUI();
    }

    // ── Manual discard from bag UI ────────────────────────────────
    public void RemoveSelectedItem()
    {
        if (selectedIndex < 0 || selectedIndex >= items.Count) return;

        TrashItem item = items[selectedIndex];
        Debug.Log("[TrashBag] Discarding: " + item.type);

        items.RemoveAt(selectedIndex);
        Destroy(item.gameObject);

        selectedIndex = -1;
        heldItem = null;
        UpdateUI();
    }

    // ── Bag punch animation ────────────────
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

    // ── UI update ─────────────────────────────────────────────────
    void UpdateUI()
    {
        if (itemButtons == null || buttonTexts == null) return;

        for (int i = 0; i < itemButtons.Length; i++)
        {
            if (i >= buttonTexts.Length) break;
            if (itemButtons[i] == null) continue;

            if (i < items.Count)
            {
                itemButtons[i].gameObject.SetActive(true);
                if (buttonTexts[i] != null)
                    buttonTexts[i].text = items[i].type.ToString();
            }
            else
            {
                itemButtons[i].gameObject.SetActive(false);
            }
        }
    }
}