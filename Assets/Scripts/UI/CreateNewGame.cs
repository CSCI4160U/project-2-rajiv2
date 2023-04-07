using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CreateNewGame : MonoBehaviour
{

    public static bool pressedCreateGame = false;
    public static string newUserName;

    private TMP_InputField inputField = null;

    private void Start()
    {
        GameObject enterUserName = GameObject.Find("EnterUserName");
        newUserName = string.Empty;

        if (enterUserName != null)
        {
            inputField = enterUserName.GetComponent<TMP_InputField>();
        }
    }

    private void Update()
    {
        if (inputField != null)
        {
            inputField.onValueChanged.AddListener(delegate { UserNameEntered(); });
        }
    }


    private void UserNameEntered()
    {
        newUserName = inputField.text;
    }
    public void GoToLevel1()
    {
        // if save file already exists
        // are you sure you would like to overwrite your saved game?

        // if confirmed
        // go to level 1
        if (newUserName != string.Empty)
        {
            Debug.Log("Created a new game");
            if (Globals.mainSceneName_Level1 != string.Empty)
            {
                SceneManager.LoadScene(Globals.mainSceneName_Level1);
                pressedCreateGame = true;
                Debug.Log("Trying to create new game...");
            }
            else
            {
                Debug.Log("Give a value to the 'First Level Name' in the Globals script!\n(Assets/Scripts/Globals/Globals.cs)");
            }
        }
        else
        {
            Debug.Log("Must Enter Username to continue.");
        }

        //SceneManager.LoadScene("MainScene");
    }

}
