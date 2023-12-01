using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public abstract class Police_Shooting : Enemy
{
    [Header("Police Shooting")]
    public float BulletDistnace;
    public Transform Aim_transform;
    public float BulletForce;
    public float Recoil;
    public bool Shooting = false;
    public float DistanceToShoot = 5;
    public float MaxDistanceFromPlayer;
    public AudioClip ShootSound;

    [Header("On touch damage")]
    [SerializeField]
    bool isDamageOnTouch = true;
    [SerializeField]
    int DamageOnTouch = 1;

    [Header("Police Shooting Moving")]


    // shooting Correction
    [SerializeField]
    [Tooltip("This is used to find collisions which destroy bullet")]
    private LayerMask BulletColision;
    [SerializeField]
    protected GameObject bullet;
    public Transform FirePoint;
    [SerializeField]
    [Tooltip("This is used for shooting Units to detect proper layers")]
    private LayerMask OnlyDetect;



    public abstract IEnumerator Fire();

    public void Update()
    {
        DetectDamageOnTouch();

        MoveTowardsPlayer();
    }

    public void DetectDamageOnTouch()
    {
        if (isDamageOnTouch && Player_Script.PlayerInstance.GetComponent<Collider2D>().IsTouching(GetComponent<Collider2D>()))
        {
            Player_Script.PlayerInstance.Damage(DamageOnTouch);
        }
    }

    public virtual void MoveTowardsPlayer()
    {
        if(Vector2.Distance(target.transform.position, transform.position) > MaxDistanceFromPlayer)
        {
            GetComponent<AIPath>().canMove = true;
        }
        else if (Vector2.Distance(target.transform.position, transform.position) < MaxDistanceFromPlayer)
        {
            GetComponent<AIPath>().canMove = false;
        }
    }

    protected bool CanShoot()
    {
        //Those lines of code are used to check bullet width and height
        Vector2 bullet_size = new Vector2(0.56f, 0.42f);

        Vector2 direction = (Player_Script.PlayerInstance.transform.position - transform.position).normalized;
        RaycastHit2D BoxHit = Physics2D.BoxCast(FirePoint.transform.position, bullet_size, FirePoint.rotation.z, direction, BulletDistnace, OnlyDetect.value);


        //Debug.Log("Dokoncz mnie");
        Debug.DrawLine(new Vector2(FirePoint.transform.position.x, FirePoint.transform.position.y), Player_Script.PlayerInstance.transform.position, Color.red);

        if (BoxHit.collider != null && ((1 << BoxHit.transform.gameObject.layer) & BulletColision) != 0)
        {
            return false;
        }
        else if (BoxHit.collider != null && ((1 << BoxHit.transform.gameObject.layer) & (1 << Player_Script.PlayerInstance.gameObject.layer)) != 0) return true;
        else return false;

    }
}

//this is used for Police who dont shoot and are very specific
public class Police_Variations : Enemy
{

    [Header("On touch damage")]
    [SerializeField]
    bool isDamageOnTouch = true;
    [SerializeField]
    int DamageOnTouch = 1;


    public void Update()
    {
        DetectDamageOnTouch();
    }

    public void DetectDamageOnTouch()
    {
        if (isDamageOnTouch && Player_Script.PlayerInstance.GetComponent<Collider2D>().IsTouching(GetComponent<Collider2D>()))
        {
            Player_Script.PlayerInstance.Damage(DamageOnTouch);
        }
    }
}
