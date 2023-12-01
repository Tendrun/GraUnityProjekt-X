using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class Police_Scout : Police_Shooting
{
    //NEW 

    public Transform EnemyFuturePosition;

    public float MaxDistanceFromPlayerX;
    public float MaxDistanceFromPlayerY;

    public float MinDistanceFromPlayerX;
    public float MinDistanceFromPlayerY;

    public Vector3 DistanceFromPlayer;
    public float DistanceToShootoffset = 1;



    protected Rigidbody2D rb;

    [Header("Police Scout")]
    [SerializeField]
    int AmountOfBullets;
    [SerializeField]
    float ReloadTime;
    bool Reloading = false;
    [SerializeField]
    Scout_State State = Scout_State.Shooting;
    enum Scout_State
    {
        Shooting,
        Reloading,
    }
    [SerializeField]
    LayerMask Walls;

    public override void Awake()
    {
        base.Awake();

        float RandX = Random.Range(MinDistanceFromPlayerX, MaxDistanceFromPlayerX);
        float RandY = Random.Range(MinDistanceFromPlayerY, MaxDistanceFromPlayerY);

        float NegX = Random.Range((float)0, (float)1);
        float NegY = Random.Range((float)0, (float)1);

        bool NegatX = NegX > 50 ? false : true;
        bool NegatY = NegY > 50 ? false : true;


        Vector2 Dis = new Vector2(RandX, RandY);

        if (DistanceToShoot < Vector2.Distance(new Vector2(RandX, RandY), new Vector2(0, 0)))
        {
            GetRandomVector2();
        }

        DistanceFromPlayer = new Vector3(RandX, RandY, 0);
    }



    protected Vector2 GetRandomVector2()
    {
        float RandX = Random.Range(MinDistanceFromPlayerX, MaxDistanceFromPlayerX);
        float RandY = Random.Range(MinDistanceFromPlayerY, MaxDistanceFromPlayerY);

        Vector2 Dis_ = new Vector2(RandX, RandY);

        if (DistanceToShoot < Vector2.Distance(new Vector2(RandX, RandY), new Vector2(0, 0)) + DistanceToShootoffset)
        {
            GetRandomVector2();
        }

        return Dis_;
    }

    public override void Start()
    {
        base.Start();

        rb = GetComponent<Rigidbody2D>();


        EnemyFuturePosition.position = target.transform.position + DistanceFromPlayer;
        //GetComponent<AIDestinationSetter>().target = EnemyFuturePosition;

        // Add yourself to list
    }

    float time = 0;
    float x, y;

    float Revers = 1;

    public new void Update()
    {
        base.Update();

        //If meets wall revers path
        //FuturePositionIsWall();

        if (Vector2.Distance(gameObject.transform.position, EnemyFuturePosition.position) < 3||
            FuturePositionIsWall())
            time += (CurrentSpeed / 2 * Time.deltaTime) * Revers;

        x = target.transform.position.x + (DistanceFromPlayer.x) * Mathf.Cos(time);
        y = target.transform.position.y + (DistanceFromPlayer.y) * Mathf.Sin(time);

        EnemyFuturePosition.position = new Vector2(x, y);
    }

    //This code doesnt function well, too many bad situations
    /*
    void FuturePositionIsWall()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(x, y), 1);

        foreach (Collider2D collider in colliders)
        {
            if(((1 << collider.gameObject.layer) & Walls) != 0)
            {
                Revers *= -1;
                return;
            }
        }
    }
    */

    bool FuturePositionIsWall()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(x, y), 1);

        foreach (var collider in colliders)
        {
            if (((1 << collider.gameObject.layer) & Walls) != 0)
            {
                return true;
            }
        }

        return false;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (!Enemy_Death && State == Scout_State.Shooting)
        {
            //Shooting
            Gun();
        }

        else if (State == Scout_State.Reloading && !Reloading)
        {
            StartCoroutine(Reload());
        }
    }


    public override void Death()
    {
        //Add to base class loot
        base.Death();
        if (Enemy_Death) return;

        gameObject.GetComponent<Animator>().SetBool("Death", true);
        Aim_transform.gameObject.SetActive(false);
        Destroy(gameObject, 1);
        GetComponent<AIPath>().maxSpeed = 0;
        Enemy_Death = true;
    }

    public override IEnumerator Fire()
    {
        Shooting = true;

        //Aiming
        Vector2 lookDir = new Vector2(target.transform.position.x, target.transform.position.y) - new Vector2(Aim_transform.transform.position.x, Aim_transform.transform.position.y);
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        Aim_transform.transform.rotation = Quaternion.Euler(Aim_transform.transform.rotation.x, Aim_transform.transform.rotation.y, angle);

        //Shooting


        for (int i = 0; i < AmountOfBullets; i++)
        {
            Quaternion Rotation = Quaternion.Euler(0, 0, FirePoint.rotation.eulerAngles.z);
            GameObject projectile = Instantiate(bullet, FirePoint.position, Rotation);
            projectile.GetComponent<Bullet_Enemy_Script>().Length = BulletDistnace;
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            rb.AddForce(projectile.transform.right * BulletForce, ForceMode2D.Impulse);


            //sound
            AudioManager AuMan = AudioManager.AudnioManagerInstance;
            AuMan.PlaySound(gameObject.transform.position, ShootSound);

            yield return new WaitForSeconds(Recoil);
        }

        State = Scout_State.Reloading;
        Shooting = false;
    }

    public virtual void Gun()
    {
        float DistanceFromPlayer = Vector2.Distance(rb.position, target.position);

        if (DistanceToShoot > DistanceFromPlayer && !Shooting && CanShoot() &&
            State == Scout_State.Shooting)
        {
            StartCoroutine(Fire());
        }


        if (target.transform.position.x - transform.position.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            Aim_transform.transform.localScale = new Vector3(1, 1, 1);
            Vector2 lookDir = new Vector2(target.transform.position.x, target.transform.position.y) - new Vector2(Aim_transform.transform.position.x, Aim_transform.transform.position.y);
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
            Aim_transform.transform.rotation = Quaternion.Euler(Aim_transform.transform.rotation.x, Aim_transform.transform.rotation.y, angle);

        }

        else if (target.transform.position.x - transform.position.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            Aim_transform.transform.localScale = new Vector3(-1, -1, 1);
            Vector2 lookDir = new Vector2(target.transform.position.x, target.transform.position.y) - new Vector2(Aim_transform.transform.position.x, Aim_transform.transform.position.y);
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
            Aim_transform.transform.rotation = Quaternion.Euler(Aim_transform.transform.rotation.x, Aim_transform.transform.rotation.y, angle);
        }
    }

    public override void MoveTowardsPlayer()
    {

    }

    IEnumerator Reload()
    {
        Reloading = true;
        GetComponent<AIPath>().canMove = false;

        yield return new WaitForSeconds(ReloadTime);

        GetComponent<AIPath>().canMove = true;
        State = Scout_State.Shooting;
        Reloading = false;
    }
}
