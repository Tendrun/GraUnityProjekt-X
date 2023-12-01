using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Enemy_Script : MonoBehaviour
{
    public LayerMask ObjectCollision;
    public int Damage = 1;
    private Vector2 StartPos;
    public float Length = Mathf.Infinity;
    public GameObject ParticleWallEffect;
    public LayerMask LayerWallEffect;

    private void Start()
    {
        Destroy(gameObject, 30);
        StartPos = transform.position;
    }

    private void Update()
    {
        float dist = Vector2.Distance(StartPos, transform.position);

        if (dist >= Length)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((1 << collision.gameObject.layer) == ObjectCollision.value)
        {
            collision.gameObject.GetComponent<Player_Script>().Damage(Damage);
        }


        if (((int)(Mathf.Pow(2, collision.gameObject.layer)) & LayerWallEffect.value) != 0)
        {
            Destroy(Instantiate(ParticleWallEffect, gameObject.transform.position, new Quaternion(-90, 0, 0, 0)), 2);            
        }

        Destroy(gameObject);
    }
}
