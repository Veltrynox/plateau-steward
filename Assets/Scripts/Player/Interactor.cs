using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles player interaction with interactable objects in the game world.
/// </summary>
namespace SubnauticaClone
{
    public class Interactor : MonoBehaviour
    {
        public float interactDistance = 3f;
        private float checkRate = 0.1f;
        private float nextCheckTime;
        [SerializeField] private GameObject m_pickupUI;
        private Transform cam;

        private void Awake()
        {
            cam = Camera.main != null ? Camera.main.transform : null;
        }

        void Update()
        {
            if (Time.time > nextCheckTime)
            {
                nextCheckTime = Time.time + checkRate;
                Ray ray = new Ray(cam.position, cam.forward);
                if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
                {
                    IInteractable interactable = hit.collider.GetComponent<IInteractable>();
                    if (interactable != null)
                    {
                        m_pickupUI.SetActive(true);
                        m_pickupUI.transform.SetParent(hit.collider.transform);
                        m_pickupUI.transform.localPosition = Vector3.zero;
                        m_pickupUI.transform.localRotation = Quaternion.identity;
                        m_pickupUI.transform.localScale = Vector3.one * 1.2f;
                    }
                    else
                    {
                        m_pickupUI.SetActive(false);
                        if (m_pickupUI.transform.parent != null)
                        {
                            m_pickupUI.transform.SetParent(null);
                        }
                    }
                }
                else
                {
                    m_pickupUI.transform.SetParent(null);
                    m_pickupUI.SetActive(false);
                }
            }
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
                        m_pickupUI.transform.SetParent(null);
                        m_pickupUI.SetActive(false);
                        interactable.Interact(gameObject);
                    }
                }
            }
        }
    }
}

interface IInteractable
{
    void Interact(GameObject interactor);
}