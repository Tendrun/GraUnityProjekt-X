using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    GameObject EnemySpawner;

    public List<EnemySpawn> Enemies;
    public float TimeBeetwenSpawn = 0.1f;


    public void StartSpawning()
    {
        StartCoroutine(SpawnWave());
    }


    IEnumerator SpawnWave()
    {
        for (int i = 0; i < Enemies.Count; i++)
        {
            SpawnEnemy(Enemies[i].EnemyGameObject);
            yield return new WaitForSeconds(Enemies[i].TimeBeetwenSpawn);
            if (Enemies.Count - 1 == i)
            {
                Debug.Log("Last index");
                Enemies.Clear();
            }
        }
        
        yield break;
    }

    void SpawnEnemy(GameObject _enemy)
    {
        //int RandomNumber = Random.Range(0, Spawners.Length);

        Transform Enemy = Instantiate(_enemy.transform, new Vector2(EnemySpawner.transform.position.x, EnemySpawner.transform.position.y), Quaternion.identity);
        StartCoroutine(SpawnProtection(Enemy));
    }

    IEnumerator SpawnProtection(Transform obj)
    {

        obj.GetComponent<Collider2D>().isTrigger = true;

        yield return new WaitForSeconds(1);

        obj.GetComponent<Collider2D>().isTrigger = false;
    }
}
