using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the UI for the pause menu, including showing/hiding it and handling button clicks.
/// </summary>
namespace SubnauticaClone
{
    public class PauseUI : MonoBehaviour
    {
        [SerializeField] private Button m_resumeButton;
        [SerializeField] private Button m_restartButton;
        [SerializeField] private GameObject m_pauseMenuUI;
        [SerializeField] private Button m_quitButton;
        
        private void Start()
        {
            GameManager.Instance.OnPauseStateChanged += SetPauseMenuVisibility;

            if (m_resumeButton != null)
            {
                m_resumeButton.onClick.AddListener(ResumeGame);
            }

            if (m_restartButton != null)
            {
                m_restartButton.onClick.AddListener(RestartGame);
            }

            if (m_quitButton != null)
            {
                m_quitButton.onClick.AddListener(QuitGame);
            }
        
            SetPauseMenuVisibility(GameManager.Instance.IsPaused);
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnPauseStateChanged -= SetPauseMenuVisibility;
                if (m_resumeButton != null)
                {
                    m_resumeButton.onClick.RemoveListener(ResumeGame);
                }
                if (m_restartButton != null)
                {
                    m_restartButton.onClick.RemoveListener(RestartGame);
                }
                if (m_quitButton != null)
                {
                    m_quitButton.onClick.RemoveListener(QuitGame);
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
            m_pauseMenuUI.SetActive(isPaused);
        }
    }
}