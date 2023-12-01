using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_1_Rocket : MonoBehaviour
{
    public float RocketSpeed, RocketRange;
    public int RocketDamage;
    public LayerMask CollisionRocketDestroy;

    GameObject Player;

    [SerializeField]
    float ExplosionRadius;
    [SerializeField]
    GameObject Effect;

    // Start is called before the first frame update
    void Start()
    {
        Player = Player_Script.PlayerInstance.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, Player_Script.PlayerInstance.transform.position, RocketSpeed * Time.deltaTime);

        Vector2 direction = Player.transform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if(((int)(Mathf.Pow(2, collision.gameObject.layer)) & CollisionRocketDestroy.value) != 0)
        {
            Collider2D[] Objects = Physics2D.OverlapCircleAll(gameObject.transform.position, ExplosionRadius);

            for (int i = 0; i < Objects.Length; i++)
            {
                if(Objects[i].GetComponent<Unit>() && !Objects[i].GetComponent<Boss_1>())
                    Objects[i].GetComponent<Unit>().Damage(RocketDamage);
            }

            Instantiate(Effect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(gameObject.transform.position, ExplosionRadius);
    }
}
