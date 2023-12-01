using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Pathfinding;

public abstract class Enemy : Unit
{
    [Header("Enemy Stats")]
    public GameObject BloodGameObject;
    [SerializeField]
    protected Loot loot;

    public bool Enemy_Death;
    public Transform target;

    protected bool OnDeathBool = false;

    [Header("Effects")]
    [SerializeField]
    protected bool stunned;

    [SerializeField]
    private int Score;
    private GameManager GameManagerInstance;



    public new virtual void Awake()
    {
        base.Awake();
    }

    public new virtual void Start()
    {
        base.Start();

        GetComponent<AIPath>().maxSpeed = CurrentSpeed;
        target = Player_Script.PlayerInstance.transform;
        GameManagerInstance = GameManager.GameManagerInstance;
    }

    //Create Target
    public Transform FuturePos()
    {
        GameObject _target = new GameObject("FuturePosition");
        _target.transform.parent = gameObject.transform;
        _target.transform.position = gameObject.transform.position;
        return _target.transform;
    }

    /*
    public override void CountSpeed()
    {
        //base.CountSpeed();

        GetComponent<AIPath>().maxSpeed = CurrentSpeed;
    }
    */

    [SerializeField]
    GameObject Damage_obj;
    public override void Damage(int damage)
    {
        CurrentHealth -= damage;
        previousHP -= damage;

        
        float y = GetComponent<BoxCollider2D>().size.y / 2;
        GameObject obj = Instantiate(Damage_obj, new Vector2(transform.position.x, transform.position.y + y), Damage_obj.transform.rotation);
        obj.GetComponentInChildren<TextMeshProUGUI>().text = damage.ToString();

        Destroy(obj, 0.25f);
        Debug.Log(damage);
        

        if (CurrentHealth <= 0)
        {
            Death();
        }

        GameObject blood = Instantiate(BloodGameObject, transform.position, Quaternion.identity);
        StartCoroutine(EnemyColorChange());

        Destroy(blood, 2);
    }

    IEnumerator EnemyColorChange()
    {
        SpriteRenderer spr = gameObject.GetComponent<SpriteRenderer>();
        for (int i = 0; i < 3; i++)
        {
            if (Enemy_Death) yield break;
            spr.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spr.color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }
        spr.color = Color.white;
    }

    public virtual void Death()
    {
        if (Enemy_Death) return;

        //Disable all colliders
        Collider2D[] coll = GetComponents<Collider2D>();
        for (int i = 0; i < coll.Length; i++)
        {
            coll[i].enabled = false;
        }

        //GameMangers things
        GameManagerInstance.AddScorePoints(Score);
        GameManagerInstance.SetLastEnemyKilled(gameObject);
        GameManagerInstance.EnemyDied();
        if (!Detached) GameManager.GameManagerInstance.onpause -= UnitPause;
        if (loot != null) loot.DropLoot(gameObject);
    }

    public int GetCurrentHealth()
    {
        return CurrentHealth;
    }

    public int GetMaxHealth()
    {
        return MaxHealth;
    }
}
