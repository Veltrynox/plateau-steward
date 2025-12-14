using UnityEngine;
using UnityEngine.InputSystem;

namespace SubnauticaClone
{
    [RequireComponent(typeof(Rigidbody))]
    public class Player : MonoBehaviour
    {
        private GameObject m_PlayerHUD;

        [Header("Movement Settings")]
        [SerializeField] private float moveForce = 15f;
        [SerializeField] private float waterDrag = 2f;
        [SerializeField] private float landDrag = 5f;
        [SerializeField] private float walkMultiplier = 2f;

        private Rigidbody rb;
        private Vector2 moveInput;
        private Transform cam;

        public void Construct(GameObject hud, Quaternion rotation, Vector3 position)
        {
            m_PlayerHUD = hud;
            transform.rotation = rotation;
            transform.position = position;
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            cam = Camera.main != null ? Camera.main.transform : null;

            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }

        private void OnMove(InputValue value)
        {
            moveInput = value.Get<Vector2>();
        }

        private void FixedUpdate()
        {
            if (cam == null) return;

            Vector3 moveDir;
            float currentForce = moveForce;

            if (transform.position.y < 0)
            {
                rb.linearDamping = waterDrag;
                rb.angularDamping = waterDrag;
                rb.useGravity = false;

                moveDir = cam.forward * moveInput.y + cam.right * moveInput.x;
            }
            else
            {
                rb.linearDamping = landDrag;
                rb.angularDamping = landDrag;
                rb.useGravity = true;

                Vector3 forwardFlat = cam.forward;
                Vector3 rightFlat = cam.right;

                forwardFlat.y = 0;
                rightFlat.y = 0;

                forwardFlat.Normalize();
                rightFlat.Normalize();

                moveDir = forwardFlat * moveInput.y + rightFlat * moveInput.x;

                currentForce *= walkMultiplier;
            }

            if (moveDir.sqrMagnitude > 0.001f)
            {
                moveDir.Normalize();
                rb.AddForce(moveDir * currentForce, ForceMode.Acceleration);
            }
        }
    }
}