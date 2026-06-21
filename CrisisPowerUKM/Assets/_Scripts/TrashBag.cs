using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

// Namespace wajib untuk XR Interaction Toolkit Unity 6
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class TrashBag : MonoBehaviour
{
    [Header("Inventory Settings")]
    public List<TrashItem> items = new List<TrashItem>();
    public int capacity = 20;

    public Transform holdPoint;
    private TrashItem heldItem;

    [Header("UI Settings")]
    public Button[] itemButtons;
    public TextMeshProUGUI[] buttonTexts;

    [Header("Juice Effects (Kesan Impak)")]
    public AudioSource collectSound;
    public ParticleSystem collectParticle;
    public Transform bagVisual;

    [Header("XR Settings (Sistem Paksa Pegang)")]
    public XRInteractionManager interactionManager;
    public XRBaseInteractor handInteractor; // Boleh terima mana-mana jenis interactor tangan

    private int selectedIndex = -1;
    private Vector3 originalScale;

    void Start()
    {
        UpdateUI();
        if (bagVisual != null) originalScale = bagVisual.localScale;

        if (interactionManager == null)
        {
            interactionManager = FindFirstObjectByType<XRInteractionManager>();
        }
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

        if (TrashManager.Instance != null)
        {
            TrashManager.Instance.TrashCollected();
        }

        UpdateUI();
    }

    // === DIUBAH: Menggunakan SelectEnter mengikut permintaan Unity 6 ===
    public void SelectItem(int index)
    {
        if (index < 0 || index >= items.Count) return;

        selectedIndex = index;
        TrashItem itemToOutput = items[index];

        // 1. Aktifkan semula objek sampah
        itemToOutput.gameObject.SetActive(true);

        if (handInteractor != null)
        {
            itemToOutput.transform.position = handInteractor.transform.position;
        }
        else
        {
            itemToOutput.transform.position = holdPoint.position;
        }

        // 2. Trik Unity 6: Guna SelectEnter dengan cara penukaran (Casting) Interface yang betul
        XRGrabInteractable grabableTrash = itemToOutput.GetComponent<XRGrabInteractable>();
        if (grabableTrash != null && handInteractor != null && interactionManager != null)
        {
            // Unity 6 memerlukan penukaran jenis ke IXRSelectInteractor & IXRSelectInteractable
            IXRSelectInteractor interactorRef = handInteractor.GetComponent<IXRSelectInteractor>();
            IXRSelectInteractable interactableRef = grabableTrash.GetComponent<IXRSelectInteractable>();

            if (interactorRef != null && interactableRef != null)
            {
                interactionManager.SelectEnter(interactorRef, interactableRef);
            }
        }

        // 3. Keluarkan dari data list beg
        items.RemoveAt(index);
        selectedIndex = -1;
        UpdateUI();
    }

    System.Collections.IEnumerator AnimateBagPunch()
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
        bagVisual.localScale = originalScale;
    }

    public TrashItem GetSelectedItem() { return null; }
    public void RemoveSelectedItem() { }

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