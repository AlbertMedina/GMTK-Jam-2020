﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Controls")]
    public KeyCode forwardKey;
    public KeyCode backwardsKey;
    public KeyCode rightKey;
    public KeyCode leftKey;
    public KeyCode jumpKey;

    [Header("Stats")]
    public float initialHealth;
    private float health;

    [Header("Movement")]
    public float movementSpeed;
    public float jumpingSpeed;
    public GP_JumpAssistantTest _jumpAssistant;

    [Header("Rotation")]
    public Transform pitchRotator;
    public float maxPitchRotation;
    public float minPitchRotation;
    public float pitchRotationSpeed;
    public float yawRotationSpeed;

    [Header("Shooting")]
    public BulletController bullet;
    public BulletController bulletGravity;
    public BulletController bulletBouncing;
    public Transform firePoint;
    public float bulletSpeed;
    public float minTimeBetweenShots;
    public float damage;
    public float headshotDamage;

    [Header("VisualShooting")]
    public Animator gunAnim;
    public ParticleSystem basicShotParts;
    public ParticleSystem blobShotParts;
    public ParticleSystem ricochetShotParts;
    public float particlesTime = 0.3f;
    public CameraShake cameraShake;

    private CharacterController characterController;
    private RoundRules roundRules;

    private float currentTime;

    private float yawRotation;
    private float pitchRotation;

    private float verticalSpeed;
    private bool isGrounded;

    //Shooting Rules
    private bool invertedMovement = false;
    private bool bouncingBullets = false;
    private bool gravityBullets = false;
    private bool onlyOneBullet = false;
    private bool bulletUsed = false;

    //Movement Rules
    private bool invertedAiming = false;

    //Winning Condition Rules
    private bool catchTheFlag = false;   
    private bool onlyHeadshots = false;
    [HideInInspector] public bool winByDying = false;

    [HideInInspector] public bool waitToStart = true;

    public enum ShootingRules
    {
        NONE,
        BOUNCING_BULLETS,
        GRAVITY_BULLETS,
        INVERTED_MOUSE,
        ONLY_ONE_BULLET
    }

    public enum MovementRules
    {
        NONE,
        SLOW_MOTION,
        HYPERFAST,
        INVERTED_CONTROLS,
    }

    public enum WinningRules
    {
        NONE,
        WIN_BY_DYING,
        CATCH_THE_FLAG,
        ONLY_HEADSHOTS,
    }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        roundRules = GetComponent<RoundRules>();
    }

    void Update()
    {
        if (!waitToStart)
        {
            #region Rotation

            float mouseAxisY;
            float mouseAxisX;

            if (invertedAiming)
            {
                mouseAxisY = -Input.GetAxis("Mouse Y");
                mouseAxisX = -Input.GetAxis("Mouse X");
            }
            else
            {
                mouseAxisY = Input.GetAxis("Mouse Y");
                mouseAxisX = Input.GetAxis("Mouse X");
            }

            pitchRotation += mouseAxisY * pitchRotationSpeed;
            pitchRotation = Mathf.Clamp(pitchRotation, minPitchRotation, maxPitchRotation);


            yawRotation += mouseAxisX * yawRotationSpeed;

            transform.rotation = Quaternion.Euler(0.0f, yawRotation, 0.0f);
            pitchRotator.localRotation = Quaternion.Euler(-pitchRotation, 0.0f, 0.0f);
            #endregion
            #region Movement
            Vector3 forwardVector = new Vector3(Mathf.Sin(yawRotation * Mathf.Deg2Rad), 0.0f, Mathf.Cos(yawRotation * Mathf.Deg2Rad));
            Vector3 rightVector = new Vector3(Mathf.Sin((yawRotation + 90.0f) * Mathf.Deg2Rad), 0.0f, Mathf.Cos((yawRotation + 90.0f) * Mathf.Deg2Rad));

            Vector3 movement;

            if (Input.GetKey(forwardKey))
            {
                if (invertedMovement)
                {
                    movement = -forwardVector;
                }
                else
                {
                    movement = forwardVector;
                }
            }
            else if (Input.GetKey(backwardsKey))
            {
                if (invertedMovement)
                {
                    movement = forwardVector;
                }
                else
                {
                    movement = -forwardVector;
                }
            }
            else
            {
                movement = Vector3.zero;
            }

            if (Input.GetKey(rightKey))
            {
                if (invertedMovement)
                {
                    movement -= rightVector;
                }
                else
                {
                    movement += rightVector;
                }
            }

            else if (Input.GetKey(leftKey))
            {
                if (invertedMovement)
                {
                    movement += rightVector;
                }
                else
                {
                    movement -= rightVector;
                }
            }

            movement.Normalize();

            movement *= Time.deltaTime * movementSpeed;

            if ((isGrounded || _jumpAssistant.isPlayerInJumpableZone) && Input.GetKeyDown(jumpKey))
            {
                verticalSpeed = jumpingSpeed;
            }

            verticalSpeed += Physics.gravity.y * Time.deltaTime;
            movement.y = verticalSpeed * Time.deltaTime;

            characterController.Move(movement);

            RaycastHit hit;
            if (Physics.Raycast(transform.position - new Vector3(0f, characterController.height / 2, 0f), -transform.up, out hit, 0.1f))
            {
                if(hit.collider.gameObject.tag != "Deadzone")
                {
                    isGrounded = true;
                    verticalSpeed = 0.0f;
                }
            }
            else
            {
                isGrounded = false;
            }
            #endregion
            #region Attack
            currentTime += Time.deltaTime;

            if (currentTime >= minTimeBetweenShots)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (!onlyOneBullet)
                    {
                        Shoot();
                    }
                    else if (!bulletUsed)
                    {
                        Shoot();
                        bulletUsed = true;
                    }
                    else
                    {
                        //No ammo sound
                        AudioManager.Instance.Play("NoBullet");
                    }
                }
            }
            #endregion

            if (health <= 0)
            {
                Death();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Flag" && catchTheFlag)
        {
            //Player wins
            FindObjectOfType<MatchController>().flag.SetActive(false);
            FindObjectOfType<MatchController>().PlayerWins();
        }
        else if(other.gameObject.tag == "Deadzone")
        {
            health = 0f;
        }
    }

    private void Shoot()
    {
        currentTime = 0f;
        AlertEnemies();

        BulletController currentBullet;
        if (bouncingBullets)
        {
            ricochetShotParts.Play();
            AudioManager.Instance.Play("Shoot");
            currentBullet = Instantiate(bulletBouncing, firePoint.position, pitchRotator.rotation);
        }
        else if (gravityBullets)
        {
            blobShotParts.Play();
            AudioManager.Instance.Play("ShootHeavy");
            currentBullet = Instantiate(bulletGravity, firePoint.position, pitchRotator.rotation);

        }
        else
        {
            basicShotParts.Play();
            AudioManager.Instance.Play("Shoot");
            currentBullet = Instantiate(bullet, firePoint.position, pitchRotator.rotation);
        }
        gunAnim.SetTrigger("Shot");
        cameraShake.ShakeCamera(0.4f, 0.2f);
        currentBullet.GetComponent<Rigidbody>().AddForce(currentBullet.transform.forward * bulletSpeed, ForceMode.Impulse);
        Physics.IgnoreCollision(currentBullet.GetComponent<Collider>(), characterController);

        currentBullet.damage = damage;
        currentBullet.headshotDamage = headshotDamage;

        currentBullet.gravityBullet = gravityBullets;
        currentBullet.gravityMultiplier = roundRules.bulletsGravityMultiplier;
        currentBullet.bouncingBullet = bouncingBullets;

        currentBullet.onlyHeadshots = onlyHeadshots;
    }
    
    private void AlertEnemies()
    {
        EnemyController[] enemies = FindObjectsOfType<EnemyController>();

        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].AlertedByShot();
        }
    }

    public void Hit(float damage)
    {
        AudioManager.Instance.Play("Hit");
        health -= damage;
    }

    private void Death()
    {
        if (winByDying)
        {
            //Player wins
            FindObjectOfType<MatchController>().PlayerWins();
        }
        else
        {
            //Enemy wins
            FindObjectOfType<MatchController>().CPUWins();
        }
    }

    public void SetRoundRules(ShootingRules shootingRule, MovementRules movementRule, WinningRules winningRule)
    {
        switch (shootingRule)
        {
            case ShootingRules.NONE:
                break;
            case ShootingRules.BOUNCING_BULLETS:
                bouncingBullets = true;
                break;
            case ShootingRules.GRAVITY_BULLETS:
                gravityBullets = true;
                break;
            case ShootingRules.INVERTED_MOUSE:
                invertedAiming = true;
                break;
            case ShootingRules.ONLY_ONE_BULLET:
                onlyOneBullet = true;
                bulletUsed = false;
                break;
        }

        switch (movementRule)
        {
            case MovementRules.NONE:
                break;
            case MovementRules.SLOW_MOTION:
                Time.timeScale = roundRules.slowTimeMultiplier;
                break;
            case MovementRules.HYPERFAST:
                Time.timeScale = roundRules.fastTimeMultiplier;
                break;
            case MovementRules.INVERTED_CONTROLS:
                invertedMovement = true;
                break;
        }

        switch (winningRule)
        {
            case WinningRules.NONE:
                break;
            case WinningRules.WIN_BY_DYING:
                winByDying = true;
                break;
            case WinningRules.CATCH_THE_FLAG:
                FindObjectOfType<MatchController>().flag.SetActive(true);
                catchTheFlag = true;
                break;
            case WinningRules.ONLY_HEADSHOTS:
                onlyHeadshots = true;
                break;
        }
    }

    public void StartRound()
    {
        waitToStart = false;
    }
    
    public void FreezePlayer()
    {
        waitToStart = true;
    }

    public void ResetRound()
    { 
        //Shooting
        invertedAiming = false;
        gravityBullets = false;
        bouncingBullets = false;
        onlyOneBullet = false;

        //Movement
        Time.timeScale = 1f;
        invertedMovement = false;

        //Winning Condition
        catchTheFlag = false;
        winByDying = false;
        onlyHeadshots = false;

        FindObjectOfType<MatchController>().flag.SetActive(false);

        BulletController[] bulletsInScene = FindObjectsOfType<BulletController>();

        for (int i = 0; i < bulletsInScene.Length; i++)
        {
            Destroy(bulletsInScene[i]);
        }

        EnemyBullet[] enemyBulletsInScene = FindObjectsOfType<EnemyBullet>();

        for (int i = 0; i < enemyBulletsInScene.Length; i++)
        {
            Destroy(enemyBulletsInScene[i]);
        }

        //Stats
        yawRotation = transform.rotation.eulerAngles.y;
        pitchRotation = pitchRotator.localRotation.eulerAngles.x;

        verticalSpeed = 0f;
        isGrounded = false;

        health = initialHealth;
        currentTime = 0f;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
