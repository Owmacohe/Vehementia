using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Range(0.1f, 3)]
    public float speed = 1.5f;
    [Range(1, 8)]
    public int jumpHeight = 6;

    private Rigidbody2D rb;
    private Animator anim;

    private float direction;
    private bool isOnGround;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        anim = GetComponent<Animator>();
    }

    private void OnMove(InputValue input)
    {
        direction = input.Get<Vector2>().x;
    }

    private void OnJump(InputValue input)
    {
        if (isOnGround)
        {
            rb.velocity += Vector2.up * jumpHeight;
        }
    }

    private void FixedUpdate()
    {
        print(direction);

        if (direction != 0)
        {
            transform.position += Vector3.right * direction * speed * 0.1f;

            if (direction > 0)
            {
                transform.rotation = Quaternion.identity;
            }
            else
            {
                transform.rotation = Quaternion.Euler(Vector3.up * 180);
            }

            anim.SetBool("isWalking", true);
        }
        else
        {
            anim.SetBool("isWalking", false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Ground"))
        {
            isOnGround = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Ground"))
        {
            isOnGround = false;
        }
    }
}
