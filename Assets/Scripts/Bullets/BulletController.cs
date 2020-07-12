using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private Rigidbody rb;

    [HideInInspector] public bool bouncingBullet;

    [HideInInspector] public bool gravityBullet;
    [HideInInspector] public float gravityMultiplier;

    [HideInInspector] public bool canTakeDamage;
    [HideInInspector] public bool onlyHeadshots;

    [HideInInspector] public float damage;
    [HideInInspector] public float headshotDamage;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (gravityBullet)
        {
            rb.AddForce(Physics.gravity * rb.mass * gravityMultiplier);
        }
        else
        {
            //rb.AddForce(Physics.gravity * rb.mass);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "EnemyHead" && canTakeDamage)
        {
            collision.gameObject.GetComponent<EnemyController>().Hit(headshotDamage);
            Destroy(gameObject);
            return;
        }
        
        if(collision.gameObject.tag == "Enemy" && canTakeDamage && !onlyHeadshots)
        {
            collision.gameObject.GetComponent<EnemyController>().Hit(damage);
            Destroy(gameObject);
            return;
        }
        
        if (!bouncingBullet)
        {
            Destroy(gameObject);
            return;
        }
    }
}
