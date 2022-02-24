using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallController : MonoBehaviour
{
    public Color[] particleColours;

    private ParticleSystem part;
    private ParticleSystem.MainModule partMain;

    private PlayerController player;

    private void Start()
    {
        part = GetComponentInChildren<ParticleSystem>();
        partMain = part.main;

        player = FindObjectOfType<PlayerController>();
    }

    private void FixedUpdate()
    {
        float speed = player.moveCount / 300f;

        if (speed <= 0 || Vector3.Distance(transform.position, player.transform.position) <= 25)
        {
            speed = 0.05f;
        }

        transform.position += Vector3.right * speed;

        int temp = Random.Range(0, 11);
        Color tempColour;

        if (temp <= 8)
        {
            tempColour = particleColours[0];
        }
        else
        {
            tempColour = particleColours[Random.Range(1, particleColours.Length)];
        }

        partMain.startColor = new ParticleSystem.MinMaxGradient(tempColour);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            collision.GetComponent<PlayerController>().die();
        }
        else if (collision.gameObject.tag.Equals("Enemy"))
        {
            collision.GetComponent<EnemyController>().die();
        }
    }
}
