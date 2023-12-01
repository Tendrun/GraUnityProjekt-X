using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class RatWalkingBehaviour : StateMachineBehaviour
{
    GameObject PoliceRatObj;
    PoliceRat PoliceRatScript;
    float AttackDistnace;
    int layer_mask;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PoliceRatObj = animator.gameObject;
        PoliceRatObj.GetComponent<AIPath>().canMove = true;
        PoliceRatScript = PoliceRatObj.GetComponent<PoliceRat>();
        AttackDistnace = PoliceRatScript.StartAttackDistance;
        layer_mask = PoliceRatScript.CollisionInterruptJump.value + (1 << Player_Script.PlayerInstance.gameObject.layer);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector2 vector2BackDir = (Player_Script.PlayerInstance.transform.position - PoliceRatScript.BackPos.transform.position).normalized;
        float vector2PlayerDistance = Vector2.Distance(Player_Script.PlayerInstance.transform.position, PoliceRatScript.transform.position);

        if (CheckRayColl()
                && Vector2.Distance(PoliceRatObj.transform.position, Player_Script.PlayerInstance.gameObject.transform.position) < AttackDistnace)
                    animator.SetTrigger("Attack");

    }

    public bool CheckRayColl()
    {
        int ReverseNumber = PoliceRatObj.transform.localScale.x > 0 ? -1 : 1;
        int numberY = PoliceRatObj.transform.localScale.x > 0 ? 1 : -1;

        RaycastHit2D hitFront = Physics2D.Linecast(PoliceRatScript.FrontPos.transform.position, (Vector2)Player_Script.PlayerInstance.transform.position - new Vector2(PoliceRatScript.FrontPos.transform.localPosition.x, PoliceRatScript.FrontPos.transform.localPosition.y * numberY) * ReverseNumber, layer_mask, -Mathf.Infinity, Mathf.Infinity);
        RaycastHit2D hitFront2 = Physics2D.Linecast(PoliceRatScript.FrontPos2.transform.position, (Vector2)Player_Script.PlayerInstance.transform.position - new Vector2(PoliceRatScript.FrontPos2.transform.localPosition.x, PoliceRatScript.FrontPos2.transform.localPosition.y * numberY) * ReverseNumber, layer_mask, -Mathf.Infinity, Mathf.Infinity);
        RaycastHit2D hitBack = Physics2D.Linecast(PoliceRatScript.BackPos.transform.position, (Vector2)Player_Script.PlayerInstance.transform.position - new Vector2(PoliceRatScript.BackPos.transform.localPosition.x, PoliceRatScript.BackPos.transform.localPosition.y * numberY) * ReverseNumber, layer_mask, -Mathf.Infinity, Mathf.Infinity);
        RaycastHit2D hitBack2 = Physics2D.Linecast(PoliceRatScript.BackPos2.transform.position, (Vector2)Player_Script.PlayerInstance.transform.position - new Vector2(PoliceRatScript.BackPos2.transform.localPosition.x, PoliceRatScript.BackPos2.transform.localPosition.y * numberY) * ReverseNumber, layer_mask, -Mathf.Infinity, Mathf.Infinity);
        RaycastHit2D hitMiddle = Physics2D.Linecast(PoliceRatScript.transform.position, Player_Script.PlayerInstance.transform.position, layer_mask, -Mathf.Infinity, Mathf.Infinity);



        Debug.DrawLine(PoliceRatScript.BackPos.transform.position, (Vector2)Player_Script.PlayerInstance.transform.position - new Vector2(PoliceRatScript.BackPos.transform.localPosition.x, PoliceRatScript.BackPos.transform.localPosition.y * numberY) * ReverseNumber, Color.blue);
        Debug.DrawLine(PoliceRatScript.FrontPos.transform.position, (Vector2)Player_Script.PlayerInstance.transform.position - new Vector2(PoliceRatScript.FrontPos.transform.localPosition.x, PoliceRatScript.FrontPos.transform.localPosition.y * numberY) * ReverseNumber, Color.red); 
        Debug.DrawLine(PoliceRatScript.BackPos2.transform.position, (Vector2)Player_Script.PlayerInstance.transform.position - new Vector2(PoliceRatScript.BackPos2.transform.localPosition.x, PoliceRatScript.BackPos2.transform.localPosition.y * numberY) * ReverseNumber, Color.cyan);
        Debug.DrawLine(PoliceRatScript.FrontPos2.transform.position, (Vector2)Player_Script.PlayerInstance.transform.position - new Vector2(PoliceRatScript.FrontPos2.transform.localPosition.x, PoliceRatScript.FrontPos2.transform.localPosition.y * numberY) * ReverseNumber, Color.grey);
        Debug.DrawLine(PoliceRatObj.gameObject.transform.position, Player_Script.PlayerInstance.transform.position);

        //calculate distance between Player and Obstacle 

        float ClosestObstacle = 100;

        if (PoliceRatObj.GetComponent<Collider2D>().IsTouchingLayers(PoliceRatScript.CollisionInterruptJump))
        {
            return false;
        }

        else if (hitFront.collider != null && ((1 << hitFront.transform.gameObject.layer) & PoliceRatScript.CollisionInterruptJump) != 0)
        {
            float Distance = Vector2.Distance(hitFront.transform.gameObject.transform.position, PoliceRatScript.transform.position);

            if (Distance < ClosestObstacle)
                ClosestObstacle = Distance;
        }

        else if (hitFront2.collider != null && ((1 << hitFront2.transform.gameObject.layer) & PoliceRatScript.CollisionInterruptJump) != 0)
        {
            float Distance = Vector2.Distance(hitFront2.transform.gameObject.transform.position, PoliceRatScript.transform.position);

            if (Distance < ClosestObstacle)
                ClosestObstacle = Distance;
        }

        else if (hitBack.collider != null && ((1 << hitBack.transform.gameObject.layer) & PoliceRatScript.CollisionInterruptJump) != 0)
        {
            float Distance = Vector2.Distance(hitBack.transform.gameObject.transform.position, PoliceRatScript.transform.position);

            if (Distance < ClosestObstacle)
                ClosestObstacle = Distance;
        }

        else if (hitBack2.collider != null && ((1 << hitBack2.transform.gameObject.layer) & PoliceRatScript.CollisionInterruptJump) != 0)
        {
            float Distance = Vector2.Distance(hitBack2.transform.gameObject.transform.position, PoliceRatScript.transform.position);

            if (Distance < ClosestObstacle)
                ClosestObstacle = Distance;
        }

        if (hitMiddle.collider != null && hitMiddle.collider.gameObject == Player_Script.PlayerInstance.gameObject)
        {
            float PlayerDistance = Vector2.Distance(PoliceRatScript.transform.gameObject.transform.position, Player_Script.PlayerInstance.transform.position);

            if (PlayerDistance < ClosestObstacle) return true;
                else return false;
        }

        return false;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Attack");
        PoliceRatObj.GetComponent<AIPath>().canMove = false;
    }
}
