using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{    
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

    float yawRotation;
    float pitchRotation;

    float verticalSpeed;
    bool isGrounded;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        yawRotation = transform.rotation.eulerAngles.y;
        pitchRotation = pitchRotator.localRotation.eulerAngles.x;

        verticalSpeed = 0f;
        isGrounded = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
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

        if (Input.GetKey(KeyCode.W))
        {
            movement = forwardVector;
        }     
        else if (Input.GetKey(KeyCode.S))
        {
            movement = -forwardVector;
        }
        else
        {
            movement = Vector3.zero;
        }

        if (Input.GetKey(KeyCode.D))
        {
            movement += rightVector;
        }
            
        else if (Input.GetKey(KeyCode.A))
        {
            movement -= rightVector;
        }
            
        movement.Normalize();

        movement *= Time.deltaTime * movementSpeed;

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            verticalSpeed = jumpingSpeed;
        }  
        #endregion
        #region Collisions_Gravity
        verticalSpeed += Physics.gravity.y * Time.deltaTime;
        movement.y = verticalSpeed * Time.deltaTime;
        
        CollisionFlags l_collisionFlags = characterController.Move(movement);
        if ((l_collisionFlags & CollisionFlags.Below) != 0)
        {
            isGrounded = true;
            verticalSpeed = 0.0f;
        }
        else
        {
            isGrounded = false;
        }
        #endregion
        #region Shooting
        if (Input.GetMouseButtonDown(0))
        {
            GameObject b = Instantiate(bullet, firePoint.position, pitchRotator.rotation);
            b.GetComponent<Rigidbody>().AddForce(b.transform.forward * bulletSpeed, ForceMode.Impulse);
            Physics.IgnoreCollision(b.GetComponent<Collider>(), characterController);
        }
        #endregion
    }
}
