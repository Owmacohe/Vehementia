using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float health = 30f;
    public Color[] bloodColours;

    private PlayerController player;
    private Rigidbody2D rb;
    private ParticleSystem blood;
    private ParticleSystem.MainModule bloodMain;
    private bool pushCooldown, hasDied;

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
        if (health <= 0 && !hasDied)
        {
            die();
        }

        if (blood.isEmitting)
        {
            bloodMain.startColor = new ParticleSystem.MinMaxGradient(bloodColours[Random.Range(0, bloodColours.Length)]);
        }
    }

    public void hit(float damage, float knockback)
    {
        if (!hasDied)
        {
            float temp = GameObject.FindGameObjectWithTag("Player").transform.position.x;
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
                rb.AddForce(new Vector2(tempAngle / Mathf.Abs(tempAngle), 1.5f) * knockback * 100);
                pushCooldown = true;
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

            if (hasDied)
            {
                GetComponent<Rigidbody2D>().simulated = false;
                GetComponent<Collider2D>().enabled = false;
            }
        }
    }
}
