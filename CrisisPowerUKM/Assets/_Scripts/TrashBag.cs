using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
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
    public XRBaseInteractor handInteractor;

    private int selectedIndex = -1;
    private Vector3 originalScale;

    void Start()
    {
        if (bagVisual != null) originalScale = bagVisual.localScale;

        if (interactionManager == null)
        {
            interactionManager = FindFirstObjectByType<XRInteractionManager>();
        }

        // Panggil UpdateUI selepas semua tetapan awal selesai
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

        selectedIndex = index;
        TrashItem itemToOutput = items[index];

        itemToOutput.gameObject.SetActive(true);

        if (handInteractor != null)
        {
            itemToOutput.transform.position = handInteractor.transform.position;
        }
        else
        {
            itemToOutput.transform.position = holdPoint.position;
        }

        XRGrabInteractable grabableTrash = itemToOutput.GetComponent<XRGrabInteractable>();
        if (grabableTrash != null && handInteractor != null && interactionManager != null)
        {
            IXRSelectInteractor interactorRef = handInteractor.GetComponent<IXRSelectInteractor>();
            IXRSelectInteractable interactableRef = grabableTrash.GetComponent<IXRSelectInteractable>();

            if (interactorRef != null && interactableRef != null)
            {
                interactionManager.SelectEnter(interactorRef, interactableRef);
            }
        }

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
        // KAWALAN KESELAMATAN: Keluar awal jika array UI belum diisi langsung di Inspector
        if (itemButtons == null || buttonTexts == null) return;

        for (int i = 0; i < itemButtons.Length; i++)
        {
            // Pastikan indeks i tidak melebihi saiz array teks bagi mengelakkan ralat out-of-bounds
            if (i >= buttonTexts.Length) break;

            // Jika slot butang ini kosong (None), langgar ke butang seterusnya
            if (itemButtons[i] == null) continue;

            if (i < items.Count)
            {
                itemButtons[i].gameObject.SetActive(true);

                // Pastikan komponen teks juga tidak kosong sebelum menukar teksnya
                if (buttonTexts[i] != null)
                {
                    buttonTexts[i].text = items[i].type.ToString();
                }
            }
            else
            {
                itemButtons[i].gameObject.SetActive(false);
            }
        }
    }
}