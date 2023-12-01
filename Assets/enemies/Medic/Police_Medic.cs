using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Pathfinding;
public class Police_Medic : Police_Variations
{
    [Header("Police Medic")]
    [SerializeField]
    protected float DistanceToHeal = 2;
    [SerializeField]
    protected Enemy EnemyToHeal;
    protected enum MedicState
    {        
        SearchEnemyToHeal,
        WalkingToHeal,
        Healing,
        Surrender,
    }
    [SerializeField]
    protected MedicState CurrentMedicState = MedicState.SearchEnemyToHeal;

    public Transform EnemyFuturePosition;

    //Heal stats
    [SerializeField]
    private float HowLongToHeal;
    [SerializeField]
    protected int HPFromHeal;


    [SerializeField]
    GameObject HealingEffect;

    public override void Awake()
    {
        base.Awake();
    }

    protected new void Start()
    {
        base.Start();

        EnemyFuturePosition = FuturePos();
    }

    public new void Update()
    {
        base.Update();

        float DistanceFromEnemy = 0;

        if (!EnemyToHeal)
        {
            if (Player_Script.PlayerInstance.transform.position.x - transform.position.x > 0)
                transform.localScale = new Vector3(1, 1, 1);

            else if (Player_Script.PlayerInstance.transform.position.x - transform.position.x < 0)
                transform.localScale = new Vector3(-1, 1, 1);
        }

        else if (EnemyToHeal.transform.position.x - transform.position.x > 0)
            transform.localScale = new Vector3(1, 1, 1);      

        else if (EnemyToHeal.transform.position.x - transform.position.x < 0)       
            transform.localScale = new Vector3(-1, 1, 1);
        

        if (EnemyToHeal)
            DistanceFromEnemy = Vector2.Distance(gameObject.transform.position, EnemyToHeal.transform.position);

        if(CurrentMedicState == MedicState.SearchEnemyToHeal)
        {

            if (!ReturnEnemyToHeal())
            {
                CurrentMedicState = MedicState.Surrender;
            }

            if (ReturnEnemyToHeal())
            {
                EnemyToHeal = ReturnEnemyToHeal();
                GetComponent<AIDestinationSetter>().target = EnemyToHeal.transform;
                CurrentMedicState = MedicState.WalkingToHeal;
            }
            return;
        }

        else if (CurrentMedicState == MedicState.Surrender)
        {
            GetComponent<AIPath>().canMove = false;

            gameObject.GetComponent<Animator>().SetBool("Surrender", true);

            if (ReturnEnemyToHeal())
            {
                gameObject.GetComponent<Animator>().SetBool("Surrender", false);
                EnemyToHeal = ReturnEnemyToHeal();
                GetComponent<AIDestinationSetter>().target = EnemyToHeal.transform;
                CurrentMedicState = MedicState.WalkingToHeal;
            }
            return;
        }


        else if (CurrentMedicState == MedicState.WalkingToHeal)
        {
            if (EnemyToHeal == null)
            {
                CurrentMedicState = MedicState.Surrender;
                return;
            }

            else if(EnemyToHeal != null)
            {
                GetComponent<AIPath>().canMove = true;
                Vector2.Distance(gameObject.transform.position, EnemyToHeal.transform.position);
                //GetComponent<AIDestinationSetter>().target = EnemyToHeal.transform;

                if(DistanceFromEnemy <= DistanceToHeal || EnemyToHeal.GetComponent<Collider2D>().IsTouching(gameObject.GetComponent<Collider2D>()))
                {
                    Debug.Log("Healing");
                    if (EnemyToHeal)
                    {
                        GetComponent<AIPath>().canMove = false;
                        CurrentMedicState = MedicState.Healing;
                        StartCoroutine(Heal());
                    }                
                }
            }
        }
    }

    protected IEnumerator Heal()
    {
        gameObject.GetComponent<Animator>().SetBool("Healing", true);
        yield return new WaitForSeconds(HowLongToHeal);
        CurrentMedicState = MedicState.WalkingToHeal;
        if (EnemyToHeal)
        {
            Destroy(Instantiate(HealingEffect, EnemyToHeal.transform.position, Quaternion.Euler(-90, 0, 0)), 2);
            EnemyToHeal.HealUnit(HPFromHeal);

            if (EnemyToHeal.GetCurrentHealth() >= EnemyToHeal.GetMaxHealth())
            {
                CurrentMedicState = MedicState.SearchEnemyToHeal;
            }
        }

        gameObject.GetComponent<Animator>().SetBool("Healing", false);
    }

    protected Enemy ReturnEnemyToHeal()
    {
        //Prioritize fighting units 
        //avoid dead units
        //Not full Hp units
        Enemy[] Enemies = FindObjectsOfType<Enemy>().Where(enemy => !enemy.Enemy_Death && enemy != GetComponent<Police_Medic>() && (enemy.GetCurrentHealth() < enemy.GetMaxHealth())).ToArray();
        //then medics
        if(Enemies.Length == 0) Enemies = FindObjectsOfType<Enemy>().Where(enemy => enemy != gameObject.GetComponent<Police_Medic>() && !enemy.Enemy_Death).ToArray();

        //Find the closest enemy
        float ClosestDistance = 100;
        Enemy ClosestEnemy = null;

        
        for (int i = 0; i < Enemies.Length; i++)
        {
            float CurrentDistance = Vector2.Distance(Enemies[i].transform.position, Player_Script.PlayerInstance.gameObject.transform.position);

                if (CurrentDistance < ClosestDistance)
                {
                    ClosestDistance = CurrentDistance;
                    ClosestEnemy = Enemies[i];
                }


        }
        

        return ClosestEnemy;
    }

    public override void Death()
    {
        //Add to base class loot
        base.Death();
        if (Enemy_Death) return;

        gameObject.GetComponent<Animator>().SetBool("Death", true);
        Destroy(gameObject, 1);
        GetComponent<AIPath>().maxSpeed = 0;
        Enemy_Death = true;
    }
}
