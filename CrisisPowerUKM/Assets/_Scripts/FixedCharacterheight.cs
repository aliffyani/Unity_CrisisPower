using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FixedCharacterHeight : MonoBehaviour
{
    [Header("Fixed Height Settings")]
    public float fixedHeight = 1.8f;
    public float fixedCenterY = 0.9f;

    private CharacterController cc;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // Force height every frame so XR can't override it
        cc.height = fixedHeight;
        cc.center = new Vector3(cc.center.x, fixedCenterY, cc.center.z);
    }
}