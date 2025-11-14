using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// Manages the game's pause functionality.
/// </summary>
public class Pause : MonoBehaviour
{
    private bool isPaused = false;

    [SerializeField] private InputAction inputAction;
    [SerializeField] private GameObject pauseMenuUI;

    private void OnEnable(){
        inputAction.Enable();
    }

    private void OnDisable(){
        inputAction.Disable();
    }

    private void Start(){
        inputAction.performed += ctx => TogglePause();
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            pauseMenuUI.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            pauseMenuUI.SetActive(false);
        }
    } 
}
