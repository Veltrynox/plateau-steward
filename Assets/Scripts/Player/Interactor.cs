using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles player interaction with interactable objects in the game world.
/// </summary>

public class Interactor : MonoBehaviour
{
    public float interactDistance = 3f;
    private Transform cam;

    private void Awake()
    {
        cam = Camera.main != null ? Camera.main.transform : null;
    }

    private void OnInteract(InputValue value)
    {
        if (value.isPressed)
        {
            Ray ray = new Ray(cam.position, cam.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
            {
                IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact(gameObject);
                }
            }
        }
    }
}
