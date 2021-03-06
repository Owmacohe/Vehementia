using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Range(0, 12)]
    public float speed = 8;
    [Range(1, 100)]
    public float health = 30f;
    [Range(0, 50)]
    public int jumpRate = 30;
    [Range(0, 1.5f)]
    public float jumpHeight = 0.5f;
    [Range(10, 50)]
    public float aggroRange = 30;

    public Color deathColour;
    public Color[] bloodColours;

    public AudioClip[] defaultSounds;
    public AudioClip[] specialSounds;

    private PlayerController player;
    private Rigidbody2D rb;
    private ParticleSystem blood;
    private ParticleSystem.MainModule bloodMain;
    private AudioSource audio;

    private bool pushCooldown, hasDied, isJumping;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        blood = GetComponentInChildren<ParticleSystem>();
        bloodMain = blood.main;
        audio = GetComponent<AudioSource>();
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

        if (!player.hasDied && !hasDied)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, transform.eulerAngles.y, 5 * Mathf.Sin(Time.time * Mathf.PI * 10)));

            if (Vector3.Distance(transform.position, player.transform.position) <= aggroRange)
            {
                if (!isJumping && Random.Range(0, jumpRate) == 0)
                {
                    rb.AddForce(Vector2.up * speed * (jumpHeight * 100));
                    isJumping = true;
                }

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
    }

    public void hit(float damage, float knockback)
    {
        if (!hasDied)
        {
            float temp = player.transform.position.x;
            float tempAngle;

            if (temp > transform.position.x)
            {
                tempAngle = 135;
            }
            else
            {
                tempAngle = -45;
            }

            var tempShape = blood.shape;
            tempShape.rotation = Vector3.forward * tempAngle;

            blood.Play();
            Invoke("stopBlood", 0.2f);

            health -= damage;

            if (!pushCooldown)
            {
                rb.AddForce(new Vector2(tempAngle / -Mathf.Abs(tempAngle), 0.5f) * (knockback + Random.Range(0f, knockback)) * 100);
                pushCooldown = true;
            }

            isJumping = true;

            int soundChance = Random.Range(0, 3);

            if (soundChance <= 1)
            {
                int specialChance = Random.Range(0, 6);

                if (specialChance == 0)
                {
                    audio.clip = specialSounds[Random.Range(0, specialSounds.Length)];
                }
                else
                {
                    audio.clip = defaultSounds[Random.Range(0, defaultSounds.Length)];
                }

                audio.Play();
            }
        }
    }

    private void stopBlood()
    {
        if (!hasDied)
        {
            blood.Stop();
        }
    }

    public void die()
    {
        player.killCount++;

        hasDied = true;

        if (!blood.isEmitting)
        {
            blood.Play();
        }

        var tempShape = blood.shape;
        tempShape.rotation = Vector3.forward * 45;

        GetComponent<SpriteRenderer>().color = deathColour;
        GetComponent<Animator>().enabled = false;

        Invoke("cleanup", 3);
    }

    private void cleanup()
    {
        FindObjectOfType<EnemySpawner>().enemyCount--;
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
