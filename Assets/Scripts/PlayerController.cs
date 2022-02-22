using Febucci.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Range(0.1f, 3)]
    public float speed = 1.5f;
    [Range(1, 8)]
    public int jumpHeight = 6;

    public TMP_Text violentMain, violentSecondary, peacefulMain;
    private GameObject violent, peaceful;
    public TextAnimator killCountMain;
    private TextDuplicator killCountDup;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer rend, weaponRend;
    private ParticleHalo halo;
    private WeaponController weapon;

    [HideInInspector]
    public float direction, moveCount;
    [HideInInspector]
    public bool isOnGround, isJumping;

    [HideInInspector]
    public int killCount;
    private int lastKillCount;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        anim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();
        halo = GetComponent<ParticleHalo>();

        weapon = GetComponentInChildren<WeaponController>();
        weaponRend = weapon.GetComponentInChildren<SpriteRenderer>();

        violent = violentMain.transform.parent.gameObject;
        peaceful = peacefulMain.transform.parent.gameObject;
        killCountDup = killCountMain.gameObject.GetComponent<TextDuplicator>();
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

    private void OnInteract()
    {
        weapon.swing();
    }

    private void FixedUpdate()
    {
        if (lastKillCount != killCount)
        {
            killCountMain.SetText( "Kill Count: " + "<shake a=" + (killCount / 50f) + ">" + killCount + "</shake>", false);
            killCountDup.generate();
            lastKillCount = killCount;
        }

        if (direction == 0 && !isJumping && !weapon.isRotating && !weapon.isMoving)
        {
            if (moveCount > 0)
            {
                moveCount -= 2;
            }

            anim.SetBool("isWalking", false);
            Color temp = new Color(1, 1, 1, 0.2f * Mathf.Sin(0.5f * Time.time * Mathf.PI) + 0.5f);
            rend.color = temp;
            halo.setIdle();

            weapon.gameObject.transform.localRotation = Quaternion.Euler(Vector3.forward * 30);
            weaponRend.color = temp;
        }
        else
        {
            moveCount += 0.2f;

            anim.SetBool("isWalking", true);
            rend.color = Color.white;
            halo.setWalking(moveCount);

            weaponRend.color = Color.white;

            transform.position += Vector3.right * direction * (speed + (moveCount / 100f)) * 0.1f;

            if (direction != 0)
            {
                if (direction > 0)
                {
                    transform.rotation = Quaternion.identity;
                    Camera.main.transform.localRotation = Quaternion.identity;
                }
                else
                {
                    transform.rotation = Quaternion.Euler(Vector3.up * 180);
                    Camera.main.transform.localRotation = Quaternion.Euler(Vector3.up * 180);
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Enemy"))
        {
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Ground"))
        {
            isOnGround = true;
            isJumping = false;
            anim.SetBool("isJumping", isJumping);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Ground"))
        {
            isOnGround = false;
            isJumping = true;
            anim.SetBool("isJumping", isJumping);
        }
    }
}
