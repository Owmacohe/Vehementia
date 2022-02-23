using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public bool playMusic = true;

    private AudioSource bass, melody;
    private float minimumBass;
    private PlayerController player;

    private void Start()
    {
        if (playMusic)
        {
            AudioSource[] temp = GetComponents<AudioSource>();

            bass = temp[0];
            minimumBass = bass.volume;
            bass.Play();

            melody = temp[1];

            player = FindObjectOfType<PlayerController>();
        }
    }

    private void FixedUpdate()
    {
        if (playMusic)
        {
            float temp = player.moveCount;

            if (temp < 15)
            {
                if (bass.isPlaying)
                {
                    bass.volume = (temp / 30f) + minimumBass;
                }

                if (melody.isPlaying && melody.volume == 0)
                {
                    melody.Stop();
                    bass.Play();
                }
                else
                {
                    melody.volume -= 0.005f;
                }
            }
            else
            {
                if (bass.isPlaying && bass.volume == 0)
                {
                    bass.Stop();
                }
                else
                {
                    bass.volume -= 0.01f;
                }

                if (!melody.isPlaying)
                {
                    melody.Play();
                }
                else
                {
                    melody.volume = (temp / 100f);
                }
            }
        }
    }
}
