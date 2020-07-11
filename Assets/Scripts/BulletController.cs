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
            //EnemyHit (headshot)
        }
        
        if(collision.gameObject.tag == "Enemy" && canTakeDamage && !onlyHeadshots)
        {
            //EnemyHit
        }
        
        if (!bouncingBullet)
        {
            Destroy(gameObject);
        }
    }
}
