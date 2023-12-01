using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSetDescription : Item
{

    public void GiveItem()
    {
        Player_Script.PlayerInstance.GiveItem(this);
    }

    public override void ItemPassiveEffect()
    {

    }

    public override void ItemEffect()
    {

    }

    public override StatObject[] ItemDelete()
    {
        //delete stats
        return Effects.ToArray();
    }
}
