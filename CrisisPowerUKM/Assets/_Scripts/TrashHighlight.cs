using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class TrashHighlight : MonoBehaviour
{
    [Header("Pulse Settings")]
    public Color outlineColor = Color.yellow;
    public float pulseSpeed = 2f;

    [Range(1.01f, 1.15f)]
    public float outlineScale = 1.06f;

    // Set this to false BEFORE the object is enabled to prevent outline from ever appearing
    public bool startHighlighted = true;

    private GameObject outlineObject;
    private Material outlineMat;
    private bool isHighlighting = false;

    void Awake()
    {
        CreateOutline();

        // Apply startHighlighted immediately in Awake so it takes effect
        // before the first frame renders — prevents any flash of yellow
        if (startHighlighted)
            EnableHighlight();
        else
            DisableHighlight();
    }

    void Start()
    {
        XRGrabInteractable grab = GetComponentInParent<XRGrabInteractable>();
        if (grab != null)
        {
            grab.selectEntered.AddListener(_ => StopHighlight());
            Debug.Log("[TrashHighlight] Hooked grab on: " + grab.gameObject.name);
        }
    }

    void CreateOutline()
    {
        Shader outlineShader = Shader.Find("Custom/XRayOutline");
        if (outlineShader == null)
        {
            Debug.LogError("[TrashHighlight] Custom/XRayOutline shader not found!");
            return;
        }

        outlineMat = new Material(outlineShader);
        outlineMat.SetColor("_OutlineColor", outlineColor);

        outlineObject = new GameObject("_Outline");
        outlineObject.transform.SetParent(transform, false);
        outlineObject.transform.localPosition = Vector3.zero;
        outlineObject.transform.localRotation = Quaternion.identity;
        outlineObject.transform.localScale = Vector3.one * outlineScale;

        CopyMeshes(transform, outlineObject.transform);

        // Start hidden — Awake will enable it if needed
        outlineObject.SetActive(false);
    }

    void CopyMeshes(Transform source, Transform outlineParent)
    {
        MeshFilter mf = source.GetComponent<MeshFilter>();
        MeshRenderer mr = source.GetComponent<MeshRenderer>();

        if (mf != null && mr != null)
        {
            outlineParent.gameObject.AddComponent<MeshFilter>().sharedMesh = mf.sharedMesh;
            MeshRenderer newMR = outlineParent.gameObject.AddComponent<MeshRenderer>();
            Material[] mats = new Material[mr.sharedMaterials.Length];
            for (int i = 0; i < mats.Length; i++)
                mats[i] = outlineMat;
            newMR.sharedMaterials = mats;
            newMR.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            newMR.receiveShadows = false;
        }

        foreach (Transform child in source)
        {
            if (child.name == "_Outline") continue;

            GameObject childOutline = new GameObject(child.name + "_ol");
            childOutline.transform.SetParent(outlineParent, false);
            childOutline.transform.localPosition = child.localPosition;
            childOutline.transform.localRotation = child.localRotation;
            childOutline.transform.localScale = child.localScale;
            CopyMeshes(child, childOutline.transform);
        }
    }

    void Update()
    {
        if (!isHighlighting || outlineMat == null || outlineObject == null) return;

        float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
        Color c = outlineColor;
        c.a = Mathf.Lerp(0.2f, 1f, pulse);
        outlineMat.SetColor("_OutlineColor", c);

        float scalePulse = Mathf.Lerp(outlineScale, outlineScale + 0.02f, pulse);
        outlineObject.transform.localScale = Vector3.one * scalePulse;
    }

    private void EnableHighlight()
    {
        isHighlighting = true;
        if (outlineObject != null)
            outlineObject.SetActive(true);
    }

    private void DisableHighlight()
    {
        isHighlighting = false;
        if (outlineObject != null)
            outlineObject.SetActive(false);
    }

    // Called by TrashBag.AddItem and TrashPickup
    public void StopHighlight()
    {
        if (!isHighlighting) return;
        DisableHighlight();
        // Also prevent it from ever turning back on
        startHighlighted = false;
        Debug.Log("[TrashHighlight] Stopped on " + gameObject.name);
    }

    void OnDestroy()
    {
        if (outlineMat != null) Destroy(outlineMat);
    }
}