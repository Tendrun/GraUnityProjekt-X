using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGameEnableScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {

        //load file
        GameSettings GameSettings_ = new GameSettings();
        GameSettings_ = GameSettings_.LoadFile();

        //volume
        /*
        MusicVolume = GameSettings_.MusicVolume;
        SoundVolume = GameSettings_.SoundVolume;
        */

        //resolution
        Debug.Log("ResolutionIndex = " + GameSettings_.ResolutionIndex);
        int ResolutionIndex = GameSettings_.ResolutionIndex;
        Resolution[] resolutions = Screen.resolutions;

        //FullScreen
        bool FullScreen = GameSettings_.FullScreen;

        Screen.SetResolution(resolutions[ResolutionIndex].width, resolutions[ResolutionIndex].height, FullScreen, resolutions[ResolutionIndex].refreshRate);
        
    }
}
