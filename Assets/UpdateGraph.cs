using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateGraph : MonoBehaviour
{
    [SerializeField]
    public Bounds graphobject;

    // Start is called before the first frame update
    void Update()
    {
        AstarPath.active.UpdateGraphs(graphobject);
    }

}
