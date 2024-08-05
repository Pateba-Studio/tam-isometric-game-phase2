using UnityEngine;

public class Simple2DMovement : MonoBehaviour
{
    public static Simple2DMovement instance;

    [Header("Refrence")]
    public GameObject character;
    public Joystick joystick;
    public Animator animator;

    [Header("Parameter")]
    public bool isWork;
    public float movementSpeed = 5f;

    Vector2 playerMovement;
    Rigidbody2D rb;

    private void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (isWork)
        {
            if (DataHolder.instance.isFirstOpen)
                DataHolder.instance.isFirstOpen = false;

            playerMovement.x = Input.GetAxisRaw("Horizontal") + joystick.Horizontal;
            playerMovement.y = Input.GetAxisRaw("Vertical") + joystick.Vertical;
            rb.velocity = playerMovement * movementSpeed;
            
            if (playerMovement.y > 0)
            {
                SetBoolAnimatorTrue("isForward");
                animator.SetBool("isBackward", false);
            }
            else if (playerMovement.y < 0)
            {
                animator.SetBool("isForward", false);
                SetBoolAnimatorTrue("isBackward");
            }

            if (playerMovement.x != 0)
            {
                if (playerMovement.y == 0)
                {
                    animator.SetBool("isForward", false);
                    SetBoolAnimatorTrue("isBackward");
                }

                if (playerMovement.x > 0)
                {
                    character.GetComponent<SpriteRenderer>().flipX = false;
                    
                }
                else if (playerMovement.x < 0)
                {
                    character.GetComponent<SpriteRenderer>().flipX = true;
                }
            }

            if (playerMovement.x == 0 && playerMovement.y == 0)
            {
                animator.SetBool("isForward", false);
                animator.SetBool("isBackward", false);
            }
        }
    }

    public void SetBoolAnimatorTrue(string param)
    {
        if (!animator.GetBool(param))
        {
            //Debug.Log(param + " is going on");
            animator.SetBool(param, true);
        }
    }

    public void SetIsWork(bool cond)
    {
        isWork = cond;
    }
}
