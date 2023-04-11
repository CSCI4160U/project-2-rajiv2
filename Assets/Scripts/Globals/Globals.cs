using UnityEngine;
using System.IO;

public class Globals : MonoBehaviour
{
    public static string mainScene = "DoorWayRoom";
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
}
