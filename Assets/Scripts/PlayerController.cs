using System.Collections;
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
    public KeyCode meleeAttackKey;
    
    [Header("Stats")]
    public float initialHealth;
    
    [Header("Movement")]
    public float movementSpeed;
    public float jumpingSpeed;

    [Header("Rotaion")]
    public Transform pitchRotator;
    public float maxPitchRotation;
    public float minPitchRotation;
    public float pitchRotationSpeed;
    public float yawRotationSpeed;

    [Header("Shooting")]
    public GameObject bullet;
    public Transform firePoint;
    public float bulletSpeed;

    private CharacterController characterController;

    private float yawRotation;
    private float pitchRotation;

    private float verticalSpeed;
    private bool isGrounded;

    public enum ShootingRules
    {
        NONE,
        BOUNCING_BULLETS,
        BULLETS_GRAVITY,
        INVERTED_MOUSE,
        ONLY_ONE_BULLET
    }

    public enum MovementRules
    {
        NONE,
        SLOW_MOTION,
        HYPERFAST,
        INVERTED_CONTROLS,
        CANNOT_MOVE
    }

    public enum WinningRules
    {
        NONE,
        WIN_BY_DYING,
        CATCH_THE_FLAG,
        ONLY_HEADSHOTS,
        ONLY_MELEE
    }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        yawRotation = transform.rotation.eulerAngles.y;
        pitchRotation = pitchRotator.localRotation.eulerAngles.x;

        verticalSpeed = 0f;
        isGrounded = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        SetRoundRules(ShootingRules.NONE, MovementRules.HYPERFAST, WinningRules.NONE);
    }

    void Update()
    {
        #region Rotation
        float l_mouseAxisY = Input.GetAxis("Mouse Y");
        pitchRotation += l_mouseAxisY * pitchRotationSpeed;
        pitchRotation = Mathf.Clamp(pitchRotation, minPitchRotation, maxPitchRotation);

        float l_MouseAxisX = Input.GetAxis("Mouse X");
        yawRotation += l_MouseAxisX * yawRotationSpeed;

        transform.rotation = Quaternion.Euler(0.0f, yawRotation, 0.0f);
        pitchRotator.localRotation = Quaternion.Euler(-pitchRotation, 0.0f, 0.0f);
        #endregion
        #region Movement
        Vector3 forwardVector = new Vector3(Mathf.Sin(yawRotation * Mathf.Deg2Rad), 0.0f, Mathf.Cos(yawRotation * Mathf.Deg2Rad));
        Vector3 rightVector = new Vector3(Mathf.Sin((yawRotation + 90.0f) * Mathf.Deg2Rad), 0.0f, Mathf.Cos((yawRotation + 90.0f) * Mathf.Deg2Rad));

        Vector3 movement;

        if (Input.GetKey(forwardKey))
        {
            movement = forwardVector;
        }     
        else if (Input.GetKey(backwardsKey))
        {
            movement = -forwardVector;
        }
        else
        {
            movement = Vector3.zero;
        }

        if (Input.GetKey(rightKey))
        {
            movement += rightVector;
        }
            
        else if (Input.GetKey(leftKey))
        {
            movement -= rightVector;
        }
            
        movement.Normalize();

        movement *= Time.deltaTime * movementSpeed;

        if (isGrounded && Input.GetKeyDown(jumpKey))
        {
            verticalSpeed = jumpingSpeed;
        }  
        #endregion
        #region Collisions_Gravity
        verticalSpeed += Physics.gravity.y * Time.deltaTime;
        movement.y = verticalSpeed * Time.deltaTime;

        characterController.Move(movement);

        if (Physics.Raycast(transform.position - new Vector3(0f, characterController.height / 2, 0f), -transform.up, 0.1f))
        {
            isGrounded = true;
            verticalSpeed = 0.0f;
        }
        else
        {
            isGrounded = false;
        }
        #endregion
        #region Attack
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }

        if (Input.GetKeyDown(meleeAttackKey))
        {
            MeleeAttack();
        }
        #endregion
    }

    private void Shoot()
    {
        GameObject b = Instantiate(bullet, firePoint.position, pitchRotator.rotation);
        b.GetComponent<Rigidbody>().AddForce(b.transform.forward * bulletSpeed, ForceMode.Impulse);
        Physics.IgnoreCollision(b.GetComponent<Collider>(), characterController);
    }
    
    private void MeleeAttack()
    {

    }

    public void SetRoundRules(ShootingRules shootingRule, MovementRules movementRule, WinningRules winningRule)
    {
        switch (shootingRule)
        {
            case ShootingRules.NONE:
                break;
            case ShootingRules.BOUNCING_BULLETS:
                break;
            case ShootingRules.BULLETS_GRAVITY:
                break;
            case ShootingRules.INVERTED_MOUSE:
                break;
            case ShootingRules.ONLY_ONE_BULLET:
                break;
        }

        switch (movementRule)
        {
            case MovementRules.NONE:
                break;
            case MovementRules.SLOW_MOTION:
                Time.timeScale = 0.5f;
                break;
            case MovementRules.HYPERFAST:
                Time.timeScale = 2f;
                break;
            case MovementRules.INVERTED_CONTROLS:
                break;
            case MovementRules.CANNOT_MOVE:
                break;
        }

        switch (winningRule)
        {
            case WinningRules.NONE:
                break;
            case WinningRules.WIN_BY_DYING:
                break;
            case WinningRules.CATCH_THE_FLAG:
                break;
            case WinningRules.ONLY_HEADSHOTS:
                break;
            case WinningRules.ONLY_MELEE:
                break;
        }
    }

    public void ResetRules()
    {
        Time.timeScale = 1f;
    }
}
