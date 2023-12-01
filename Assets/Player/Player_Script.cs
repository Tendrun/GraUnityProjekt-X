using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public abstract class Player_Script : Unit
{
    [Header("Player Health Limit")]

    [SerializeField]
    protected int HealthLimit;
    [SerializeField]
    [Tooltip("Time of player being Invulnerable after getting damaged")]
    protected float InvulnerableTime = 0.5f;
    protected bool Invulnerable = false;
    [SerializeField]
    GameObject bloodGameObject;

    //Spritning Properties
    [Header("Player Stats")]
    [Header("Player Sprinting Stats")]
    [Tooltip("This value shows how faster Player will run")]
    public float SprintingSpeedMultiplier = 2;
    [SerializeField]
    protected float BaseStaminaRegenMultiplier = 1;
    [SerializeField]
    protected float StaminaRegenMultiplier = 1;
    [SerializeField]
    protected Dictionary<int, StaminaRegenObject> StaminaRegenObjects = new Dictionary<int, StaminaRegenObject>();
    [SerializeField]
    protected float Base_Stamina;
    [SerializeField]
    protected float Max_stamina;
    [SerializeField]
    protected float Current_Stamina;
    protected float SprintingSpeed_Real = 1;

    public bool PenaltyForRunning = false;

    //Gun Properties
    [Header("Player Guns")]
    [SerializeField]
    protected GameObject[] Guns;
    protected int GunNumber = 1;
    [SerializeField]
    protected Gun gun;
    [SerializeField]
    protected Dictionary<int, ReloadTimeObject> ReloadTimeObjects = new Dictionary<int, ReloadTimeObject>();

    // Gun handler
    private bool GunEnable = false;

    [Header("Player UI")]

    //HUD
    public GameObject Player_HUD;
    private Player_HUD_Data _Player_HUD;
    protected TextMeshProUGUI AmmoText;
    public List<Image> Hearts;
    protected GameObject Heart;
    protected GameObject _Heart;
    protected Sprite EmptyHeart;
    protected Sprite FullHeart;
    protected GameObject SortingLayerHearts;
    protected GameObject Portrait;
    protected GameObject[] Portraits = new GameObject[5];
    protected bool PortraitDamaged = false;
    protected float intervalsCheckingAmmo = 0.5f;
    protected Image StaminaImage;

    //Ammo HUD
    protected Text LightAmmoText;
    protected Text HeavyAmmoText;
    protected Image GunIcon;

    //RGB2
    protected Rigidbody2D Player_RB;

    //Player State
    protected bool DeadPlayer = false;

    [Header("Player Passive")]
    //Player's passive
    [SerializeField]
    protected GameObject PlayerPassiveItem;
    public float PlayerPassiveRegenTime;
    [SerializeField]
    protected float PlayerPassiveCurrentTime;
    [SerializeField]
    protected bool PlayerPassiveTimerBool = false;
    protected bool Stun;


    //Player Singleton
    #region Singleton
    protected static Player_Script _PlayerInstance;

    public static Player_Script PlayerInstance
    {
        get
        {
            return _PlayerInstance;
        }
    }

    #endregion

    protected new void Awake()
    {
        base.Awake();

        Player_RB = GetComponent<Rigidbody2D>();
        _PlayerInstance = GetComponent<Player_Script>();
        CountStaminaOnStart();

        gun = Guns[GunNumber].GetComponentInChildren<Gun>();
    }

    private void OnEnable()
    {
        //Check if there is another Player HUD
        if (!FindObjectOfType<Player_HUD_Data>())
        {

            GameObject obj = Instantiate(Player_HUD);

            _Player_HUD = obj.GetComponent<Player_HUD_Data>();
            LoadHUDData();
        }
        else
        {
            _Player_HUD = FindObjectOfType<Player_HUD_Data>();
            LoadHUDData();
        }
    }

    protected new void Start()
    {
        base.Start();

        StartCoroutine(CheckAmmo(intervalsCheckingAmmo));

        PlayerPassiveCurrentTime = PlayerPassiveRegenTime;
        SwapWeapons(GunNumber);

        //start Passive
        StartCoroutine(PlayerPassive());
    }

    //Interaction variables
    bool IsTalking = false;
    Interactable_Object ObjToTalk = null;

    // Update is called once per frame
    protected void Update()
    {
        //get the Input from Horizontal axis
        float horizontalInput = Input.GetAxis("Horizontal");
        //get the Input from Vertical axis
        float verticalInput = Input.GetAxis("Vertical");
        if (Stun)
        {
            GunEnable = false;
            Debug.Log(Stun);
            DisableWeapon();
            return;
        }

        else if (!GunEnable)
        {
            GunEnable = true;
            EnableWeapon();
        }

        if (!DeadPlayer)
        {
            Flip();
            SprintingAnimation(horizontalInput, verticalInput);
            Sprinting();
            MovingPlayer(horizontalInput, verticalInput);

            //search for interactions

            if (ObjToTalk && IsTalking && Vector2.Distance(ObjToTalk.gameObject.transform.position, gameObject.transform.position) >= 3)
            {
                Debug.Log("End");
                IsTalking = false;
                ObjToTalk.EndDialogue();
            }

            if (!IsTalking && Input.GetKeyDown(KeyCode.E))
                ObjToTalk = SearchForDialog(ref IsTalking);
            else if (IsTalking && Input.GetKeyDown(KeyCode.E))
                Talk(ObjToTalk);


            //SwapWeapons
            if (Input.GetKeyDown("space") && gun.SwapWeapon())
            {
                if (GunNumber + 1 < Guns.Length) SwapWeapons(++GunNumber);
                else SwapWeapons(GunNumber = 0);
            }
        }

        HUD_Update();
    }

    protected void LateUpdate()
    {
        if (!DeadPlayer) gun.UseGun();
    }

    #region Crit chance

    [Header("Crit attributes")]

    [SerializeField]
    protected int CritChance;
    [SerializeField]
    protected float CritDamageMultiplier;

    protected bool crit()
    {
        int number = Random.Range(0, 101);

        if (number > CritChance) return false;
        else return true;
    }

    /// <summary>
    /// Returns normal damage if crit wasnt lucky
    /// </summary>
    public int CritDamage(int Damage)
    {
        if (!crit()) 
        {
            //WasCrit = false;
            return Damage;
        }

        else
        {
            //WasCrit = true;
            return Mathf.CeilToInt(Damage * CritDamageMultiplier);
        }

    }

    #endregion

    #region Gold/Items

    [Header("Player Gold/Items")]
    [SerializeField]
    protected int gold;
    [SerializeField]
    protected GameObject ItemSlot;
    [SerializeField]
    public GameObject Inventory;

    //Items used
    [SerializeField]
    protected Dictionary<int, Item> Items = new Dictionary<int, Item>();

    public Dictionary<int, Item> ShowItems()
    {
        
        foreach (Item item in Items.Values)
        {
            Debug.Log("ItemCount = " + item.name);
            Debug.Log("ItemCount = " + Items.Count);
            Debug.Log("Item IndexInEq = " + item.IndexInEq);
        }
 
        return Items;
    }

    public void ShowItems(int x)
    {

        foreach (Item item in Items.Values)
        {
            Debug.Log("ItemCount = " + item);
            Debug.Log("ItemCount = " + Items.Count);
            Debug.Log("Item IndexInEq = " + item.IndexInEq);
        }

    }

    public void ChangeGoldAmount(int amount)
    {
        Debug.Log("gold = " + gold);
        gold += amount;
    }

    public int CheckGold()
    {
        return gold;
    }

    public void GiveItem(Item item)
    {
        //Set vairables for item 
        GameObject itemobj = Instantiate(item.item);
        itemobj.transform.SetParent(Inventory.transform);
        int index = ReturnKey(Items);

        itemobj.GetComponent<Item>().IndexInEq = index;

        Items.Add(index, itemobj.GetComponent<Item>());
        itemobj.GetComponent<Item>().ItemAddStats();

        
        itemobj.GetComponent<SpriteRenderer>().enabled = false;
        
        //GameObject ItemSlot_ = Instantiate(ItemSlot, _Player_HUD.transform);
        //ItemSlot_.GetComponent<ItemSlotManager>().ItemImage.sprite = itemobj.GetComponent<Item>().ItemSprite;
        
    }

    public void DeleteItem(int Key)
    {
        StatObject[] Stats = Items[Key].ItemDelete();

        for (int i = 0; i < Stats.Length; i++)
        {
            DeleteStat(Stats[i]);
        }

        GameObject ItemToDelete = null;

        for (int i = 0; i < Inventory.transform.childCount; i++)
        {
            Debug.Log("item.IndexInEq = " + Inventory.transform.GetChild(i).GetComponent<ItemSetDescription>().IndexInEq);

            if (Inventory.transform.GetChild(i).GetComponent<ItemSetDescription>().IndexInEq == Key)
            {
                ItemToDelete = Inventory.transform.GetChild(i).gameObject;
                Debug.Log("item.IndexInEq = " + Inventory.transform.GetChild(i).GetComponent<ItemSetDescription>().IndexInEq);
                break;
            }
        }

        Items.Remove(Key);
        Destroy(ItemToDelete);

        //GameObject ItemSlot_ = Instantiate(ItemSlot, _Player_HUD.transform);
        //ItemSlot_.GetComponent<ItemSlotManager>().ItemImage.sprite = item.ItemSprite;
    }

    void DeleteStat(StatObject obj)
    {
        int Key = obj.Key;


        if (obj.GetType() == typeof(LightDamageObject)) {
            LightDamageObjects.Remove(Key);
            CountLightWeaponDamage();
        }

        else if (obj.GetType() == typeof(MaxHeavyAmmoObject))
        {
            MaxHeavyAmmoObjectbjects.Remove(Key);
            CountMaxHeavyAmmo();
        }

        else if(obj.GetType() == typeof(MaxHealthObject))
        {
            MaxHealthObjects.Remove(Key);
            CountMaxHealth();
        }

        else if(obj.GetType() == typeof(SpeedObject))
        {
            SpeedObjects.Remove(Key);
            CountLightWeaponDamage();
        }

        else if(obj.GetType() == typeof(ReloadTimeObject))
        {
            ReloadTimeObjects.Remove(Key);
            CountReloadTimeDecrease();
        }

        else if(obj.GetType() == typeof(StaminaRegenObject))
        {
            StaminaRegenObjects.Remove(Key);
            CountStaminaRegen();
        }
    }

    #endregion

    #region Dialog

    Interactable_Object SearchForDialog(ref bool IsTalking)
    {
        DialogueManager DialogMan = DialogueManager.GameDialogueManagerInstance;


        if (DialogMan.InteractableObjects.Count <= 0)
        {
            Debug.Log(DialogMan.InteractableObjects.Count);
            return null;
        }

        Interactable_Object ClosestObj = DialogMan.InteractableObjects[0];
        float distance = Vector2.Distance(DialogMan.InteractableObjects[0].transform.position, gameObject.transform.position);

        for (int i = 1; i < DialogMan.InteractableObjects.Count; i++)
        {
            if (distance > Vector2.Distance(DialogMan.InteractableObjects[i].transform.position, gameObject.transform.position))
            {
                distance = Vector2.Distance(DialogMan.InteractableObjects[i].transform.position, gameObject.transform.position);
                ClosestObj = DialogMan.InteractableObjects[i];
            }
        }

        Debug.Log(ClosestObj + " Distance = " + distance);

        if (ClosestObj && distance < 3)
        {
            IsTalking = true;
            ClosestObj.StartDialogue();
            return ClosestObj;
        }

        else
        {
            return null;
        }
    }

    public void EndConversation()
    {
        IsTalking = false;
    }

    void Talk(Interactable_Object ObjToTalk)
    {
        ObjToTalk.NextSentence(ref IsTalking);
        if (IsTalking)
        {
            ObjToTalk = null;
        }

    }

    #endregion

    #region Player Passive

    public abstract IEnumerator PlayerPassive();


    #endregion

    #region Guns

    public Gun[] GetGuns()
    {
        Gun[] guns = new Gun[Guns.Length];

        for (int i = 0; i < Guns.Length; i++)
        {
            guns[i] = Guns[i].GetComponentInChildren<Gun>();
        }

        return guns;
    }


    protected void SwapWeapons(int NumberWeapon)
    {
        for (int i = 0; i < Guns.Length; i++)
        {
            if (i == NumberWeapon)
            {
                Guns[i].SetActive(true);
                gun = Guns[i].GetComponentInChildren<Gun>();

                ChangeGunIcon(gun.GunIcon);
                CountReloadTimeDecrease();
            }

            else Guns[i].SetActive(false);
        }
    }

    //Set Weapon State

    public void DisableWeapon()
    {
        gun.SetGunState(Gun.GunState.Disabled);
    }

    public void EnableWeapon()
    {
        gun.SetGunState(Gun.GunState.Idle);
    }

    #endregion

    #region Hud

    //Gun_HUD
    IEnumerator CheckAmmo(float InvokeCheckAmmo)
    {
        if (DeadPlayer) yield break;

        for (int i = 0; i < Guns.Length; i++)
        {
            if (GetGuns()[i].GunAmmoType == Gun.AmmoType.Light) SetAmmoTextLight("Infinity");
            else if (GetGuns()[i].GunAmmoType == Gun.AmmoType.Heavy) SetAmmoTextHeavy(GetGuns()[i].GetComponent<HeavyGun>().HeavyAmmo, GetGuns()[i].GetComponent<HeavyGun>().MaxHeavyAmmo);
        }

        yield return new WaitForSeconds(InvokeCheckAmmo);

        StartCoroutine(CheckAmmo(InvokeCheckAmmo));
    }

    public void ChangeGunIcon(Sprite Icon)
    {
        GunIcon.sprite = Icon;
    }

    public void SetAmmoTextLight(string LightAmmo)
    {
        LightAmmoText.text = LightAmmo;
    }

    public void SetAmmoTextHeavy(int HeavyAmmo, int MaxHeavyAmmo)
    {
        HeavyAmmoText.text = HeavyAmmo.ToString() + " / " + MaxHeavyAmmo.ToString();
    }

    protected void LoadHUDData()
    {
        AmmoText = _Player_HUD.AmmoText;
        Heart = _Player_HUD.Heart;
        EmptyHeart = _Player_HUD.EmptyHeart;
        FullHeart = _Player_HUD.FullHeart;
        SortingLayerHearts = _Player_HUD.SortingLayerHearts;
        Portrait = _Player_HUD.Portrait;
        GunIcon = _Player_HUD.GunIcon;
        HeavyAmmoText = _Player_HUD.HeavyAmmoText;
        LightAmmoText = _Player_HUD.LightAmmoText;
        GunIcon = _Player_HUD.GunIcon;
        StaminaImage = _Player_HUD.StaminaImage;

        for (int i = 0; i < _Player_HUD.Portraits.Length; i++)
        {
            Portraits[i] = _Player_HUD.Portraits[i];
        }
    }

    protected void HUD_Update()
    {
        AmmoText.text = gun.CurrentAmmo + "/" + gun.Max_ammo_Magasine;


        if (previousHP > CurrentHealth)
        {
            TriggerOnHPChange();
        }
    }

    public override void TriggerOnHPChange()
    {
        if (CurrentHealth > HealthLimit)
        {
            CurrentHealth = HealthLimit;
            previousHP = HealthLimit;
        }
        if (MaxHealth > HealthLimit) MaxHealth = HealthLimit;


        float percentageHP = (float)CurrentHealth / (float)MaxHealth;

        for (int i = 0; i < Hearts.Count; i++)
        {
            if (i >= CurrentHealth)
            {
                Hearts[i].sprite = EmptyHeart;
            }

            else if (i < CurrentHealth)
            {
                Hearts[i].sprite = FullHeart;
            }
        }

        if (DeadPlayer)
        {
            Portrait.GetComponent<Animator>().runtimeAnimatorController = null;
            Portrait.GetComponent<Image>().sprite = Portraits[4].GetComponent<Image>().sprite;
        }

        else if (PortraitDamaged)
        {

        }

        else if (previousHP > CurrentHealth)
        {
            previousHP = CurrentHealth;
            StartCoroutine(ReactionChange());
            StartCoroutine(PlayerColorChange());
        }

        else if (percentageHP >= 0 && percentageHP < 0.34f)
        {
            Portrait.GetComponent<Animator>().runtimeAnimatorController = Portraits[0].GetComponent<Animator>().runtimeAnimatorController;
            Portrait.GetComponent<Image>().sprite = Portraits[0].GetComponent<Image>().sprite;
        }

        else if (percentageHP >= 0.34f && percentageHP < 0.70f)
        {
            Portrait.GetComponent<Animator>().runtimeAnimatorController = Portraits[1].GetComponent<Animator>().runtimeAnimatorController;
            Portrait.GetComponent<Image>().sprite = Portraits[1].GetComponent<Image>().sprite;
        }

        else if (percentageHP >= 0.70f && percentageHP <= 1)
        {
            Portrait.GetComponent<Animator>().runtimeAnimatorController = Portraits[2].GetComponent<Animator>().runtimeAnimatorController;
            Portrait.GetComponent<Image>().sprite = Portraits[2].GetComponent<Image>().sprite;
        }
    }

    IEnumerator PlayerColorChange()
    {
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), true);

        SpriteRenderer spr = gameObject.GetComponent<SpriteRenderer>();
        for (int i = 0; i < InvulnerableTime / 0.2; i++)
        {
            spr.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            spr.color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }
        spr.color = Color.white;

        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Enemy"), false);
    }

    public override void HUD_Health()
    {
        if (MaxHealth > SortingLayerHearts.GetComponentsInChildren<Image>().Length)
        {
            for (int i = SortingLayerHearts.transform.childCount; i < MaxHealth; i++)
            {
                _Heart = Instantiate(Heart, SortingLayerHearts.transform);
                Hearts.Add(_Heart.GetComponentInChildren<Image>());
            }
        }



        else if (MaxHealth < SortingLayerHearts.GetComponentsInChildren<Image>().Length)
        {
            for (int i = SortingLayerHearts.transform.childCount; i > MaxHealth; i--)
            {
                Destroy(Hearts[i - 1].gameObject);
                Hearts.RemoveAt(i - 1);
            }
        }
    }

    #endregion

    #region MovingPlayer

    protected void SprintingAnimation(float horizontalInput, float verticalInput)
    {
        float speed = 0;

        if (horizontalInput == 1 || horizontalInput == -1)
            speed = SprintingSpeed_Real;

        else if (verticalInput == 1 || verticalInput == -1)
            speed = SprintingSpeed_Real;

        gameObject.GetComponent<Animator>().SetFloat("Speed", speed);

    }

    protected void Sprinting()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Current_Stamina >= 0 && !PenaltyForRunning)
        {
            Current_Stamina -= Time.deltaTime;
        }

        else if (Current_Stamina <= Max_stamina && !(Current_Stamina <= 0))
        {
            Current_Stamina += StaminaRegenMultiplier * (Time.deltaTime / 3);
        }

        else if (Current_Stamina <= 0)
        {
            Current_Stamina += StaminaRegenMultiplier * (Time.deltaTime / 3);

            StartCoroutine(SprintingPenalty());
        }

        if (Input.GetKey(KeyCode.LeftShift) && !PenaltyForRunning)
        {
            SprintingSpeed_Real = SprintingSpeedMultiplier;
        }

        else if (!Input.GetKey(KeyCode.LeftShift)) SprintingSpeed_Real = 1;

        else if (PenaltyForRunning)
        {
            SprintingSpeed_Real = 1;
        }

        //HUD Sprinting
        StaminaImage.fillAmount = Current_Stamina / Max_stamina;
    }

    protected void MovingPlayer(float horizontalInput, float verticalInput)
    {
        transform.position = transform.position + new Vector3(horizontalInput * (CurrentSpeed) * SprintingSpeed_Real * Time.deltaTime, verticalInput * (CurrentSpeed) * SprintingSpeed_Real * Time.deltaTime, 0);
    }


    protected void Flip()
    {
        Vector2 MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);


        if (MousePosition.x > transform.position.x)
        {
            Player_RB.transform.localScale = new Vector3(1, 1, 1);
            gun.Aim_Transform.transform.localScale = new Vector3(1, 1, 1);
        }

        else if (MousePosition.x < transform.position.x)
        {
            Player_RB.transform.localScale = new Vector3(-1, 1, 1);
            gun.Aim_Transform.transform.localScale = new Vector3(-1, -1, 1);
        }
    }

    #endregion

    #region Player Components
    public override void Damage(int damage)
    {
        if (!Invulnerable && damage > 0)
        {
            GameObject blood = Instantiate(bloodGameObject, transform.position, Quaternion.identity);
            Destroy(blood, 2);
            CurrentHealth -= damage;
            StartCoroutine(InvulnerableCounter());
        }


        if (CurrentHealth <= 0 && !DeadPlayer)
        {
            Death();
        }



        //HUD_Health();
    }

    protected IEnumerator InvulnerableCounter()
    {
        Invulnerable = true;
        yield return new WaitForSeconds(InvulnerableTime);
        Invulnerable = false;
    }

    protected void Death()
    {
        //Shooting_Script.enabled = false;
        DeadPlayer = true;
        gameObject.GetComponent<Animator>().SetBool("Death", true);
        gun.gameObject.SetActive(false);
        GameManager.GameManagerInstance.ENDGAME();
    }

    protected IEnumerator ReactionChange()
    {
        Portrait.GetComponent<Animator>().runtimeAnimatorController = Portraits[3].GetComponent<Animator>().runtimeAnimatorController;
        Portrait.GetComponent<Image>().sprite = Portraits[3].GetComponent<Image>().sprite;

        PortraitDamaged = true;

        yield return new WaitForSeconds(1);

        PortraitDamaged = false;
        TriggerOnHPChange();
    }

    #endregion

    #region StatsHandler
    [SerializeField]
    protected Dictionary<int, LightDamageObject> LightDamageObjects = new Dictionary<int, LightDamageObject>();
    public void IncreaseLightWeaponDamage(int Damage)
    {
        LightDamageObject obj = new LightDamageObject(Damage);

        int index = ReturnKey(LightDamageObjects);

        LightDamageObjects.Add(index, obj);
        CountLightWeaponDamage();
    }

    public StatObject IncreaseLightWeaponDamage(int Damage, int x = 0)
    {
        LightDamageObject obj = new LightDamageObject(Damage);

        int index = ReturnKey(LightDamageObjects);
        obj.Key = index;
        LightDamageObjects.Add(index, obj);
        CountLightWeaponDamage();

        return obj;
    }

    public void CountLightWeaponDamage()
    {
        gun.BulletDamage = gun.BaseBulletDamage;
        int PassedValue = 0;

        foreach (LightDamageObject obj in LightDamageObjects.Values)
        {
            PassedValue += obj.Damage;
        }

        for (int i = 0; i < Guns.Length; i++)
        {
            Gun gun = Guns[i].GetComponentInChildren<Gun>();
            if (gun.GunAmmoType == Gun.AmmoType.Light)
                gun.GetComponent<LightGun>().BulletDamage += PassedValue;
        }
    }


    [SerializeField]
    protected Dictionary<int, MaxHeavyAmmoObject> MaxHeavyAmmoObjectbjects = new Dictionary<int, MaxHeavyAmmoObject>();
    public void IncreaseMaxHeavyAmmo(int HeavyAmmoAmount)
    {
        MaxHeavyAmmoObject obj = new MaxHeavyAmmoObject(HeavyAmmoAmount);

        int index = ReturnKey(MaxHeavyAmmoObjectbjects);

        MaxHeavyAmmoObjectbjects.Add(index, obj);
        CountMaxHeavyAmmo();
    }

    public StatObject IncreaseMaxHeavyAmmo(int HeavyAmmoAmount, int x = 0)
    {
        MaxHeavyAmmoObject obj = new MaxHeavyAmmoObject(HeavyAmmoAmount);

        int index = ReturnKey(MaxHeavyAmmoObjectbjects);
        obj.Key = index;
        MaxHeavyAmmoObjectbjects.Add(index, obj);
        CountMaxHeavyAmmo();

        return obj;
    }

    public void CountMaxHeavyAmmo()
    {
        gun.ReducedReloadingTime = 0;
        int PassedValue = 0;

        foreach (MaxHeavyAmmoObject obj in MaxHeavyAmmoObjectbjects.Values)
        {
            PassedValue += obj.Ammo;
        }

        for (int i = 0; i < Guns.Length; i++)
        {
            Gun gun = Guns[i].GetComponentInChildren<Gun>();
            if (gun.GunAmmoType == Gun.AmmoType.Heavy)
            {
                gun.GetComponent<HeavyGun>().MaxHeavyAmmo = PassedValue + gun.GetComponent<HeavyGun>().Basic_MaxHeavyAmmo;
            }
        }
    }

    //Heavy Ammo Add
    public void AddHeavyAmmo(int HeavyAmmoAmount)
    {
        for (int i = 0; i < Guns.Length; i++)
        {
            Gun gun = Guns[i].GetComponentInChildren<Gun>();
            if (gun.GunAmmoType == Gun.AmmoType.Heavy && (gun.GetComponent<HeavyGun>().HeavyAmmo + HeavyAmmoAmount) <= gun.GetComponent<HeavyGun>().MaxHeavyAmmo)
                gun.GetComponent<HeavyGun>().HeavyAmmo += HeavyAmmoAmount;
            else if (gun.GunAmmoType == Gun.AmmoType.Heavy && (gun.GetComponent<HeavyGun>().HeavyAmmo + HeavyAmmoAmount) > gun.Max_ammo_Magasine)
                gun.GetComponent<HeavyGun>().HeavyAmmo = gun.GetComponent<HeavyGun>().MaxHeavyAmmo;
        }
    }

    public void AddStaminaRegen(float StaminaRegenMultiplier)
    {
        StaminaRegenObject StaminaRegenObject = new StaminaRegenObject(StaminaRegenMultiplier);


        int index = ReturnKey(StaminaRegenObjects);

        StaminaRegenObjects.Add(index, StaminaRegenObject);
        CountStaminaRegen();
    }

    
    public StatObject AddStaminaRegen(float StaminaRegenMultiplier, int x = 0)
    {
        StaminaRegenObject StaminaRegenObject = new StaminaRegenObject(StaminaRegenMultiplier);


        int index = ReturnKey(StaminaRegenObjects);
        StaminaRegenObject.Key = index;
        StaminaRegenObjects.Add(index, StaminaRegenObject);
        CountStaminaRegen();

        return StaminaRegenObject;
    }
    

    //Count Stamina

    protected void CountStaminaOnStart()
    {
        Max_stamina = Base_Stamina;
        Current_Stamina = Max_stamina;
    }
    protected void CountStaminaRegen()
    {
        StaminaRegenMultiplier = 1;

        foreach (StaminaRegenObject obj in StaminaRegenObjects.Values)
        {
            if (obj.StaminaRegenMultiplier <= 0)
            {
                Debug.Log("THE MULTIPLIER NUMBER CANT BE LESS OR EQUUAL TO ZERO");
            }

            StaminaRegenMultiplier += obj.StaminaRegenMultiplier;
        }
    }

    protected IEnumerator SprintingPenalty()
    {
        PenaltyForRunning = true;
        StaminaImage.color = Color.red;

        yield return new WaitUntil(() => Current_Stamina >= Max_stamina);

        StaminaImage.color = Color.white;
        PenaltyForRunning = false;
    }

    //Reload Time

    public void AddReloadTimeDivider(float ReloadTimeDivider)
    {
        ReloadTimeObject ReloadTimeObject = new ReloadTimeObject(ReloadTimeDivider);

        int index = ReturnKey(ReloadTimeObjects);

        ReloadTimeObjects.Add(index, ReloadTimeObject);
        CountReloadTimeDecrease();
    }

    public StatObject AddReloadTimeDivider(float ReloadTimeDivider, int x = 0)
    {
        ReloadTimeObject ReloadTimeObject = new ReloadTimeObject(ReloadTimeDivider);

        int index = ReturnKey(ReloadTimeObjects);

        ReloadTimeObject.Key = index;
        ReloadTimeObjects.Add(index, ReloadTimeObject);
        CountReloadTimeDecrease();

        return ReloadTimeObject;
    }

    public void CountReloadTimeDecrease()
    {
        gun.ReducedReloadingTime = 0;
        float PassedValue = 0;

        foreach (ReloadTimeObject obj in ReloadTimeObjects.Values)
        {
            PassedValue += obj.ReduceTime;
        }

        gun.ReducedReloadingTime = Mathf.Clamp(PassedValue, 0, 0.99f);
    }

    //Stun State

    protected void ReturnStunState()
    {
        if (StunObjects.Count > 0) Stun = true;
        else Stun = false;
    }

    public void StunUnit(float duration)
    {
        StunObject StunObj = new StunObject(duration);

        int index = ReturnKey(StunObjects);
        StunObjects.Add(index, StunObj);
        StartCoroutine(CountStun(duration, index));
    }


    public IEnumerator CountStun(float time, int i)
    {
        ReturnStunState();
        yield return new WaitForSeconds(time);
        //Remove Object

        StunObjects.Remove(i);

        ReturnStunState();
    }

    #endregion
}

public abstract class Unit : StatsHandler, IPause, IDamage
{
    [HideInInspector]
    public bool Detached { get; set; } = false;
    public virtual void Damage(int damage)
    {
        CurrentHealth -= damage;

        if (CurrentHealth <= 0)
        {
            Debug.Log("Death");
        }

        //HUD_Health();
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        GameManager.GameManagerInstance.onpause += UnitPause;
    }

    protected void OnApplicationQuit()
    {
        GameManager.GameManagerInstance.onpause -= UnitPause;
        Detached = true;
    }

    protected void OnDestroy()
    {
        //if(!Detached) GameManager.GameManagerInstance.onpause -= UnitPause;
    }

    public void UnitPause(bool Pause)
    {
        if (Pause) GetComponent<MonoBehaviour>().enabled = false;

        else GetComponent<MonoBehaviour>().enabled = true;
    }
}

public class StatsHandler : UnitStats
{
    [Header("Unit Additonal Stats")]
    [SerializeField]
    protected Dictionary<int, StunObject> StunObjects = new Dictionary<int, StunObject>();
    [SerializeField]
    protected Dictionary<int, SpeedObject> SpeedObjects = new Dictionary<int, SpeedObject>();

    protected virtual void Awake()
    {
        
    }

    protected virtual void Start()
    {
        CountHpOnStart();
        CountSpeed();
    }

    #region HealthBased

    protected void CountHpOnStart()
    {
        CurrentHealth = BaseHealth;
        previousHP = BaseHealth;
        MaxHealth = BaseHealth;
        HUD_Health();
        TriggerOnHPChange();
    }

    public void HealUnit(int Heal)
    {
        if (CurrentHealth + Heal <= MaxHealth)
        {
            CurrentHealth += Heal;
            previousHP += Heal;
            if (previousHP > MaxHealth) previousHP = MaxHealth;

            TriggerOnHPChange();
        }
        else if (CurrentHealth + Heal >= MaxHealth)
        {
            CurrentHealth = MaxHealth;
            previousHP = MaxHealth;
        }
    }
    protected Dictionary<int, MaxHealthObject> MaxHealthObjects = new Dictionary<int, MaxHealthObject>();

    public void AddMaxHealthUnit(int health)
    {
        MaxHealthObject obj = new MaxHealthObject(health);


        int index = ReturnKey(MaxHealthObjects);
        obj.Key = index;
        MaxHealthObjects.Add(index, obj);
        CountMaxHealth();
    }

    public StatObject AddMaxHealthUnit(int health, int x = 0)
    {
        MaxHealthObject obj = new MaxHealthObject(health);


        int index = ReturnKey(MaxHealthObjects);
        obj.Key = index;
        MaxHealthObjects.Add(index, obj);
        CountMaxHealth();

        return obj;
    }

    public void CountMaxHealth()
    {
        MaxHealth = BaseHealth;
        int PassedValue = BaseHealth;

        foreach (MaxHealthObject obj in MaxHealthObjects.Values)
        {
            PassedValue += obj.health;
        }

        MaxHealth = PassedValue;
        CurrentHealth = PassedValue;
        previousHP = PassedValue;


        HUD_Health();
        TriggerOnHPChange();
    }


    public virtual void TriggerOnHPChange()
    {

    }

    public virtual void HUD_Health()
    {

    }

    #endregion


    #region Speed Based

    public void CountSpeedOnStart()
    {
        CountSpeed();
    }

    public void AddSpeed(float speed)
    {
        SpeedObject speedObject = new SpeedObject(speed);

        int index = ReturnKey(SpeedObjects);


        SpeedObjects.Add(index, speedObject);
        CountSpeed();
    }

    public StatObject AddSpeed(float speed, int x = 0)
    {
        SpeedObject speedObject = new SpeedObject(speed);

        int index = ReturnKey(SpeedObjects);

        speedObject.Key = index;
        SpeedObjects.Add(index, speedObject);
        CountSpeed();

        return speedObject;
    }

    public virtual void CountSpeed()
    {
        AdditionalSpeed = 0;

        foreach (SpeedObject obj in SpeedObjects.Values)
        {
            AdditionalSpeed += obj.speed;
        }

        CurrentSpeed = AdditionalSpeed + BaseMovementSpeed;
    }

    //KeyManager

    protected int ReturnKey<T>(Dictionary<int, T> obj)
    {
        int Index = 0;
        while (obj.ContainsKey(Index))
        {
            Index++;
        }

        return Index;
    }

    #endregion
}


public class UnitStats : MonoBehaviour
{
    [Header("Unit Basic Stats")]

    [Header("Unit Health")]
    [SerializeField]
    protected int MaxHealth;
    [SerializeField]
    protected int BaseHealth;
    [SerializeField]
    protected int CurrentHealth;
    [SerializeField]
    protected int previousHP;

    [Header("Unit Speed")]
    [SerializeField]
    protected float CurrentSpeed;
    [SerializeField]
    protected float BaseMovementSpeed;
    [SerializeField]
    protected float AdditionalSpeed;
}

public interface IDamage
{
    public abstract void Damage(int damage);
}