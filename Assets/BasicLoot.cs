using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BasicLoot : Loot
{
    
}

public class Loot : MonoBehaviour
{
    [SerializeField]
    private LootItem[] LootItems;

    public void Init(LootItem[] LootItems)
    {
        this.LootItems = LootItems;
    }



    public void DropLoot(GameObject obj)
    {
        for (int i = 0; i < LootItems.Length; i++)
        {
            int RangeNum = Random.Range(0, 101);
            if (RangeNum <= LootItems[i].Chance)
            {
                Instantiate(LootItems[i].Loot, obj.transform.position, Quaternion.identity);
            }
        }
    }
}

[System.Serializable]
public class LootItem
{
    public GameObject Loot;
    [Range(0, 100)]
    public int Chance = 0;
}