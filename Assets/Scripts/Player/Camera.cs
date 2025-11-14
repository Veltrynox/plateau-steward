using UnityEngine;
using UnityEngine.InputSystem;

namespace SubnauticaClone
{
    public class CameraLook : MonoBehaviour
    {
        [SerializeField] private Transform playerBody;
        [SerializeField] private float sensitivity = 100f;

        private float xRotation = 0f;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void OnLook(InputValue value)
        {
            Vector2 delta = value.Get<Vector2>();
            float mouseX = delta.x * sensitivity * Time.deltaTime;
            float mouseY = delta.y * sensitivity * Time.deltaTime;

            // pitch
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f); // limit vertical look

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            // yaw
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
}