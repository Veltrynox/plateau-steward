using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the UI for the pause menu, including showing/hiding it and handling button clicks.
/// </summary>
namespace SubnauticaClone
{
    public class PauseUI : MonoBehaviour
    {
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private GameObject pauseMenuUI;
        [SerializeField] private Button quitButton;
        
        private void Start()
        {
            GameManager.Instance.OnPauseStateChanged += SetPauseMenuVisibility;

            if (resumeButton != null)
            {
                resumeButton.onClick.AddListener(ResumeGame);
            }

            if (restartButton != null)
            {
                restartButton.onClick.AddListener(RestartGame);
            }

            if (quitButton != null)
            {
                quitButton.onClick.AddListener(QuitGame);
            }
        
            SetPauseMenuVisibility(GameManager.Instance.IsPaused);
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnPauseStateChanged -= SetPauseMenuVisibility;
                if (resumeButton != null)
                {
                    resumeButton.onClick.RemoveListener(ResumeGame);
                }
                if (restartButton != null)
                {
                    restartButton.onClick.RemoveListener(RestartGame);
                }
                if (quitButton != null)
                {
                    quitButton.onClick.RemoveListener(QuitGame);
                }
            }
        }

        private void QuitGame()
        {
            GameManager.Instance.QuitGame();
        }

        private void RestartGame()
        {
            GameManager.Instance.LoadScene();
        }

        private void ResumeGame()
        {
            GameManager.Instance.TogglePause();
        }

        private void SetPauseMenuVisibility(bool isPaused)
        {
            pauseMenuUI.SetActive(isPaused);
        }
    }
}