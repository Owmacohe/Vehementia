using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Range(0, 12)]
    public float speed = 7;
    [Range(1, 100)]
    public float health = 30f;

    [Range(0, 50)]
    public int jumpRate = 30;
    [Range(0, 1.5f)]
    public float jumpHeight = 0.5f;

    public Color[] bloodColours;

    private PlayerController player;
    private Rigidbody2D rb;
    private ParticleSystem blood;
    private ParticleSystem.MainModule bloodMain;
    private bool pushCooldown, hasDied, isJumping;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        blood = GetComponentInChildren<ParticleSystem>();
        bloodMain = blood.main;
    }

    private void FixedUpdate()
    {
        if (transform.position.y < 2)
        {
            transform.position = new Vector3(transform.position.x, 2, 0);
            rb.velocity = Vector3.zero;
        }

        if (health <= 0 && !hasDied)
        {
            die();
        }

        if (blood.isEmitting)
        {
            bloodMain.startColor = new ParticleSystem.MinMaxGradient(bloodColours[Random.Range(0, bloodColours.Length)]);
        }

        if (!hasDied)
        {
            if (!isJumping && Random.Range(0, jumpRate) == 0)
            {
                rb.AddForce(Vector2.up * speed * (jumpHeight * 100));
                isJumping = true;
            }

            transform.rotation = Quaternion.Euler(new Vector3(0, transform.eulerAngles.y, 5 * Mathf.Sin(Time.time * Mathf.PI * 10)));

            float temp = speed / 100f;
            float speedOffset = Random.Range(0f, temp);

            if (transform.position.x > player.transform.position.x + temp)
            {
                transform.position -= Vector3.right * (temp + speedOffset);
                transform.rotation = Quaternion.Euler(new Vector3(0, 180, transform.eulerAngles.z));
            }
            else if (transform.position.x < player.transform.position.x - temp)
            {
                transform.position += Vector3.right * (temp + speedOffset);
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, transform.eulerAngles.z));
            }
        }
    }

    public void hit(float damage, float knockback)
    {
        if (!hasDied)
        {
            float temp = player.transform.position.x;
            float tempAngle;

            if (temp > transform.position.x)
            {
                tempAngle = -45;
            }
            else
            {
                tempAngle = 135;
            }

            var tempShape = blood.shape;
            tempShape.rotation = Vector3.forward * tempAngle;

            blood.Play();
            Invoke("stopBlood", 0.2f);

            health -= damage;

            if (!pushCooldown)
            {
                rb.AddForce(new Vector2(tempAngle / Mathf.Abs(tempAngle), 1f) * (knockback + Random.Range(0f, knockback)) * 100);
                pushCooldown = true;
            }

            isJumping = true;
        }
    }

    private void stopBlood()
    {
        if (!hasDied)
        {
            blood.Stop();
        }
    }

    private void die()
    {
        player.killCount++;

        hasDied = true;

        if (!blood.isEmitting)
        {
            blood.Play();
        }

        var tempShape = blood.shape;
        tempShape.rotation = Vector3.forward * 45;

        Invoke("cleanup", 3);
    }

    private void cleanup()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Ground"))
        {
            pushCooldown = false;
            isJumping = false;

            if (hasDied)
            {
                GetComponent<Rigidbody2D>().simulated = false;
                GetComponent<Collider2D>().enabled = false;
            }
        }
        
        if (collision.gameObject.tag.Equals("Enemy"))
        {
            Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        }
    }
}
