using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public VariableJoystick variableJoystick;
    public List<Animator> animators;

    private Vector2 movement;
    private Rigidbody2D rb;
    private string currentAnim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;  // Set interpolation for smoother movement
    }

    void Update()
    {
        if (!DataHandler.instance.isPlaying) return;
        float moveInputX = Input.GetAxisRaw("Horizontal") + variableJoystick.Horizontal;
        float moveInputY = Input.GetAxisRaw("Vertical") + variableJoystick.Vertical;

        movement = new Vector2(moveInputX, moveInputY).normalized;

        if (movement.magnitude > 0)
        {
            if (movement.x > 0)
            {
                if (movement.y > 0) PlayAnim("BR");
                else if (movement.y < 0) PlayAnim("FR");
                else PlayAnim("FR");
            }
            else if (movement.x < 0)
            {
                if (movement.y > 0) PlayAnim("BL");
                else if (movement.y < 0) PlayAnim("FL");
                else PlayAnim("FL");
            }
            else
            {
                if (movement.y > 0) PlayAnim("BL");
                else if (movement.y < 0) PlayAnim("FL");
            }
        }
        else
        {
            PlayAnim("Idle");
        }
    }

    void FixedUpdate()
    {
        rb.velocity = movement * moveSpeed;  // Use velocity for smooth movement
    }

    void PlayAnim(string anim)
    {
        if (currentAnim == anim) return;

        currentAnim = anim;
        foreach (var item in animators)
        {
            item.Play(anim);
        }
    }
}
