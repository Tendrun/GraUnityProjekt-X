using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton

    private static GameManager _GameManagerInstance;
    public static GameManager GameManagerInstance
    {
        get
        {
            if (_GameManagerInstance == null)
            {
                _GameManagerInstance = FindObjectOfType<GameManager>();
                if (_GameManagerInstance == null && !AppQuitting)
                {
                    Debug.Log("Object is created");
                    GameObject newGO = new GameObject("GameManagerObj");
                    _GameManagerInstance = newGO.AddComponent<GameManager>();
                }
                return _GameManagerInstance;
            }
            return _GameManagerInstance;
        }
    }


    #endregion 

    [SerializeField]
    private GameObject GamoeOverObject;
    [SerializeField]
    private GameObject[] DropLoot;
    Player_Script Player;
    [SerializeField]
    private int EnemiesDied = 0;
    private Vector2 LastEnemyKilled;
    [SerializeField]
    private Text ScoreText;


    private int CountPassive;
    private int AllPoints;

    //Pasue
    public bool Pasued = false;
    private float fixedDeltaTime;
    public delegate void OnPasue(bool Pause);
    public event OnPasue onpause;

    //UI
    private Stack<Canvas> Menucanvas;

    //appliaction Quitting
    public static bool AppQuitting = false;

    private void Awake()
    {
        if (GameManagerInstance != null && GameManagerInstance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _GameManagerInstance = this;
        }

        this.fixedDeltaTime = Time.fixedDeltaTime;      
    }

    private void OnApplicationQuit()
    {
        AppQuitting = true;
    }

    private void Start()
    {

        //Check for MenuManager
        if (FindObjectOfType<MenuManager>() == null)
        {
            MenuManager.MenuManagerInstance.TurnOnManger();
            Debug.LogWarning("THERE IS NO MenuManager");
        }

        //Check for Player
        if (FindObjectOfType<Player_Script>() == null)
        {
            Debug.LogWarning("THERE IS NO PLAYER");
            return;
        }
        Player = Player_Script.PlayerInstance;
    }

    public void SetScoreText(Text ScoreText)
    {
        this.ScoreText = ScoreText;
    }

    public void Stop()
    {
        Pasued = true;
        onpause?.Invoke(true);
        Time.timeScale = 0f;
        Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
    }
    public void Resume()
    {
        Pasued = false;
        onpause?.Invoke(false);
        Time.timeScale = 1f;
        Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
    }

    public void Restart()
    {
        Pasued = false;
        Time.timeScale = 1f;
        Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
    }

    public void ENDGAME()
    {
        Instantiate(GamoeOverObject, new Vector3(0, 0, 0), Quaternion.identity);
    }


    public void EnemyDied()
    {
        EnemiesDied++;
        CountPassive++;
    }

    public bool On10EnemiesDeaths()
    {
        if (CountPassive >= 10)
        {
            CountPassive = 0;
            return true;
        }
        return false;
    }

    public bool On1EnemiesDeaths()
    {
        if (CountPassive >= 1)
        {
            CountPassive = 0;
            return true;
        }
        return false;
    }

    public void SetLastEnemyKilled(GameObject obj)
    {
        LastEnemyKilled = (Vector2)obj.transform.position;
    }

    public Vector2 GetLastEnemyKilled()
    {
        return LastEnemyKilled;
    }

    public void AddScorePoints(int points)
    {
        AllPoints += points;
        ScoreText.text = "Score = " + AllPoints;
    }
}

public interface IPause
{
    /*
    protected void Start()
    {
        GameManager.GameManagerInstance.onpause += UnitPause;
    }

    private void OnApplicationQuit()
    {
        GameManager.GameManagerInstance.onpause -= UnitPause;
        Detached = true;
    }

    protected void OnDestroy()
    {
        if (!Detached) GameManager.GameManagerInstance.onpause -= UnitPause;
    }
    */

    public bool Detached { get; set; }
    void UnitPause(bool Pause);
}

