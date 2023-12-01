using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class SettingManager : MonoBehaviour
{
    public static SettingManager SettingManager_;
    private GameSettings GameSettings_;


    float SoundVolume;
    [SerializeField]
    Slider SoundVolumeSlider;

    float MusicVolume;
    [SerializeField]
    Slider MusicVolumeSlider;



    [SerializeField]
    TMPro.TMP_Dropdown DropdownResolution;
    int ResolutionIndex;
    Resolution[] resolutions;

    bool FullScreen;
    [SerializeField]
    Toggle FullScreenBool;

    private void Start()
    {
        SettingManager_ = GetComponent<SettingManager>();
        GameSettings_ = new GameSettings(SoundVolume, MusicVolume, ResolutionIndex, FullScreen);


        //Load File
        LoadSettings();
    }

    //SetLoadedValuesToGame
    private void LoadSettings()
    {
        //load file
        GameSettings_ = GameSettings_.LoadFile();

        //volume
        SoundVolume = GameSettings_.SoundVolume;
        SoundVolumeSlider.value = SoundVolume;

        MusicVolume = GameSettings_.MusicVolume;
        MusicVolumeSlider.value = MusicVolume;


        //resolution
        ResolutionIndex = GameSettings_.ResolutionIndex;

        List<string> options = new List<string>();

        //FullScreen
        FullScreen = GameSettings_.FullScreen;

        resolutions = Screen.resolutions;
        for (int i = 0; i < resolutions.Length; i++)
        {
            options.Add(resolutions[i].width.ToString()+ " x " + resolutions[i].height.ToString() + " " + resolutions[i].refreshRate + "Hz");
        }

        Screen.SetResolution(resolutions[ResolutionIndex].width, resolutions[ResolutionIndex].height, FullScreen, resolutions[ResolutionIndex].refreshRate);

        //FullScreen this is has to be here, otherwise it wont compile
        FullScreenBool.isOn = FullScreen;


        DropdownResolution.AddOptions(options);
        DropdownResolution.SetValueWithoutNotify(ResolutionIndex);
    }

    //volume
    public void SetGeneralVolume(float Volume)
    {
        SoundVolume = Volume;
    }

    public void SetMusicVolume(float Volume)
    {
        MusicVolume = Volume;
    }

    //screen    
    public void SetResolutionIndex(int value)
    {
        ResolutionIndex = value;
        Screen.SetResolution(resolutions[value].width, resolutions[value].height, FullScreen, resolutions[value].refreshRate);
    }

    public void SetFullScreen(bool value)
    {
        FullScreen = value;
        Screen.SetResolution(resolutions[ResolutionIndex].width, resolutions[ResolutionIndex].height, FullScreen, resolutions[ResolutionIndex].refreshRate);
    }


    public void DestroyAndSaveFile()
    {
        GameSettings_ = new GameSettings(SoundVolume, MusicVolume, ResolutionIndex, FullScreen);
        GameSettings_.SaveFile();
        SetInGameSettings();

        Destroy(gameObject);
    }

    public void SetInGameSettings()
    {
        ResolutionIndex = DropdownResolution.value;

        GameSettings_ = new GameSettings(SoundVolume, MusicVolume, ResolutionIndex, FullScreen);
        GameSettings_.SaveFile();

        //Change volume settings
        if(AudioManager.AudnioManagerInstance)
            AudioManager.AudnioManagerInstance.LoadSettings(GameSettings_);

    }

    public GameSettings GetGamesSettings()
    {
        return GameSettings_;
    }
}

