using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleHalo : MonoBehaviour
{
    [Header("Idle")]
    public Color[] idleColours;
    public float idleSpeed = 0.75f;
    public float idleAmount = 20;

    [Header("Walking")]
    public Color[] walkingColours;
    public float walkingSpeed = 3;
    public float walkingAmount = 30;

    private ParticleSystem halo;
    private ParticleSystem.MainModule particleMain;
    private bool isIdle;

    private void Start()
    {
        halo = GetComponentInChildren<ParticleSystem>();
        particleMain = halo.main;
    }

    private void FixedUpdate()
    {
        Color[] temp;

        if (isIdle)
        {
            temp = idleColours;
        }
        else
        {
            temp = walkingColours;
        }

        particleMain.startColor = new ParticleSystem.MinMaxGradient(temp[Random.Range(0, temp.Length)]);
    }

    public void setIdle()
    {
        isIdle = true;

        particleMain.startSpeed = idleSpeed;

        var tempEmission = halo.emission;
        tempEmission.rateOverTime = idleAmount;
    }

    public void setWalking(float moveCount)
    {
        isIdle = false;

        if (moveCount > 4 * walkingAmount)
        {
            moveCount = 4 * walkingAmount;
        }

        particleMain.startSpeed = walkingSpeed + (moveCount / 100);
     
        var tempEmission = halo.emission;
        tempEmission.rateOverTime = walkingAmount + moveCount;
    }
}
