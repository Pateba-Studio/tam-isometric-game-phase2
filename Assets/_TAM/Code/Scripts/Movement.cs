using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public bool IsWork;

    [Header("Refrence")]
    public GameObject character;
    public Joystick joystick;
    public Animator anim;

    [Header("Movement")]
    public KeyCode sprintKey = KeyCode.LeftShift;
    public float currentSpeed;
    public float moveSpeed;
    public float runningSpeed;
    public float walkRotationSpeed;
    public float runRotationSpeed;
    public bool isWalking;
    public bool isRun;

    [Header("Target Transform")]
    public Vector2 playerMovement;
    public Vector3 whenPlayerGoUp;
    public Vector3 whenPlayerGoRight;
    public Vector3 whenPlayerGoLeft;
    public Vector3 whenPlayerGoDown;

    Quaternion lastRotation = Quaternion.identity;
    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        character.transform.localRotation = Quaternion.Euler(whenPlayerGoDown.x, whenPlayerGoDown.y, whenPlayerGoDown.z);
        lastRotation = character.transform.localRotation;
        currentSpeed = moveSpeed;
    }

    void Update()
    {
        UpdateRotation();
        if (IsWork)
        {
            Vector2 currentInputDirection = new Vector2(playerMovement.x, playerMovement.y);

            if (playerMovement.x > 0)
            {
                character.transform.localRotation = Quaternion.Lerp(character.transform.localRotation, Quaternion.Euler(whenPlayerGoRight.x, whenPlayerGoRight.y, whenPlayerGoRight.z), isRun ? runRotationSpeed * Time.deltaTime : walkRotationSpeed * Time.deltaTime);
            }
            else if (playerMovement.x < 0)
            {
                character.transform.localRotation = Quaternion.Lerp(character.transform.localRotation, Quaternion.Euler(whenPlayerGoLeft.x, whenPlayerGoLeft.y, whenPlayerGoLeft.z), isRun ? runRotationSpeed * Time.deltaTime : walkRotationSpeed * Time.deltaTime);
            }

            if (playerMovement.y > 0)
            {
                character.transform.localRotation = Quaternion.Lerp(character.transform.localRotation, Quaternion.Euler(whenPlayerGoUp.x, whenPlayerGoUp.y, whenPlayerGoUp.z), isRun ? runRotationSpeed * Time.deltaTime : walkRotationSpeed * Time.deltaTime);
            }
            else if (playerMovement.y < 0)
            {
                character.transform.localRotation = Quaternion.Lerp(character.transform.localRotation, Quaternion.Euler(whenPlayerGoDown.x, whenPlayerGoDown.y, whenPlayerGoDown.z), isRun ? runRotationSpeed * Time.deltaTime : walkRotationSpeed * Time.deltaTime);
            }

            //movement
            playerMovement.x = Input.GetAxisRaw("Horizontal") + joystick.Horizontal;
            playerMovement.y = Input.GetAxisRaw("Vertical") + joystick.Vertical;
            rb.velocity = playerMovement * currentSpeed;

            if (rb.velocity.magnitude == 0)
            {
                isWalking = false;
                anim.SetBool("isWalking", isWalking);
                character.transform.localRotation = Quaternion.Lerp(character.transform.localRotation, lastRotation, isRun ? runRotationSpeed * Time.deltaTime : walkRotationSpeed * Time.deltaTime);
            }

            else if (playerMovement.magnitude > 0)
            {
                isWalking = true;
                anim.SetBool("isWalking", isWalking);
                lastRotation = character.transform.localRotation; // store the last rotation
            }

            if (isRun || Input.GetKey(sprintKey) && playerMovement.magnitude > 0)
            {
                currentSpeed = runningSpeed;
                anim.SetBool("isRun", true);

                if (playerMovement.magnitude == 0)
                {
                    currentSpeed = moveSpeed;
                    anim.SetBool("isRun", false);
                }
            }
            else
            {
                currentSpeed = moveSpeed;
                anim.SetBool("isRun", false);
            }
        }


    }

    public void PlayerRunning(bool running)
    {
        if (playerMovement.magnitude > 0)
        {
            isRun = running;
            currentSpeed = running ? runningSpeed : moveSpeed;
            anim.SetBool("isRun", running);
        }
        else
        {
            isRun = false;
            currentSpeed = moveSpeed;
            anim.SetBool("isRun", false);
        }
    }

    void UpdateRotation()
    {
        if (playerMovement.x > 0)
        {
            character.transform.localRotation = Quaternion.Lerp(character.transform.localRotation, Quaternion.Euler(whenPlayerGoRight.x, whenPlayerGoRight.y, whenPlayerGoRight.z), isRun ? runRotationSpeed * Time.deltaTime : walkRotationSpeed * Time.deltaTime);
        }
        else if (playerMovement.x < 0)
        {
            character.transform.localRotation = Quaternion.Lerp(character.transform.localRotation, Quaternion.Euler(whenPlayerGoLeft.x, whenPlayerGoLeft.y, whenPlayerGoLeft.z), isRun ? runRotationSpeed * Time.deltaTime : walkRotationSpeed * Time.deltaTime);
        }

        if (playerMovement.y > 0)
        {
            character.transform.localRotation = Quaternion.Lerp(character.transform.localRotation, Quaternion.Euler(whenPlayerGoUp.x, whenPlayerGoUp.y, whenPlayerGoUp.z), isRun ? runRotationSpeed * Time.deltaTime : walkRotationSpeed * Time.deltaTime);
        }
        else if (playerMovement.y < 0)
        {
            character.transform.localRotation = Quaternion.Lerp(character.transform.localRotation, Quaternion.Euler(whenPlayerGoDown.x, whenPlayerGoDown.y, whenPlayerGoDown.z), isRun ? runRotationSpeed * Time.deltaTime : walkRotationSpeed * Time.deltaTime);
        }
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.gameObject != null)
    //    {
    //        Debug.Log("Collided with object named: " + collision.gameObject.name);
    //    }
    //}
}
