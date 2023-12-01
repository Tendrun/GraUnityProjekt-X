using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class StatsChanger : MonoBehaviour
{
    [SerializeField]
    private Player_Script PlayerScript;
    /// <summary>
    /// Give player Heavy ammo, if number AmountOfHeavyAmmo + current player heavy ammo is bigger than player capacity, the number is set to max value
    /// </summary>
    public int AmountOfHeavyAmmo;
    public int AddMaxhealth;
    public int Heal;
    public float speed;
    public float StaminaRegenMultiplier;
    [Range(0f, 1f)]
    public float ReloadTimeDivider;
    public float StunTime;
    public int MaxHeavyAmmoPlayer;
    public int IncreaseLightWeaponDMG;

    public bool AddStatsOnStart = false;

    public ItemSetDescription item;

    [SerializeField]
    GameObject Healing_Effect;

    // Start is called before the first frame update
    void Start()
    {
        PlayerScript = Player_Script.PlayerInstance.GetComponent<Player_Script>();
        if (AddStatsOnStart)
            AddStats();
    }

    public void AddStats()
    {
        PlayerScript = Player_Script.PlayerInstance.GetComponent<Player_Script>();

        if (IncreaseLightWeaponDMG != 0) PlayerScript.IncreaseLightWeaponDamage(IncreaseLightWeaponDMG);
        if (MaxHeavyAmmoPlayer != 0) PlayerScript.IncreaseMaxHeavyAmmo(MaxHeavyAmmoPlayer);
        if (AmountOfHeavyAmmo != 0) PlayerScript.AddHeavyAmmo(AmountOfHeavyAmmo);

        if (AddMaxhealth != 0) PlayerScript.AddMaxHealthUnit(AddMaxhealth);
        if (Heal != 0) Destroy(Instantiate(Healing_Effect, gameObject.transform.position, Quaternion.Euler(-90, 0, 0)), 2);
            PlayerScript.HealUnit(Heal);
        if (speed != 0) PlayerScript.AddSpeed(speed);

        if (StaminaRegenMultiplier != 0) PlayerScript.AddStaminaRegen(StaminaRegenMultiplier);
        if (ReloadTimeDivider != 0) PlayerScript.AddReloadTimeDivider(ReloadTimeDivider);
        if (StunTime != 0) PlayerScript.StunUnit(StunTime);
    }
}

/*
 * 
 * 
 * 
 * WAZNE CZYTAJ TO
 * 
 * JEZELI CHCESZ DODAC NOWY EFFEKT MUSISZ DODAC DO 
 * ITEM ItemAddStats() NOWA STATYSTYKE I WARUNEK TAK SAMO ITEM ADDSTATS
 * A U GRACZA DODAC DICTONARY COUNT ADD I DELETE STATS
 * 
 * no to wszystko powodzenia z tym kodem <3
 * 
 * 
 * 
 * 
 */

[System.Serializable]
public class LightDamageObject : StatObject
{
    public int Damage;

    public LightDamageObject(int Damage)
    {
        this.Damage = Damage;
    }
}

public class MaxHeavyAmmoObject : StatObject
{
    public int Ammo;

    public MaxHeavyAmmoObject(int Ammo)
    {
        this.Ammo = Ammo;
    }
}

public class MaxHealthObject : StatObject
{
    public int health;

    public MaxHealthObject(int health)
    {
        this.health = health;
    }
}

[System.Serializable]
public class StunObject : StatObject
{
    public float StunTime;

    public StunObject(float StunTime)
    {
        this.StunTime = StunTime;
    }
}


[System.Serializable]
public class SpeedObject : StatObject
{
    public float speed;

    public SpeedObject(float speed)
    {
        this.speed = speed;
    }
}

[System.Serializable]
public class StaminaRegenObject : StatObject
{
    public float StaminaRegenMultiplier = 1;

    public StaminaRegenObject(float StaminaRegenMultiplier)
    {
        this.StaminaRegenMultiplier = StaminaRegenMultiplier;
    }
}

[System.Serializable]
public class ReloadTimeObject : StatObject
{
    public float ReduceTime = 1;

    public ReloadTimeObject(float ReduceTime)
    {
        this.ReduceTime = ReduceTime;
    }
}

[System.Serializable]
public class StatObject
{
    public int Key;
    public string item;

    public string CurrentType;
    public StatObject()
    {
        CurrentType = GetType().ToString();
    }

}

public abstract class StatsObjectsHandler
{
    public StatObject[] Effects = new StatObject[8]; // There are 8 classes udner StatObject
}