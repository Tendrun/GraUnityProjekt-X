using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatAttackingBehaviour : StateMachineBehaviour
{
    Player_Script Player;
    Vector2 enemy;
    Vector2 PlayerPos;
    float speed;
    PoliceRat policerat;
    bool AttackPlayer = true;
    Vector2 PositionToJump;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player = Player_Script.PlayerInstance;
        PlayerPos = Player.gameObject.transform.position;
        enemy = animator.gameObject.transform.position;
        speed = animator.GetComponent<PoliceRat>().JumpAttackSpeed;
        policerat = animator.gameObject.GetComponent<PoliceRat>();
        AttackPlayer = true;


        Vector2 direction = PlayerPos - (Vector2)animator.gameObject.transform.position;
        direction.Normalize();
        PositionToJump = (Vector2)animator.gameObject.transform.position + direction * policerat.JumpLength;
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
            Player.Damage(policerat.JumpDamage);
        }

        int layer_mask = policerat.CollisionInterruptJump.value;


        if (animator.GetComponent<Collider2D>().IsTouchingLayers(layer_mask))
        {
            Debug.Log("Wall touched");
            AttackPlayer = false;
            animator.SetTrigger("InterruptJump");
            return;
        }
        

    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Physics2D.IgnoreCollision(Player.GetComponent<Collider2D>(), animator.GetComponent<Collider2D>(), false);
    }
}
