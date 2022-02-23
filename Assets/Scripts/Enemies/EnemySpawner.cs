using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Vector2 spawnRange = new Vector2(10f, 20f);
    public int spawnCap = 30;

    [HideInInspector]
    public int enemyCount;
    private GameObject[] enemies;

    private void Start()
    {
        enemies = Resources.LoadAll<GameObject>("Enemies");
    }

    private void FixedUpdate()
    {
        if (enemyCount < spawnCap)
        {
            int temp = Random.Range(0, 50);

            if (temp == 0)
            {
                float direction = Random.Range(spawnRange.x, spawnRange.y);

                if (direction > 1 || direction < -1)
                {
                    spawnEnemy(direction);
                }
            }
        }
    }

    private void spawnEnemy(float direction)
    {
        GameObject newEnemy = Instantiate(enemies[Random.Range(0, enemies.Length)], transform.position + (Vector3.right * direction), Quaternion.identity);
        newEnemy.transform.parent = null;

        enemyCount++;
    }
}
