using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveData", menuName = "ScriptableObjects/WaveData")]
public class WaveData : ScriptableObject
{
    public string WaveName;
    public EnemySpawn[] enemy;
}

[System.Serializable]
public class EnemySpawn
{
    public GameObject EnemyGameObject;
    [Range(0, 6)]
    public int EnemySpawner;
    public int count;
    public float TimeBeetwenSpawn = 1;
}
