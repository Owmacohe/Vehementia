using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Range(1, 30)]
    public int swingSpeed = 20;
    [Range(1, 30)]
    public int swingAmount = 15;

    public float damage = 15;
    public float knockback = 25;

    [HideInInspector]
    public bool isRotating, isMoving;
    private float startTime;

    private List<EnemyController> collidingEnemies;

    private void Start()
    {
        collidingEnemies = new List<EnemyController>();
    }

    public void swing()
    {
        transform.localRotation = Quaternion.identity;
        transform.localPosition = Vector3.zero;

        isRotating = true;
        isMoving = false;

        startTime = Time.time;

        float temp = 0.5f * Mathf.Pow(swingSpeed, 2);
        Invoke("stopRotating", swingSpeed / temp);

        if (collidingEnemies.Count > 0)
        {
            foreach (EnemyController i in collidingEnemies)
            {
                i.hit(damage, knockback);
            }
        }
    }

    private void FixedUpdate()
    {
        if (isRotating)
        {
            transform.localRotation = Quaternion.Euler(transform.localEulerAngles + (Vector3.forward * swingAmount * Mathf.Sin((Time.time - startTime) * swingSpeed * Mathf.PI)));
        }
        else if (isMoving)
        {
            transform.localPosition += Vector3.right * 0.01f * swingAmount * -Mathf.Sin(-(Time.time - startTime) * swingSpeed * Mathf.PI);
        }
    }

    private void stopRotating()
    {
        if (isRotating)
        {
            isRotating = false;
            isMoving = true;

            startTime = Time.time;

            float temp = 0.5f * Mathf.Pow(swingSpeed, 2);
            Invoke("stopMoving", swingSpeed / temp);
        }
    }

    private void stopMoving()
    {
        if (isMoving)
        {
            isMoving = false;
            transform.localRotation = Quaternion.identity;
            transform.localPosition = Vector3.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Enemy"))
        {
            collidingEnemies.Add(collision.GetComponent<EnemyController>());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Enemy"))
        {
            collidingEnemies.Remove(collision.GetComponent<EnemyController>());
        }
    }
}
