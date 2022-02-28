using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    [Range(10, 40)]
    public float transitionPoint = 20;

    private AudioSource bass, melody;
    private float minimumBass;
    private PlayerController player;

    private void Start()
    {
        AudioSource[] temp = GetComponents<AudioSource>();

        bass = temp[0];
        minimumBass = bass.volume;
        bass.volume = 0;
        bass.Play();

        melody = temp[1];

        player = FindObjectOfType<PlayerController>();
    }

    private void Update()
    {
        float temp = player.moveCount;
        float pos = player.transform.position.x;

        if (temp < transitionPoint)
        {
            if (bass.isPlaying)
            {
                if (temp <= 0 && pos < 0)
                {
                    bass.volume = 0;
                }
                else
                {
                    bass.volume = (temp / 30f) + minimumBass;
                }
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
                if (temp <= 0 && pos < 0)
                {
                    melody.volume = 0;
                }
                else
                {
                    melody.volume = (temp / 100f);
                }
            }
        }
    }
}
