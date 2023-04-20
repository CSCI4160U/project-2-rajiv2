using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsControls : MonoBehaviour
{
    //public static AudioSource backgroundMusic = null;
    public Slider musicVolumeSlider = null;
    public Slider sfxVolumeSlider = null;
    private string savePath;
    public static SettingsControls _instance;
    public SettingsData settingsData = null;
    //[SerializeField] private MusicStorage backgroundMusicStorage = null;
    [SerializeField] private AudioMixer globalAudioMixer = null;

    private void Awake()
    {
        //if (backgroundMusicStorage != null)
        //{
        //    backgroundMusic = backgroundMusicStorage.audioSource;
        //}

        _instance = this;

        musicVolumeSlider.value = musicVolumeSlider.maxValue;
        sfxVolumeSlider.value = sfxVolumeSlider.maxValue;

        savePath = Application.persistentDataPath + "/saveData/";
        Debug.Log("Saving Settings to path: " + savePath);
        this.settingsData = new SettingsData();

        if (Globals.NoSaveFilesFound())
        {
            // creating settings save file
            SaveSettings();
        }


        LoadSettings();
    }

    private void FixedUpdate()
    {
        if (AllSettingsExist())
        {
            SaveSettings();
            LoadSettings();
        }
    }

    private void Update()
    {
        if (AllSettingsExist())
        {
            UpdateVolume();
        }
    }


    private void UpdateVolume()
    {
        //backgroundMusic.volume = musicVolumeSlider.value/100;
        globalAudioMixer.SetFloat("Music", Mathf.Log10(musicVolumeSlider.value)*20);
        globalAudioMixer.SetFloat("SFX", Mathf.Log10(sfxVolumeSlider.value)*20);
    }



    /*
     * Returns true if all existing settings are not null
     */
    private bool AllSettingsExist()
    {
        return globalAudioMixer != null;
    }


    [ContextMenu("Save Settings")]
    public void SaveSettings()
    {

        this.settingsData.musicVolume = musicVolumeSlider.value*100;

        this.settingsData.sfxVolume = sfxVolumeSlider.value*100;

        JSONLoaderSaver.SaveSettingsDataAsJSON(savePath, "settingsData.json", this.settingsData);
    }

    [ContextMenu("Load Settings")]
    public void LoadSettings()
    {

        this.settingsData = JSONLoaderSaver.LoadSettingsDataFromJSON(savePath, "settingsData.json");

        if (settingsData != null)
        {
            // updating background music volume
            globalAudioMixer.SetFloat("Music", musicVolumeSlider.value);
            musicVolumeSlider.value = settingsData.musicVolume/100;

            // updating sfx volume
            globalAudioMixer.SetFloat("SFX", sfxVolumeSlider.value);
            sfxVolumeSlider.value = settingsData.sfxVolume/100;

            // ...

            // TODO: other settings (eg. Input Sensitivity)
        }
    }
}
