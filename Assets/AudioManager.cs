using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    #region Singleton

    private static AudioManager _AudnioManagerInstance;
    public static AudioManager AudnioManagerInstance
    {
        get
        {
            if (_AudnioManagerInstance == null)
            {
                _AudnioManagerInstance = FindObjectOfType<AudioManager>();
                if (_AudnioManagerInstance == null && !AppQuitting)
                {
                    Debug.Log("Object is created");
                    GameObject newGO = new GameObject("AudnioManagerInstance");
                    _AudnioManagerInstance = newGO.AddComponent<AudioManager>();
                }
                return _AudnioManagerInstance;
            }
            return _AudnioManagerInstance;
        }
    }

    #endregion 

    [HideInInspector]
    public bool Detached { get; set; } = false;
    //appliaction Quitting
    public static bool AppQuitting = false;

    private GameSettings settings;

    [SerializeField]
    List<AudioClip> MusicList = new List<AudioClip>();
    [SerializeField]
    GameObject MusicGameObject;

    private void Awake()
    {
        if (AudnioManagerInstance != null && AudnioManagerInstance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _AudnioManagerInstance = this;
        }

    }


    private void Start()
    {
        if (SettingManager.SettingManager_)
            settings = SettingManager.SettingManager_.GetGamesSettings();
        else settings = new GameSettings().LoadFile();


        GameManager.GameManagerInstance.onpause += MusicPause;

        //Play music in sequence
        //if(MusicList[0])
        StartCoroutine(PlayMusicSequence());
    }

    protected void OnApplicationQuit()
    {
        GameManager.GameManagerInstance.onpause -= MusicPause;
        Detached = true;
        AppQuitting = true;
    }

    public void LoadSettings(GameSettings settings_)
    {
        settings = settings_;

        //Change music volume
        MusicGameObject.GetComponent<AudioSource>().volume = settings.MusicVolume;
    }

    public void PlaySound(Vector2 Pos, AudioClip clip)
    {

        GameObject obj = new GameObject("Sound " + clip);
        AudioSource Source = obj.AddComponent<AudioSource>();
        Source.transform.position = Pos;
        Source.clip = clip;
        Source.playOnAwake = false;
        Source.volume = settings.SoundVolume;
        Source.Play();


        Destroy(obj, clip.length);
    }

    private IEnumerator PlayMusicSequence()
    {
        //Vector2 Pos = Player_Script.PlayerInstance.transform.position;
        MusicGameObject = new GameObject("Music GameObject");
        AudioSource Source = MusicGameObject.AddComponent<AudioSource>();
        //Source.transform.position = Pos;
        Source.playOnAwake = false;
        Source.volume = settings.MusicVolume;

        for (int i = 0; i < MusicList.Count; i++)
        {
            Source.clip = MusicList[i];
            Source.Play();


            yield return new WaitForSeconds(MusicList[i].length);

            if (MusicList.Count - 1 <= i)
            {
                i = -1;
                Debug.Log("Restart");
            }
        }
    }


    public void MusicPause(bool Pause)
    {
        if (Pause)
        {
            MusicGameObject.GetComponent<AudioSource>().Pause();
            GetComponent<MonoBehaviour>().enabled = false;

        }

        else {
            MusicGameObject.GetComponent<AudioSource>().UnPause();
            GetComponent<MonoBehaviour>().enabled = true;
        }
    }

    public void PlayMusic()
    {
        MusicGameObject.GetComponent<AudioSource>().Play();
    }
}
