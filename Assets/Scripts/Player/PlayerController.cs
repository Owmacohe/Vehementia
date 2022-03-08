using Febucci.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float health = 100;
    private float lastHealth;
    [Range(0.1f, 3)]
    public float speed = 1.5f;
    [Range(1, 8)]
    public int jumpHeight = 6;

    public TextAnimator killCountMain, healthMain;
    private TextDuplicator killCountDup, healthDup;

    public SpriteRenderer fade;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer rend, weaponRend;
    private ParticleHalo halo;
    private WeaponController weapon;
    private Camera playerCamera;
    private SceneLoader loader;

    [HideInInspector]
    public float direction, moveCount;
    [HideInInspector]
    public bool isOnGround, isJumping, hasDied, hasForgotten;

    [HideInInspector]
    public int killCount;
    private int lastKillCount;

    private float deathTime;
    private bool pushCooldown, hasLeftSanctuary;

    private void Start()
    {
        if (FindObjectOfType<SceneLoader>() == null)
        {
            Instantiate(Resources.Load("Scene Manager"), Vector2.zero, Quaternion.identity);
        }

        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        anim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();
        halo = GetComponent<ParticleHalo>();
        playerCamera = Camera.main;

        weapon = GetComponentInChildren<WeaponController>();
        weaponRend = weapon.GetComponentInChildren<SpriteRenderer>();

        killCountDup = killCountMain.gameObject.GetComponent<TextDuplicator>();
        healthDup = healthMain.gameObject.GetComponent<TextDuplicator>();

        loader = FindObjectOfType<SceneLoader>();

        if (loader.hasAlreadyPlayed)
        {
            string temp = "Kill Count: " + "<shake a=" + (killCount / 50f) + ">" + killCount + "</shake>";

            if (loader.hasAlreadyPlayed)
            {
                temp += "\nHigh Score: " + loader.highScore;
            }

            killCountMain.SetText(temp, false);
            killCountDup.generate();
        }
    }

    private void OnMove(InputValue input)
    {
        direction = input.Get<Vector2>().x;
    }

    private void OnJump(InputValue input)
    {
        if (!hasDied && isOnGround)
        {
            if ((loader.hasAlreadyPlayed && hasForgotten) || !loader.hasAlreadyPlayed)
            {
                rb.velocity += Vector2.up * jumpHeight;
            }
        }
    }

    private void OnInteract()
    {
        if ((loader.hasAlreadyPlayed && hasForgotten) || !loader.hasAlreadyPlayed)
        {
            weapon.swing();
        }
        else
        {
            hasForgotten = true;
            healthMain.SetText("Health: " + "<shake a=" + (1f / (health / 5f)) + ">" + health + "</shake>", false);
            healthDup.generate();
            healthDup.transform.parent.gameObject.SetActive(false);
            halo.setWalking(100);
        }
    }

    private void FixedUpdate()
    {
        if (transform.position.x < -12)
        {
            if (playerCamera.enabled)
            {
                playerCamera.enabled = false;
                killCountDup.transform.parent.gameObject.SetActive(false);

                if (!loader.hasAlreadyPlayed)
                {
                    healthDup.transform.parent.gameObject.SetActive(false);
                }
                else
                {
                    if (!hasForgotten)
                    {
                        healthMain.SetText("Health: " + "<shake a=0.3>DEAD</shake>", false);
                        healthDup.generate();
                    }
                }
            }
        }
        else
        {
            if (!playerCamera.enabled)
            {
                playerCamera.enabled = true;
                killCountDup.transform.parent.gameObject.SetActive(true);
                healthDup.transform.parent.gameObject.SetActive(true);
            }

            if (!hasLeftSanctuary && transform.position.x >= 20)
            {
                hasLeftSanctuary = true;

                Instantiate(Resources.Load("Wall Of Death"), Vector3.zero, Quaternion.identity);
                GetComponent<EnemySpawner>().enabled = true;
            }

            if (lastKillCount != killCount)
            {
                if (killCount > loader.highScore)
                {
                    loader.highScore = killCount;
                }

                string temp = "Kill Count: " + "<shake a=" + (killCount / 50f) + ">" + killCount + "</shake>";

                if (loader.hasAlreadyPlayed)
                {
                    temp += "\nHigh Score: " + loader.highScore;
                }

                killCountMain.SetText(temp, false);
                killCountDup.generate();
                lastKillCount = killCount;
            }

            if (lastHealth != health)
            {
                healthMain.SetText("Health: " + "<shake a=" + (1f / (health / 5f)) + ">" + health + "</shake>", false);
                healthDup.generate();
                lastHealth = health;
            }
        }

        if (hasDied)
        {
            fade.color = new Color(0, 0, 0, Time.time - deathTime);

            if (Time.time - deathTime >= 3)
            {
                SceneLoader loader = FindObjectOfType<SceneLoader>();
                loader.hasAlreadyPlayed = true;
                loader.load("Main Scene");
            }

            if (moveCount > 0)
            {
                moveCount--;
            }
        }
        else
        {
            if (health <= 0)
            {
                die();
            }

            if (direction == 0 && !isJumping && !weapon.isRotating && !weapon.isMoving)
            {
                if (moveCount > 0)
                {
                    moveCount--;
                }

                anim.SetBool("isWalking", false);
                Color temp = new Color(1, 1, 1, 0.2f * Mathf.Sin(0.5f * Time.time * Mathf.PI) + 0.5f);
                rend.color = temp;

                if ((loader.hasAlreadyPlayed && transform.position.x >= -12) || (loader.hasAlreadyPlayed && !hasForgotten) || !loader.hasAlreadyPlayed)
                {
                    halo.setIdle();
                }

                weapon.gameObject.transform.localRotation = Quaternion.Euler(Vector3.forward * 30);
                weaponRend.color = temp;
            }
            else
            {
                if ((loader.hasAlreadyPlayed && hasForgotten) || !loader.hasAlreadyPlayed)
                {
                    rb.velocity = Vector2.up * rb.velocity.y;

                    if (transform.position.x >= 0)
                    {
                        moveCount += 0.2f;
                    }

                    anim.SetBool("isWalking", true);
                    rend.color = Color.white;

                    if (transform.position.x >= -12)
                    {
                        halo.setWalking(moveCount);
                    }

                    weaponRend.color = Color.white;

                    transform.position += Vector3.right * direction * speed * 0.1f;

                    if (direction != 0)
                    {
                        if (direction > 0)
                        {
                            transform.rotation = Quaternion.identity;
                            playerCamera.transform.localRotation = Quaternion.identity;
                        }
                        else
                        {
                            transform.rotation = Quaternion.Euler(Vector3.up * 180);
                            playerCamera.transform.localRotation = Quaternion.Euler(Vector3.up * 180);
                        }
                    }
                }
            }
        }
    }

    public void hit(float damage, float knockback, int direction)
    {
        health -= damage;
        rb.AddForce(2 * new Vector2(direction * knockback * 100, 1));
    }

    public void die()
    {
        if (!hasDied)
        {
            hasDied = true;

            anim.SetBool("isJumping", true);
            rend.color = Color.white;
            halo.setWalking(moveCount);

            deathTime = Time.time;

            healthMain.SetText("Health: " + "<shake a=0.3>DEAD</shake>", false);
            healthDup.generate();
        }
    }

    private void stopPushCooldown()
    {
        pushCooldown = false;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!hasDied)
        {
            if (collision.gameObject.tag.Equals("Ground"))
            {
                isOnGround = true;
                isJumping = false;
                anim.SetBool("isJumping", isJumping);
            }

            if (collision.gameObject.tag.Equals("Enemy"))
            {
                if (!pushCooldown)
                {
                    int temp = 1;

                    if (collision.transform.position.x > transform.position.x)
                    {
                        temp = -1;
                    }

                    hit(10, 10, temp);
                    pushCooldown = true;
                    Invoke("stopPushCooldown", 2);
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!hasDied && collision.gameObject.tag.Equals("Ground"))
        {
            isOnGround = false;
            isJumping = true;
            anim.SetBool("isJumping", isJumping);
        }
    }
}
