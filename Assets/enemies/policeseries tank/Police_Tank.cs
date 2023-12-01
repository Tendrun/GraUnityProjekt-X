using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;


public class Police_Tank : Police_Shooting
{
    [Header("Police Tank gun stats")]
    [SerializeField]
    private float AmountOfBullets, ShootingInterval;



    //NEW 

    public Transform EnemyFuturePosition;

    public float DistanceFromPlayerX;
    public float DistanceFromPlayerY;
    public Vector3 DistanceFromPlayer;
    public float DistanceToShootoffset = 1;



    protected Rigidbody2D rb;



    public override void Awake()
    {
        base.Awake();

        float RandX = Random.Range(-DistanceFromPlayerX / 2, DistanceFromPlayerX / 2);
        float RandY = Random.Range(-DistanceFromPlayerY / 2, DistanceFromPlayerY / 2);

        Vector2 Dis = new Vector2(RandX, RandY);

        if (DistanceToShoot < Vector2.Distance(new Vector2(RandX, RandY), new Vector2(0, 0)))
        {
            GetRandomVector2();
        }

        DistanceFromPlayer = new Vector3(RandX, RandY, 0);
    }



    protected Vector2 GetRandomVector2()
    {
        float RandX = Random.Range(-DistanceFromPlayerX / 2, DistanceFromPlayerX / 2);
        float RandY = Random.Range(-DistanceFromPlayerY / 2, DistanceFromPlayerY / 2);

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
        GetComponent<AIDestinationSetter>().target = EnemyFuturePosition;
    }


    public new void Update()
    {
        base.Update();

        EnemyFuturePosition.position = target.transform.position + DistanceFromPlayer;
    }


    // Update is called once per frame
    private void FixedUpdate()
    {
        if (!Enemy_Death)
        {
            //Shooting
            Gun();
        }
    }


    public override void Death()
    {
        base.Death();
        if (Enemy_Death) return;

        Enemy_Death = true;
        gameObject.GetComponent<Animator>().SetBool("Death", true);
        Aim_transform.gameObject.SetActive(false);
        Destroy(gameObject, 1);
        GetComponent<AIPath>().maxSpeed = 0;
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

            yield return new WaitForSeconds(ShootingInterval);
        }

        yield return new WaitForSeconds(Recoil);

        Shooting = false;
    }

    public void Gun()
    {
        float DistanceFromPlayer = Vector2.Distance(rb.position, target.position);

        if (DistanceToShoot > DistanceFromPlayer && !Shooting && CanShoot())
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
}
