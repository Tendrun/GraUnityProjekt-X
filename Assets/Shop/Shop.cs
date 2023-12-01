using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [SerializeField]
    List<GameObject> Items;
    [SerializeField]
    GameObject ItemsSlot;
    [SerializeField]
    GameObject grid_layout;

    Player_Script Player;

    private void Start()
    {
        Player = Player_Script.PlayerInstance;

        for(int i = 0; i < Items.Count; i++)
        {
            GameObject item = Instantiate(ItemsSlot, grid_layout.transform);

            Item item1 = Items[i].GetComponent<Item>();

            item.GetComponent<ItemSlotManager>().ItemImage.sprite = item1.ItemSprite;
            item.GetComponent<ItemSlotManager>().item = item1;
            item.GetComponent<ItemSlotManager>().button.onClick.AddListener(delegate { buy(item1); });
            //item.GetComponent<ItemSlotManager>().button.navigation = Navigation.Mode.None;

        }
    }

    public void buy(Item item)
    {
        if (item.price <= Player.CheckGold())
        {
            Player.GiveItem(item);
            Player.ChangeGoldAmount(-item.price);
        }

        else
        {
            Debug.Log("Biedaku nazbieraj kase");
        }
    }
}

public abstract class Item : MonoBehaviour
{
    public Sprite ItemSprite;
    public GameObject item;
    public int price;
    public string Descrpiton;
    public int IndexInEq;

    public List<StatObject> Effects;

    [SerializeField]
    StatsChanger Stats;
    Player_Script Player;

    public abstract void ItemPassiveEffect();
    public void ItemAddStats()
    {
        Player = Player_Script.PlayerInstance;

        if (Stats.IncreaseLightWeaponDMG != 0)
            Effects.Add(Player.IncreaseLightWeaponDamage(Stats.IncreaseLightWeaponDMG, 0));

        if (Stats.MaxHeavyAmmoPlayer != 0)
            Effects.Add(Player.IncreaseMaxHeavyAmmo(Stats.MaxHeavyAmmoPlayer, 0));

        if (Stats.AddMaxhealth != 0)
            Effects.Add(Player.AddMaxHealthUnit(Stats.AddMaxhealth, 0));

        if (Stats.speed != 0)
            Effects.Add(Player.AddSpeed(Stats.speed, 0));

        if (Stats.ReloadTimeDivider != 0)
            Effects.Add(Player.AddReloadTimeDivider(Stats.ReloadTimeDivider, 0));

        if (Stats.StaminaRegenMultiplier != 0)
            Effects.Add(Player.AddStaminaRegen(Stats.StaminaRegenMultiplier, 0));
    }

    public void ListItems()
    {
        Player.ShowItems(0);


    }
    public void DeleteItemPublic(int key)
    {
        Player.DeleteItem(key);
    }

    public void AddindexEffect(StatObject obj)
    {
        Effects.Add(obj);
    }

    public abstract void ItemEffect();
    public abstract StatObject[] ItemDelete();
}