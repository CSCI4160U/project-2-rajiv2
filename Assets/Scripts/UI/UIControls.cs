using UnityEngine;

public class UIControls : MonoBehaviour
{
    private bool pauseMenuIsShown = false;
    private bool deathScreenIsShown = false;
    private GameObject pauseMenu;

    private void Awake()
    {
        pauseMenu = GameObject.Find("PauseMenu");
    }

    private void Update()
    {
        TogglePauseMenu();
        ShowDeathScreen();
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
        GameObject deathScreen = GameObject.Find("DeathScreen");


        if (deathScreen != null)
        {
            deathScreen.GetComponent<Canvas>().enabled = deathScreenIsShown;
        }

        if (this.GetComponent<Player>().isDead)
        {
            deathScreenIsShown = true;
        }
        else
        {
            deathScreenIsShown = false;
        }
    }
}
