using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;


public class PoliceRat : Police_Variations
{
    
    public float StartAttackDistance;
    public float JumpLength;
    public float JumpAttackSpeed = 9;
    public int JumpDamage;
    public LayerMask CollisionInterruptJump;

    public Transform BackPos, FrontPos, BackPos2, FrontPos2;

    public override void Start()
    {
        base.Start();
        GetComponent<AIPath>().maxSpeed = CurrentSpeed;
        GetComponent<AIDestinationSetter>().target = target;
    }

    private new void Update()
    {
        base.Update();
    }

    private void FixedUpdate()
    {
        if (!Enemy_Death)
        {
            //Rotate
            Rotate();
        }
    }

    private void Rotate()
    {

        if (target.transform.position.x - transform.position.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);

        }

        else if (target.transform.position.x - transform.position.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }


    public override void Death()
    {
        base.Death();
        if (Enemy_Death) return;
        
        gameObject.GetComponent<Animator>().SetBool("Death", true);
        Destroy(gameObject, 1);
        GetComponent<AIPath>().maxSpeed = 0;
        Enemy_Death = true;
    }
}
