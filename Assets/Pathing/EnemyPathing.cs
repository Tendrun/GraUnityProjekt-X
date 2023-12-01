using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Pathfinding.Util;
using UnityEngine;

public class EnemyPathing : MonoBehaviour
{
    Seeker seeker;
    GridGraph graph;

    GraphNode OldNode;
    GraphNode NearestNode;

    static List<GameObject> Enemies;

    private void Start()
    {
        graph = AstarPath.active.data.gridGraph;

        seeker = GetComponent<Seeker>();
        seeker.pathCallback += OnPathComplete;
    }

    void OnPathComplete(Path P)
    {
        if (OldNode != null)
        {
            OldNode.Penalty = 0;
        }
        
        
        OldNode = NearestNode;
        NearestNode = graph.GetNearest(transform.position).node;
        NearestNode.Penalty = 1000;           
        
    }

    public void AddToList(GameObject obj)
    {
        Enemies.Add(obj);
    }

    private void OnDestroy()
    {
        if (OldNode != null)
        {
            OldNode.Penalty = 0;
        }
    }
}
