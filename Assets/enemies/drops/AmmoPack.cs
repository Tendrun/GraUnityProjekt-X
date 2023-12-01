using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPack : PickUpItem
{
    public override void ItemEffect()
    {
        AddStats();
    }
}


public abstract class PickUpItem : StatsChanger
{
    public LayerMask ObjectCollision = 6;

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if ((1 << collision.gameObject.layer) == ObjectCollision.value)
        {
            ItemEffect();
            Destroy(gameObject, 0);
        }
    }

    public virtual void ItemEffect() => AddStats();
}