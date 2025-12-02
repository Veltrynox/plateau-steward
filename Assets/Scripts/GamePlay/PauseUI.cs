using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/// <summary>
/// Handles the UI for the pause menu, including showing/hiding it and handling button clicks.
/// </summary>
namespace SubnauticaClone
{
    public class PauseUI : MonoBehaviour
    {
        [SerializeField] private Button m_resumeButton;
        [SerializeField] private GameObject m_pauseMenuUI;
        
        private void Start()
        {
            GameManager.Instance.OnPauseStateChanged += SetPauseMenuVisibility;

            if (m_resumeButton != null)
            {
                m_resumeButton.onClick.AddListener(ResumeGame);
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
            }
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