using UnityEngine;
using UnityEngine.UI;

public class SettingsControls : MonoBehaviour
{
    public static AudioSource backgroundMusic = null;
    public Slider musicVolumeSlider = null;
    private string savePath;
    public static SettingsControls _instance;
    public SettingsData settingsData = null;

    private void Awake()
    {
        GameObject bm = GameObject.Find("BackgroundMusic");
        if (bm != null)
        {
            backgroundMusic = bm.GetComponent<AudioSource>();
        }

        _instance = this;

        musicVolumeSlider.value = musicVolumeSlider.maxValue;

        savePath = Application.persistentDataPath + "/saveData/";
        Debug.Log("Saving Settings to path: " + savePath);
        this.settingsData = new SettingsData();

        if (Globals.NoSaveFilesFound())
        {
            // creating settings save file
            SaveSettings();
        }

        LoadSettings();

        this.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        SaveSettings();
        LoadSettings();
    }

    private void Update()
    {
        if(AllSettingsExist())
        {
            UpdateVolume();
        }
    }

    
    private void UpdateVolume()
    {
        backgroundMusic.volume = musicVolumeSlider.value/100;
    }

    /*
     * Returns true if all existing settings are not null
     */
    private static bool AllSettingsExist()
    {
        return backgroundMusic != null;
    }


    [ContextMenu("Save Settings")]
    public void SaveSettings()
    {
        if (AllSettingsExist())
        {
            this.settingsData.musicVolume = backgroundMusic.volume;
            JSONLoaderSaver.SaveSettingsDataAsJSON(savePath, "settingsData.json", this.settingsData);
        }
    }

    [ContextMenu("Load Settings")]
    public void LoadSettings()
    {

        if (AllSettingsExist())
        {
            this.settingsData = JSONLoaderSaver.LoadSettingsDataFromJSON(savePath, "settingsData.json");

            if(settingsData != null)
            {
                // updating background music
                backgroundMusic.volume = settingsData.musicVolume;
                musicVolumeSlider.value = backgroundMusic.volume * 100;
            }
            

            // ...

            // TODO: other settings
        }
    }
}
