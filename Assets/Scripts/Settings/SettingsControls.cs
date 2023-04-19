using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsControls : MonoBehaviour
{
    public static AudioSource backgroundMusic = null;
    public Slider musicVolumeSlider = null;
    public Slider sfxVolumeSlider = null;
    private string savePath;
    public static SettingsControls _instance;
    public SettingsData settingsData = null;
    [SerializeField] private MusicStorage backgroundMusicStorage = null;
    [SerializeField] private AudioMixer sfxAudioMixer = null;

    private void Awake()
    {
        if (backgroundMusicStorage != null)
        {
            backgroundMusic = backgroundMusicStorage.audioSource;
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
        sfxAudioMixer.SetFloat("master", sfxVolumeSlider.value/100);
    }



    /*
     * Returns true if all existing settings are not null
     */
    private bool AllSettingsExist()
    {
        return backgroundMusic != null && sfxAudioMixer != null;
    }


    [ContextMenu("Save Settings")]
    public void SaveSettings()
    {
        if (AllSettingsExist())
        {
            this.settingsData.musicVolume = backgroundMusic.volume;
            float sfxVolume;
            sfxAudioMixer.GetFloat("master", out sfxVolume);
            this.settingsData.sfxVolume = sfxVolume;
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
                // updating background music volume
                backgroundMusic.volume = settingsData.musicVolume;
                sfxAudioMixer.SetFloat("master", settingsData.musicVolume);
                musicVolumeSlider.value = backgroundMusic.volume * 100;

                // updating sfx volume
                float sfxVolume;
                sfxAudioMixer.GetFloat("master", out sfxVolume);
                sfxVolumeSlider.value = sfxVolume;

                // ...

                // TODO: other settings (eg. Input Sensitivity)
            }



        }
    }
}
