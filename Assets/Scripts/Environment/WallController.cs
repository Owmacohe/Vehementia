using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallController : MonoBehaviour
{
    public float speed = 2;
    public Color[] particleColours;

    private ParticleSystem part;
    private ParticleSystem.MainModule partMain;

    private void Start()
    {
        part = GetComponentInChildren<ParticleSystem>();
        partMain = part.main;
    }

    private void FixedUpdate()
    {
        transform.position += Vector3.right * (speed / 100f);

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
    }
}
