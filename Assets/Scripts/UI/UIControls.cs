using UnityEngine;
using UnityEngine.SceneManagement;

public class UIControls : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private Player player;
    private bool pauseMenuIsShown = false;
    private bool deathScreenIsShown = false;

    private void Update()
    {
        // have access to ingame menu controls
        if(player != null)
        {
            if (!player.isDead)
            {
                deathScreenIsShown = false;
                TogglePauseMenu();
            }
            else
            {
                deathScreenIsShown = true;
            }
            ShowDeathScreen();
        }
    }

    public void ClosePauseMenu()
    {
        Debug.Log("Close Pause Menu");
        if (pauseMenu != null)
        {
            Cursor.lockState = CursorLockMode.Locked;
            pauseMenuIsShown = false;
            pauseMenu.GetComponent<Canvas>().enabled = pauseMenuIsShown;
        }
    }

    public void TogglePauseMenu()
    {
        if (pauseMenu != null)
        {
            pauseMenu.GetComponent<Canvas>().enabled = pauseMenuIsShown;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Toggled Pause Menu");
            if (pauseMenuIsShown)
            {
                pauseMenuIsShown = false;
                
                Cursor.lockState = CursorLockMode.Locked;

                // TODO: Pause all In-Game Activity
                FirstPersonController._instance.enabled = true;
            }
            else
            {
                pauseMenuIsShown = true;
                Cursor.lockState = CursorLockMode.None;

                // TODO: Unpause all In-Game Activity
                FirstPersonController._instance.enabled = false;
            }
        }

        
    }

    public void ShowDeathScreen()
    {
        if (deathScreen != null)
        {
            deathScreen.GetComponent<Canvas>().enabled = deathScreenIsShown;
        }
    }
}
