using UnityEngine;
using System.IO;

public class Globals : MonoBehaviour
{
    public static string mainSceneName_Level1 = "MainScene_Level1";
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
