using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
public class Boss_1 : Police_Variations
{
    Animator animator;

    //Shooting Based components
    [Header("Shooting Based components")]
    public float BulletDistnace;
    public Transform Aim_transform;
    public float BulletForce;
    public int BulletDamage;
    public float Recoil;
    public float AmountOfBullets, ShootingInterval, DistanceToShoot;
    [Tooltip("This number has to be odd(nieparzysta)")]
    public int AmountOfWaves;
    public Rigidbody2D rb;
    public bool Shooting;
    [SerializeField]
    [Tooltip("This is used to find collisions which destroy bullet")]
    public LayerMask BulletColision;
    [SerializeField]
    public GameObject bullet;
    public Transform FirePoint;
    [SerializeField]
    [Tooltip("This is used for shooting Units to detect proper layers")]
    public LayerMask OnlyDetect;
    public AudioClip ShootSound;
    public float SpreadAngle;

    [Header("Charge")]
    [SerializeField]
    float ChargeTime;
    [SerializeField]
    float ChargeTimer;
    public float JumpAttackSpeed;
    public float JumpLength;
    public int JumpDamage;
    public LayerMask CollisionInterruptJump;

    [Header("Rocket")]
    [SerializeField]
    private float RocketTime;
    [SerializeField]
    private float RocketSpeed;
    private float RocketTimer;
    [SerializeField]
    private int RocketDamage;
    [SerializeField]
    LayerMask CollisionRocketDestroy;
    [SerializeField]
    GameObject Rocket;
    [SerializeField]
    bool Buff = false;

    private enum Boss_Stages
    {
        Shooting,
        Charge,
        Walking,
    }

    [SerializeField]
    Boss_Stages Stage = Boss_Stages.Shooting;

    public new virtual void Start()
    {
        base.Start();

        RocketTimer = RocketTime;
        ChargeTimer = ChargeTime;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public new void Update()
    {
        base.Update();


        Vector2 Body_size = GetComponent<Collider2D>().bounds.size;

        Vector2 direction = (Player_Script.PlayerInstance.transform.position - transform.position).normalized;
        RaycastHit2D BoxHit = Physics2D.BoxCast(transform.position, Body_size, FirePoint.rotation.z, direction, BulletDistnace, OnlyDetect.value);




        if (((float)CurrentHealth / (float)MaxHealth) <= 0.5f && Buff == false)
        {
            Buff = true;
            RocketSpeed *= 2;
        }

        else if ((CurrentHealth / MaxHealth) > 0.5f && Buff == true)
        {
            RocketSpeed /= 2;
        }

        if (ChargeTimer >= 0)
        {
            ChargeTimer -= Time.deltaTime;
        }

        if (RocketTimer >= 0)
        {
            RocketTimer -= Time.deltaTime;
        }

        else if (RocketTimer <= 0)
        {
            RocketTimer = RocketTime;
            LaunchRocket();
        }

        if (Stage == Boss_Stages.Walking)
        {
            animator.SetBool("Shooting", false);
            GetComponent<AIDestinationSetter>().target = Player_Script.PlayerInstance.transform;
            GetComponent<AIPath>().canMove = true;
            Gun();


            if (CanShoot() &&
                BoxHit.collider != null && ((1 << BoxHit.transform.gameObject.layer) & CollisionInterruptJump) == 0)
                {
                    Stage = Boss_Stages.Shooting;
                }
        }

        else if (Stage == Boss_Stages.Shooting)
        {
            GetComponent<AIPath>().canMove = false;
            animator.SetBool("Shooting", true);

            Aim_transform.gameObject.SetActive(true);

            if (!Enemy_Death && !CanCharge())
            {
                Gun();
            }

            if (ChargeTimer <= 0 && CanCharge())
            {
                Debug.Log("Can charge");
                Stage = Boss_Stages.Charge;
                animator.SetBool("Shooting", false);
            }           

            else if (!CanCharge() &&
                BoxHit.collider != null && ((1 << BoxHit.transform.gameObject.layer) & CollisionInterruptJump) != 0)
                {
                    Stage = Boss_Stages.Walking;
                }

            else if (!CanShoot())
            {
                Debug.Log("Cant shoot");
                Stage = Boss_Stages.Walking;
            }
        }

        else if (Stage == Boss_Stages.Charge)
        {
            Aim_transform.gameObject.SetActive(false);
            GetComponent<AIPath>().canMove = false;
            animator.SetBool("Charg", true);
        }
    }

    private void LaunchRocket()
    {
        Boss_1_Rocket Rocket_ = Instantiate(Rocket, (Vector2)gameObject.transform.position, Quaternion.identity).GetComponent<Boss_1_Rocket>();

        Rocket_.RocketSpeed = RocketSpeed;
        Rocket_.RocketDamage = RocketDamage;
    }

    public void ChargeEnded()
    {
        Stage = Boss_Stages.Shooting;
        ChargeTimer = ChargeTime;
        animator.SetBool("Charg", false);
    }

    public bool CanShoot()
    {
        //Those lines of code are used to check bullet width and height
        Vector2 bullet_size = new Vector2(0.56f, 0.42f);

        Vector2 direction = (Player_Script.PlayerInstance.transform.position - transform.position).normalized;
        RaycastHit2D BoxHit = Physics2D.BoxCast(FirePoint.transform.position, bullet_size, FirePoint.rotation.z, direction, BulletDistnace, OnlyDetect.value);


        
        Debug.DrawLine(new Vector2(FirePoint.transform.position.x, FirePoint.transform.position.y), Player_Script.PlayerInstance.transform.position, Color.red);

        if (BoxHit.collider != null && ((1 << BoxHit.transform.gameObject.layer) & BulletColision) != 0)
        {
            return false;

        }
        else if (BoxHit.collider != null && ((1 << BoxHit.transform.gameObject.layer) & (1 << Player_Script.PlayerInstance.gameObject.layer)) != 0) return true;
        else return false;
    }

    public bool CanCharge()
    {
        if (Shooting == true || ChargeTimer >= 0) return false;

        //Those lines of code are used to check bullet width and height
        Vector2 Body_size = GetComponent<Collider2D>().bounds.size;

        Vector2 direction = (Player_Script.PlayerInstance.transform.position - transform.position).normalized;
        RaycastHit2D BoxHit = Physics2D.BoxCast(transform.position, Body_size, FirePoint.rotation.z, direction, BulletDistnace, OnlyDetect.value);


        if (BoxHit.collider != null && ((1 << BoxHit.transform.gameObject.layer) & BulletColision) != 0)
        {
            return false;
        }

        else if (BoxHit.collider != null && ((1 << BoxHit.transform.gameObject.layer) & (1 << Player_Script.PlayerInstance.gameObject.layer)) != 0) return true;
        else return false;
    }

    //Shooting
    public void Gun()
    {
        float DistanceFromPlayer = Vector2.Distance(rb.position, target.position);

        if (DistanceToShoot > DistanceFromPlayer && !Shooting && CanShoot())
        {
            StartCoroutine(Fire());
        }


        if (target.transform.position.x - gameObject.transform.position.x > 0)
        {
            gameObject.transform.localScale = new Vector3(1, 1, 1);
            Aim_transform.transform.localScale = new Vector3(1, 1, 1);
            Vector2 lookDir = new Vector2(target.transform.position.x, target.transform.position.y) - new Vector2(Aim_transform.transform.position.x, Aim_transform.transform.position.y);
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
            Aim_transform.transform.rotation = Quaternion.Euler(Aim_transform.transform.rotation.x, Aim_transform.transform.rotation.y, angle);

        }

        else if (target.transform.position.x - gameObject.transform.position.x < 0)
        {
            gameObject.transform.localScale = new Vector3(-1, 1, 1);
            Aim_transform.transform.localScale = new Vector3(-1, -1, 1);
            Vector2 lookDir = new Vector2(target.transform.position.x, target.transform.position.y) - new Vector2(Aim_transform.transform.position.x, Aim_transform.transform.position.y);
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
            Aim_transform.transform.rotation = Quaternion.Euler(Aim_transform.transform.rotation.x, Aim_transform.transform.rotation.y, angle);
        }
    }

    public IEnumerator Fire()
    {
        Shooting = true;

        //Aiming
        Vector2 lookDir = new Vector2(target.transform.position.x, target.transform.position.y) - new Vector2(Aim_transform.transform.position.x, Aim_transform.transform.position.y);
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        Aim_transform.transform.rotation = Quaternion.Euler(Aim_transform.transform.rotation.x, Aim_transform.transform.rotation.y, angle);

        //Shooting
        float AngleCounter = AmountOfWaves / 2;

        for (int i = 0; i < AmountOfBullets; i++)
        {

            //Amount of waves
            for (int j = 0; j < 3; j++)
            {
                float SpreadAngle_ = 0;



                if (j == 0) SpreadAngle_ = -SpreadAngle;
                else if (j == 1) SpreadAngle_ = 0;
                else if (j == 2) SpreadAngle_ = SpreadAngle;

                Quaternion Rotation = Quaternion.Euler(0, 0, FirePoint.rotation.eulerAngles.z + SpreadAngle_);

                //Quaternion Rotation = Quaternion.Euler(0, 0, FirePoint.rotation.eulerAngles.z);
                GameObject projectile = Instantiate(bullet, FirePoint.position, Rotation);
                projectile.GetComponent<Bullet_Enemy_Script>().Length = BulletDistnace;
                projectile.GetComponent<Bullet_Enemy_Script>().Damage = BulletDamage;
                Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
                rb.AddForce(projectile.transform.right * BulletForce, ForceMode2D.Impulse);
            }




            //sound
            AudioManager AuMan = AudioManager.AudnioManagerInstance;
            AuMan.PlaySound(gameObject.transform.position, ShootSound);

            yield return new WaitForSeconds(ShootingInterval);
        }

        yield return new WaitForSeconds(Recoil);
        Shooting = false;
    }
}
