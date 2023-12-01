using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Gun_Script : LightGun
{
    protected override IEnumerator Fire()
    {
        CurrentGunState = GunState.Shooting;

        Quaternion Rotation = Quaternion.Euler(0, 0, FirePoint.rotation.eulerAngles.z);

        //play sound
        AudioManager audioman = AudioManager.AudnioManagerInstance;
        audioman.PlaySound(gameObject.transform.position, GunSoundShoot);

        GunAnimator.GetComponent<Animator>().SetTrigger("Shoot");
        GameObject projectile = Instantiate(bullet, FirePoint.position, Rotation);
        projectile.GetComponent<Bullet_Script>().Damage = Player_Script.PlayerInstance.CritDamage(BulletDamage);
        projectile.GetComponent<Bullet_Script>().Length = BulletLength;
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.AddForce(projectile.transform.right * BulletForce, ForceMode2D.Impulse);

        

        yield return new WaitForSeconds(RecoilTime);
        if(CurrentGunState != GunState.Disabled)
            CurrentGunState = GunState.Idle;
    }
}
