using System;
using SubnauticaClone;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Manages the overall game state, including pausing and unpausing.
/// </summary>
namespace SubnauticaClone
{
    public class GameManager : SingletonBase<GameManager>
    {
        public event Action<bool> OnPauseStateChanged;
        [SerializeField] private InputAction inputAction;

        protected override void Awake()
        {
            base.Awake();
            IsPaused = false;
        }

        private void OnEnable()
        {
            inputAction.performed += TogglePause;
            inputAction.Enable();
        }

        private void OnDisable()
        {
            inputAction.performed -= TogglePause;
            inputAction.Disable();
        }

        public bool IsPaused { get; private set; }

        public void TogglePause()
        {
            IsPaused = !IsPaused;
            Time.timeScale = IsPaused ? 0 : 1;
            
            if (IsPaused)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            OnPauseStateChanged?.Invoke(IsPaused);
        }

        private void TogglePause(InputAction.CallbackContext context)
        {
            TogglePause();
        }
    }
}