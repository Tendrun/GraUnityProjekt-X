using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : HeavyGun
{
    [SerializeField]
    private int NumberOfProjectiles;
    [SerializeField]
    private float SpreadAngle;



    protected override IEnumerator Fire()
    {
        //animator

        CurrentGunState = GunState.Shooting;
        GunAnimator.SetTrigger("Shoot");

        //bullet spread

        float angleStep = SpreadAngle / NumberOfProjectiles;
        float aimingAngle = FirePoint.rotation.eulerAngles.z;
        float centeringOffset = (SpreadAngle / 2) - (angleStep / 2);

        for (int i = 0; i < NumberOfProjectiles; i++)
        {
            float currentBulletAngle = angleStep * i;

            Quaternion rotation = Quaternion.Euler(new Vector3(0, 0, aimingAngle + currentBulletAngle - centeringOffset));
            GameObject projectile = Instantiate(bullet, FirePoint.position, rotation);


            projectile.GetComponent<Bullet_Script>().Damage = Player_Script.PlayerInstance.CritDamage(BulletDamage);
            projectile.GetComponent<Bullet_Script>().Length = BulletLength;
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            rb.AddForce(projectile.transform.right * BulletForce, ForceMode2D.Impulse);
        }

        //play sound
        AudioManager audioman = AudioManager.AudnioManagerInstance;
        audioman.PlaySound(gameObject.transform.position, GunSoundShoot);

        yield return new WaitForSeconds(RecoilTime);
        if (CurrentGunState != GunState.Disabled)
            CurrentGunState = GunState.Idle;
    }


}
