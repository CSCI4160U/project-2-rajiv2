using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuControls : MonoBehaviour
{
    public static bool pressedLoadGame = false;
    public static bool pressedRestartLevel = false;

    public void GoToMainMenu()
    {
        Debug.Log("Opened MainMenu");
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadGame()
    {

        if(Globals.NoGameSaveDataFound())
        {
            HUDConsole._instance.Log("No save files available.\nCreate a new game", 5f);
            Debug.Log("No save files found. Create a new game\n");
        }
        else
        {
            Debug.Log("Loaded game");

            // bool used in SaveManager.cs to load scene where last saved
            pressedLoadGame = true;

            // Change to Loading Scene
            SceneManager.LoadScene("LoadingScene");
        }

        
    }

    public void ShowCreateNewGameWarning()
    {
        if(!Globals.NoGameSaveDataFound() && !Globals.NoSaveFilesFound())
        {
            HUDConsole._instance.Log("Warning: You will overwrite current save files if you continue!", 7f);
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(Globals.mainSceneName_Level1);
        pressedRestartLevel = true;
        Debug.Log("Trying to restart new game...");
    }

    public void ShutDownGame()
    {
        Application.Quit();
    }
}
