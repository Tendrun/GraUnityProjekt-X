using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Fries_Script : Player_Script
{
    #region Player Passive
    public bool PassiveTimer()
    {
        if (PlayerPassiveCurrentTime > 0)
        {
            PlayerPassiveTimerBool = false;
            PlayerPassiveCurrentTime -= Time.deltaTime;
            return false;
        }

        else
        {
            PlayerPassiveTimerBool = true;
            return true;
        }

    }

    public override IEnumerator PlayerPassive()
    {
        //Every 10 killed Enemies drops a blant which heals him for 1 hp
        yield return new WaitUntil(() => PassiveTimer() && GameManager.GameManagerInstance.On10EnemiesDeaths());
        {
            Instantiate(PlayerPassiveItem, GameManager.GameManagerInstance.GetLastEnemyKilled(), Quaternion.identity);
            PlayerPassiveCurrentTime = PlayerPassiveRegenTime;
        }

        StartCoroutine(PlayerPassive());
    }

    #endregion

    Seeker seeker;


    public new void Start()
    {
        base.Start();

        //seeker.pathCallback += OnPathComplete;
    }

    public new void Update()
    {
        base.Update();
    }

    GraphNode Oldnode = null;

    public void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            GraphNode node = AstarPath.active.GetNearest(transform.position).node;
            if (node != null && node.Walkable && node != Oldnode)
            {
                Debug.Log("Walking");

                node.Penalty = 100000;
                if (Oldnode != null)
                    Oldnode.Penalty = 0;

                Oldnode = node;
            }

            else if (node == Oldnode)
            {
                Debug.Log("node = " + node.NodeIndex);
                Oldnode.Penalty = 100000;
            }

        }
    }
}