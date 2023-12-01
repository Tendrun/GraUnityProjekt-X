using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour
{
    public enum SpawnState { SPAWNING, WAITING, COUNTING, END, SHOP};

    public GameObject[] Spawners;


    [System.Serializable]
    public class WavePhases
    {
        public string WavePhase;
        public WaveData[] waves;
    }

    public WavePhases[] PhaseNumber;
    private int NextWave = 0;
    private int NextPhase = 0;
    private int NumberOfWaves = 0;

    public float timeBetweenWaves = 5f;
    public float waveCountdown;

    private float searchCountdown = 1f;

    [SerializeField]
    private SpawnState state = SpawnState.COUNTING;


    [Header("Shop")]
    [SerializeField]
    int ShopInterval;
    int ShopCounter = 0;
    [SerializeField]
    GameObject[] Sellers;
    [SerializeField]
    Transform Seller_Place;
    GameObject CurrentSeller;

    [Header("UI")]
    [SerializeField]
    Text TimeText;

    private void Awake()
    {
        NumberOfWaves = CountWaves();
    }

    private void Start()
    {
        waveCountdown = timeBetweenWaves;
    }

    private void Update()
    {
        if(state == SpawnState.WAITING)
        {
            if (!EnemyIsAlive())
            {
                WaveCompleted();
                return;
            }

            else 
            {
                return;
            }
        }

        if (state == SpawnState.SHOP)
        {
            return;
        }

        if (waveCountdown <= 0)
        {
            TimeText.text = null;

            if (state == SpawnState.END)
            {
                Debug.Log("You have completed all waves GJ summoner");
            }

            else if (state != SpawnState.SPAWNING)
            {
                if (NextWave <= 9)
                {
                    StartCoroutine(SpawnWave(PhaseNumber[NextPhase]));
                }
                else if (NextWave == 10)
                {
                    Debug.Log("Next Phase");
                    NextPhase++;
                    NextWave = 0;
                }
            }         
        }

        else
        {
            waveCountdown -= Time.deltaTime;
            TimeText.text = Mathf.RoundToInt(waveCountdown).ToString();
        }
    }

    public void EndShoping()
    {
        state = SpawnState.COUNTING;
        waveCountdown = timeBetweenWaves;
        ShopCounter = 0;

        Destroy(CurrentSeller);
    }

    bool EnemyIsAlive()
    {
        searchCountdown -= Time.deltaTime;
        if(searchCountdown <= 0f)
        {
            searchCountdown = 2f;

            if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
            {
                return false;
            }
        }
     
        return true;
    }

    void WaveCompleted()
    {
        state = SpawnState.COUNTING;
        waveCountdown = timeBetweenWaves;

        if(NextWave + 1 + NextPhase * 10 >= NumberOfWaves)
        {
            ShopCounter = 0;
            state = SpawnState.END;
        }

        else
        {
            ShopCounter++;
            NextWave++;
        }

        if(ShopCounter == ShopInterval)
        {
            state = SpawnState.SHOP;

            //Spawn Seller
            TimeText.text = "SHOP";
            int rand = Random.Range(0, Sellers.Length);
            CurrentSeller = Instantiate(Sellers[rand], Seller_Place.transform.position, Sellers[rand].transform.rotation);
        }
    }

    public int CountWaves()
    {
        int NumberOfWaves = 0;

        for(int i = 0; i < PhaseNumber.Length; i++)
        {
            for (int a = 0; a < PhaseNumber[i].waves.Length; a++)
            {
                NumberOfWaves++;
            }
        }

        return NumberOfWaves;
    }


    IEnumerator SpawnWave (WavePhases _wave)
    {
        state = SpawnState.SPAWNING;

        //Random Wave

        int RandomWave = Random.Range(0, _wave.waves.Length);        
        Debug.Log("RandomWave = " + RandomWave);

            for (int a = 0; a < _wave.waves[RandomWave].enemy.Length; a++)
            {
                for (int i = 0; i < _wave.waves[RandomWave].enemy[a].count; i++)
                {
                    Debug.Log("index");
                    Spawners[_wave.waves[RandomWave].enemy[a].EnemySpawner].GetComponent<Spawner>().Enemies.Add(_wave.waves[RandomWave].enemy[a]);
                }
            }

            for (int i = 0; i < Spawners.Length; i++)
            {
                Spawners[i].GetComponent<Spawner>().StartSpawning();
            }

            //Wait until all enemies are spawned

        state = SpawnState.WAITING;

        yield break;
    }
}
