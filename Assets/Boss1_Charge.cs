using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1_Charge : StateMachineBehaviour
{
    Player_Script Player;
    Vector2 enemy;
    Vector2 PlayerPos;
    float speed;
    Boss_1 Boss;
    bool AttackPlayer = true;
    Vector2 PositionToJump;

    MonoBehaviour monoBehaviour;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player = Player_Script.PlayerInstance;
        PlayerPos = Player.gameObject.transform.position;
        enemy = animator.gameObject.transform.position;
        speed = animator.GetComponent<Boss_1>().JumpAttackSpeed;
        Boss = animator.gameObject.GetComponent<Boss_1>();
        AttackPlayer = true;


        Vector2 direction = PlayerPos - (Vector2)animator.gameObject.transform.position;
        direction.Normalize();
        PositionToJump = (Vector2)animator.gameObject.transform.position + direction * Boss.JumpLength;

        monoBehaviour = animator.GetComponent<MonoBehaviour>();
        monoBehaviour.StartCoroutine(Fire());
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.DrawLine(enemy, PositionToJump, Color.red);

        animator.gameObject.transform.position = Vector2.MoveTowards(animator.gameObject.transform.position, PositionToJump, Time.deltaTime * speed);


        if (AttackPlayer == true && animator.GetComponent<Collider2D>().IsTouching(Player.GetComponent<Collider2D>()))
        {
            Physics2D.IgnoreCollision(Player.GetComponent<Collider2D>(), animator.GetComponent<Collider2D>(), true);
            AttackPlayer = false;
            Player.Damage(Boss.JumpDamage);
        }

        int layer_mask = Boss.CollisionInterruptJump.value;

        //On Wall touch
        if (animator.GetComponent<Collider2D>().IsTouchingLayers(layer_mask))
        {
            Debug.Log("Wall touched");
            AttackPlayer = false;
            animator.SetTrigger("InterruptJump");
            Boss.ChargeEnded();
            return;
        }
        
        //On Distance travelled
        if (Vector2.Distance((Vector2)animator.gameObject.transform.position, PositionToJump) <= 1)
        {
            Boss.ChargeEnded();
        }
        
    }

    public IEnumerator Fire()
    {
        //Aiming
        Vector2 lookDir = new Vector2(Boss.target.transform.position.x, Boss.target.transform.position.y) - new Vector2(Boss.Aim_transform.transform.position.x, Boss.Aim_transform.transform.position.y);
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        Boss.Aim_transform.transform.rotation = Quaternion.Euler(Boss.Aim_transform.transform.rotation.x, Boss.Aim_transform.transform.rotation.y, angle);

        //Shooting
        float AngleCounter = Boss.AmountOfWaves / 2;

        for (int i = 0; i < Boss.AmountOfBullets; i++)
        {

            //Amount of waves
            for (int j = 0; j < 3; j++)
            {
                float SpreadAngle_ = 0;



                if (j == 0) SpreadAngle_ = -Boss.SpreadAngle;
                else if (j == 1) SpreadAngle_ = 0;
                else if (j == 2) SpreadAngle_ = Boss.SpreadAngle;

                Quaternion Rotation = Quaternion.Euler(0, 0, Boss.FirePoint.rotation.eulerAngles.z + SpreadAngle_);

                //Quaternion Rotation = Quaternion.Euler(0, 0, FirePoint.rotation.eulerAngles.z);
                GameObject projectile = Instantiate(Boss.bullet, Boss.FirePoint.position, Rotation);
                projectile.GetComponent<Bullet_Enemy_Script>().Length = Boss.BulletDistnace;
                projectile.GetComponent<Bullet_Enemy_Script>().Damage = Boss.BulletDamage;
                Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
                rb.AddForce(projectile.transform.right * Boss.BulletForce, ForceMode2D.Impulse);
            }




            //sound
            AudioManager AuMan = AudioManager.AudnioManagerInstance;
            AuMan.PlaySound(Boss.gameObject.transform.position, Boss.ShootSound);

            yield return new WaitForSeconds(Boss.ShootingInterval);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Physics2D.IgnoreCollision(Player.GetComponent<Collider2D>(), animator.GetComponent<Collider2D>(), false);
        monoBehaviour = null;
        Boss.ChargeEnded();
    }
}
