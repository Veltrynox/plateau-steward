using UnityEngine;
using UnityEngine.InputSystem;

namespace SubnauticaClone
{
    /// <summary>
    /// Manages the player's movement and interaction.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class Player : MonoBehaviour
    {
        private GameObject m_PlayerHUD;

        public void Construct(GameObject hud, Quaternion rotation, Vector3 position)
        {
            m_PlayerHUD = hud;
            transform.rotation = rotation;
            transform.position = position;
        }

        [SerializeField] private float moveForce = 10f;
        [SerializeField] private float drag = 2f;
        // [SerializeField] private float verticalForce = 1f;

        private Rigidbody rb;
        private Vector2 moveInput;
        // private float verticalInput;
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

        // private void OnVertical(InputValue value)
        // {
        //     verticalInput = value.Get<float>();
        // }

        private void FixedUpdate()
        {
            if (cam == null) return;

            // Horizontal movement (WASD)
            Vector3 moveDir =
                cam.forward * moveInput.y +
                cam.right * moveInput.x;

            moveDir.Normalize();

            // Vertical movement (E/Q)
            // Vector3 verticalDir = Vector3.up * verticalInput;

            Vector3 movementDirection =
                cam.forward * moveInput.y +
                cam.right * moveInput.x;
            movementDirection.Normalize();

            if (movementDirection.sqrMagnitude > 0.001f)
                rb.AddForce(movementDirection * moveForce, ForceMode.Acceleration);

            // if (Mathf.Abs(verticalInput) > 0.001f)
            //     rb.AddForce(verticalDir * verticalForce, ForceMode.Acceleration);
        }
    }
}
