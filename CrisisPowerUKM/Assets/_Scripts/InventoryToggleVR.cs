using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryToggleVR : MonoBehaviour
{
    public GameObject inventoryPanel;

    public InputActionReference toggleAction;

    private void OnEnable()
    {
        toggleAction.action.performed += ToggleInventory;
        toggleAction.action.Enable();
    }

    private void OnDisable()
    {
        toggleAction.action.performed -= ToggleInventory;
        toggleAction.action.Disable();
    }

    private void ToggleInventory(InputAction.CallbackContext context)
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
    }
}