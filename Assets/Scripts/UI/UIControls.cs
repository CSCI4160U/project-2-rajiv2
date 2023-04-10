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
            TogglePauseMenu();
            ShowDeathScreen();
        }
    }

    public void ClosePauseMenu()
    {
        if (pauseMenu != null)
        {
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
            pauseMenuIsShown = !pauseMenuIsShown;
        }
    }

    public void ShowDeathScreen()
    {

        if (deathScreen != null)
        {
            deathScreen.GetComponent<Canvas>().enabled = deathScreenIsShown;
        }

        if (player.isDead)
        {
            deathScreenIsShown = true;
        }
        else
        {
            deathScreenIsShown = false;
        }
    }
}
