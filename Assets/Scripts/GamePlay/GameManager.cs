using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            IsPaused = false;
            Time.timeScale = 1;
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

        public void GameOver()
        {
            Debug.Log("Game Over!");
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            OnPauseStateChanged?.Invoke(true);
        }

        internal void QuitGame()
        {
            throw new NotImplementedException();
        }

        internal void LoadScene()
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }
    }
}