using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1_Shooting : StateMachineBehaviour
{
    Boss_1 Boss;

    //Shooting Based components
    [Header("Shooting Based components")]
    public float BulletDistnace;
    public Transform Aim_transform;
    public float BulletForce;
    public float Recoil;
    public float AmountOfBullets, ShootingInterval, DistanceToShoot;
    public Rigidbody2D rb;
    public bool Shooting;
    [SerializeField]
    [Tooltip("This is used to find collisions which destroy bullet")]
    private LayerMask BulletColision;
    [SerializeField]
    protected GameObject bullet;
    public Transform FirePoint;
    [SerializeField]
    [Tooltip("This is used for shooting Units to detect proper layers")]
    private LayerMask OnlyDetect;

    public AudioClip ShootSound;
    public Transform target;
    public GameObject gameObject;
    public float SpreadAngle;
    public int AmountOfWaves;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }


    

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}
