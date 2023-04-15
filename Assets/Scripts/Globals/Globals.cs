using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class Globals : MonoBehaviour
{
    public static string mainScene = "MainScene";
    public static string mainMenuScene = "MainMenu";
    public static string loadingScene = "LoadingScene";
    public static string savePath = Application.persistentDataPath + "/saveData/";

    public static bool NoSaveFilesFound()
    {
        return (new DirectoryInfo(savePath)).GetFiles().Length == 0;
    }

    public static bool NoGameSaveDataFound()
    {
        return (new DirectoryInfo(savePath)).GetFiles().Length == 1;
    }

    public static void PauseAllMovementInGame()
    {
        PlayerShoot._instance.enabled = false;
        //FirstPersonController._instance.enabled = false;
    }

    public static void ResumeAllMovementInGame()
    {
        PlayerShoot._instance.enabled = true;
        //FirstPersonController._instance.enabled = true;
    }
}
