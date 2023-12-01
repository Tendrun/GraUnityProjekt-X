using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEngine.Events; 
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    #region Singleton
    private static MenuManager _MenuManagerInstance;
    public static MenuManager MenuManagerInstance
    {
        get
        {
            if(_MenuManagerInstance == null)
            {
                _MenuManagerInstance = FindObjectOfType<MenuManager>();
                if (_MenuManagerInstance == null && !AppQuitting)
                {
                    Debug.Log("Object is created");
                    GameObject newGO = new GameObject("MenuManager");
                    _MenuManagerInstance = newGO.AddComponent<MenuManager>();
                }
                return _MenuManagerInstance;
            }
            return _MenuManagerInstance;
        }
    }

    #endregion 

    private GameManager GameManagerObj;
    [SerializeField]
    static GameObject Settings_Menu;
    [SerializeField]
    GameObject CurrentMenu;
    [SerializeField]
    GameObject MenuPauseObj;
    GameManager Settings;
    public GameObject eventSystem;

    //appliaction Quitting
    public static bool AppQuitting = false;

    //Settings
    [SerializeField]
    private GameObject SettingsMenu;

    private void Awake()
    {
        if (MenuManagerInstance != null && MenuManagerInstance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _MenuManagerInstance = this;
        }
    }

    private void Start()
    {
        GameManagerObj = GameManager.GameManagerInstance;
    }


    private void OnApplicationQuit()
    {
        AppQuitting = true;
    }

    private void Update()
    {
        //Pause Game
        if (Input.GetKeyDown(KeyCode.Escape) && !CurrentMenu)
        {
            if (!GameManagerObj.Pasued) GameManagerObj.Stop();
                AddMenu(MenuPauseObj);

            //Disable Cursor
            FindObjectOfType<Cursor_Script>().EnableCursor();
        }

        //Resmue Game
        else if (Input.GetKeyDown(KeyCode.Escape) && Settings_Menu != null)
        {
            HideSettings();
        }
        
        //Delete Menu
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(CurrentMenu);
            ResumeGame();
        }
        
    }

    public void TurnOnManger()
    {
        Debug.Log("Hello There General Kenobi");
    }

    //Settings
    public void ShowSettings()
    {
        //To not have another copy of settings
        if (Settings_Menu) return;

        CreateSettings();
    }

    private void CreateSettings()
    {
        Settings_Menu = Instantiate(SettingsMenu);
        Debug.Log(Settings_Menu);
    }

    public void HideSettings()
    {
        SettingManager settingManager = Settings_Menu.GetComponent<SettingManager>();
        settingManager.DestroyAndSaveFile();
    }



    //Menu

    public void AddMenu(GameObject Menu)
    {
        CurrentMenu = Instantiate(Menu);
    }

    

    //Game Options
    private void ResumeGame()
    {
        //Disable Cursor
        FindObjectOfType<Cursor_Script>().DisableCursor();

        //PlayMusic
        //AudioManager.AudnioManagerInstance.PlayMusic();

        Destroy(CurrentMenu);
        GameManagerObj.Resume();
    }

    public static void Button_ResumeGame()
    {
        FindObjectOfType<MenuManager>().ResumeGame();
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    public void LoadSurvior()
    {
        SceneManager.LoadScene("Survivor");
    }

    public void RestartSurvior()
    {
        FindObjectOfType<GameManager>().Restart();
        SceneManager.LoadScene("Survivor");
    }

    public void GoBackToMenu()
    {
        //Restart dziala dobrze dla tego przypadku
        FindObjectOfType<GameManager>().Restart();

        //Enable Cursor
        FindObjectOfType<Cursor_Script>().EnableCursor();

        SceneManager.LoadScene("Menu");
    }
}