using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : MonoBehaviour, IPause
{
    public GameObject bullet;
    public Transform Aim_Transform;
    public int BaseBulletDamage;
    public int BulletDamage;
    public float BulletLength;
    public Transform FirePoint;
    public float BulletForce;
    public float RecoilTime;
    public float ReloadTime;
    public int Max_ammo_Magasine = 7;
    public int CurrentAmmo;
    [SerializeField]
    protected Animator GunAnimator;

    [SerializeField]
    protected AnimationClip RecoilGun, ReloadingGun;
    [SerializeField]
    protected AudioClip GunSoundReload, GunSoundShoot;

    [SerializeField]
    protected GameObject Magasine;
    [SerializeField]
    protected Transform MagasineDropLocation;
    public GameObject Aim_GameObject;
    private Camera cam;
    public Sprite GunIcon;

    //Detached
    [HideInInspector]
    public bool Detached { get; set; } = false;

    public enum AmmoType
    {
        Light,
        Heavy
    }
    public AmmoType GunAmmoType = AmmoType.Light;


    ///Modifications
    public float ReducedReloadingTime;
    public enum GunState
    {
        Disabled,
        Reloading,
        Shooting,
        Idle,
    }

    [SerializeField]
    protected GunState CurrentGunState = GunState.Idle;

    protected void Awake()
    {
        CurrentAmmo = Max_ammo_Magasine;
        //BulletDamage = BaseBulletDamage;
        GunAnimator = GetComponent<Animator>();
        cam = Camera.main;
    }

    public void SetGunState(GunState state)
    {
        CurrentGunState = state;
    }

    protected virtual void Start()
    {
        GameManager.GameManagerInstance.onpause += UnitPause;
        Detached = false;
    }

    private void OnApplicationQuit()
    {
        if (!Detached) GameManager.GameManagerInstance.onpause -= UnitPause;
        Detached = true;
    }

    protected void OnDestroy()
    {
        if(!Detached) GameManager.GameManagerInstance.onpause -= UnitPause;
    }


    public void UnitPause(bool Pause)
    {
        if (Pause) GetComponent<MonoBehaviour>().enabled = false;

        else GetComponent<MonoBehaviour>().enabled = true;
    }

    protected void OnEnable()
    {
        GameManager.GameManagerInstance.onpause += UnitPause;
        Detached = false;
        SetRecoilAnimationSpeed();
    }

    private void OnDisable()
    {
        if (!Detached) GameManager.GameManagerInstance.onpause -= UnitPause;
        Detached = true;
    }


    // protected void FixedUpdate()
    // {
    //     FollowTheMouse();
    // }

    protected void LateUpdate()
    {
        if (CurrentGunState == GunState.Disabled) return;
        FollowTheMouse();
    }

    public abstract void UseGun();

    protected void FollowTheMouse()
    {
        Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 lookDir = mousePos - new Vector2(Aim_GameObject.transform.position.x, Aim_GameObject.transform.position.y);
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
        Aim_GameObject.transform.rotation = Quaternion.Euler(Aim_GameObject.transform.rotation.x, Aim_GameObject.transform.rotation.y, angle);
    }

   

    protected abstract IEnumerator Fire();

    void SetRecoilAnimationSpeed()
    {
        float RecoilSpeed = RecoilGun.length / RecoilTime;

        GunAnimator.SetFloat("RecoilSpeed", RecoilSpeed);
    }

    public bool SwapWeapon()
    {
        if (GunAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && CurrentGunState == GunState.Idle) return true;
        else return false;
    }
}

public abstract class LightGun : Gun
{

    public virtual IEnumerator ReloadLight()
    {
        CurrentGunState = GunState.Reloading;
        float ReloadingTime = ReloadTime * (1 - ReducedReloadingTime);
        float AnimationReloadingSpeed = ReloadingGun.length / ReloadingTime;
        GunAnimator.SetTrigger("Reloading");
        GunAnimator.SetFloat("ReloadingSpeed", AnimationReloadingSpeed);
        CurrentGunState = GunState.Reloading;

        //play sound
        AudioManager audioman = AudioManager.AudnioManagerInstance;
        audioman.PlaySound(gameObject.transform.position, GunSoundReload);

        GameObject Magasine_ = Instantiate(Magasine, MagasineDropLocation.transform.position, Quaternion.identity);
        Destroy(Magasine_, 1f);


        yield return new WaitForSeconds(ReloadingTime);

        CurrentGunState = GunState.Idle;
        CurrentAmmo = Max_ammo_Magasine;
    }

    public override void UseGun()
    {

        if (CurrentGunState == GunState.Disabled) return;

        if (CurrentAmmo > 0 && Input.GetKey(KeyCode.Mouse0) && CurrentGunState == GunState.Idle)
        {
            CurrentAmmo--;
            StartCoroutine(Fire());
        }

        else if ((CurrentAmmo <= 0 || Input.GetKeyDown("r")) && CurrentGunState == GunState.Idle && GunAmmoType == AmmoType.Light)
        {
            StartCoroutine(ReloadLight());
        }
    }
}

public abstract class HeavyGun : Gun
{
    [SerializeField]
    public int HeavyAmmo;
    public int MaxHeavyAmmo;
    public int Basic_MaxHeavyAmmo;

    protected override void Start()
    {
        base.Start();

        Basic_MaxHeavyAmmo = MaxHeavyAmmo;
    }

    public virtual IEnumerator ReloadHeavy()
    {
        CurrentGunState = GunState.Reloading;
        float ReloadingTime = ReloadTime * (1 - ReducedReloadingTime);
        float AnimationReloadingSpeed = ReloadingGun.length / ReloadingTime;
        GunAnimator.SetTrigger("Reloading");
        GunAnimator.SetFloat("ReloadingSpeed", AnimationReloadingSpeed);
        CurrentGunState = GunState.Reloading;

        //play sound
        AudioManager audioman = AudioManager.AudnioManagerInstance;
        audioman.PlaySound(gameObject.transform.position, GunSoundReload);

        HeavyAmmo += CurrentAmmo;
        int HeavyAmmoToReload = 0;
        if (HeavyAmmo >= Max_ammo_Magasine)
        {
            HeavyAmmo -= Max_ammo_Magasine;
            HeavyAmmoToReload = Max_ammo_Magasine;
        }
        else
        {
            HeavyAmmoToReload = HeavyAmmo;
            HeavyAmmo -= HeavyAmmoToReload;
        }

        GameObject Magasine_ = Instantiate(Magasine, MagasineDropLocation.transform.position, Quaternion.identity);
        Destroy(Magasine_, 1f);


        yield return new WaitForSeconds(ReloadingTime);

        CurrentGunState = GunState.Idle;
        CurrentAmmo = HeavyAmmoToReload;
    }

    public override void UseGun()
    {

        if (CurrentGunState == GunState.Disabled) return;

        if (CurrentAmmo > 0 && Input.GetKey(KeyCode.Mouse0) && CurrentGunState == GunState.Idle)
        {
            CurrentAmmo--;
            StartCoroutine(Fire());
        }

        else if ((HeavyAmmo > 0 && (CurrentAmmo <= 0 || Input.GetKeyDown("r"))) && CurrentGunState == GunState.Idle && GunAmmoType == AmmoType.Heavy)
        {
            StartCoroutine(ReloadHeavy());
        }
    }
}