using UnityEngine;
using UnityEngine.InputSystem;

namespace SubnauticaClone
{
    [RequireComponent(typeof(Rigidbody))]
    public class CapsuleController : MonoBehaviour
    {
        [SerializeField] private float moveForce = 10f;
        [SerializeField] private float drag = 2f;
        [SerializeField] private float verticalForce = 1f;

        private Rigidbody rb;
        private Vector2 moveInput;
        private float verticalInput;
        private Transform cam;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.linearDamping = drag;
            rb.angularDamping = drag;

            cam = Camera.main != null ? Camera.main.transform : null;
        }

        private void OnMove(InputValue value)
        {
            moveInput = value.Get<Vector2>();
        }

        private void OnVertical(InputValue value)
        {
            verticalInput = value.Get<float>();
        }

        private void FixedUpdate()
        {
            if (cam == null) return;

            // Horizontal movement (WASD)
            Vector3 moveDir =
                cam.forward * moveInput.y +
                cam.right * moveInput.x;

            moveDir.Normalize();

            // Vertical movement (E/Q)
            Vector3 verticalDir = Vector3.up * verticalInput;

            Vector3 horizontalDir =
                cam.forward * moveInput.y +
                cam.right * moveInput.x;
            horizontalDir.Normalize();

            if (horizontalDir.sqrMagnitude > 0.001f)
                rb.AddForce(horizontalDir * moveForce, ForceMode.Acceleration);

            if (Mathf.Abs(verticalInput) > 0.001f)
                rb.AddForce(verticalDir * verticalForce, ForceMode.Acceleration);
        }
    }
}
