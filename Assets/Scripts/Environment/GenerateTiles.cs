using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GenerateTiles : MonoBehaviour
{
    [Range(5, 30)]
    public int tileLoadOffset = 20;

    [Header("Ground")]
    public Tilemap groundTiles;
    public Tile topLayer, underLayer;

    [Header("Wall")]
    public Tilemap wallTiles;
    public Tile defaultSection, middleSection, endSection;

    [Header("Grass")]
    public Tilemap grassTiles;
    public Tile[] grass;

    private PlayerController player;
    private List<int> generated;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        generated = new List<int>();
    }

    private void FixedUpdate()
    {
        if (player.hasDied)
        {
            grassTiles.GetComponent<TilemapRenderer>().sortingOrder = 4;
        }

        for (int i = -tileLoadOffset; i < tileLoadOffset; i++)
        {
            int temp = (int)player.gameObject.transform.position.x + i;

            if (!generated.Contains(temp))
            {
                setColumn(new Vector3Int(temp, 0, 0));
            }
        }
    }

    private void setColumn(Vector3Int p)
    {
        if (p.x >= 0)
        {
            int temp1 = Random.Range(0, 5);
            int heightOffset = Random.Range(2, 7);

            if (temp1 == 0)
            {
                checkAndSetTile(wallTiles, p + (heightOffset * Vector3Int.up), defaultSection);
            }

            int temp2 = Random.Range(0, 5);

            if (temp2 == 0)
            {
                checkAndSetTile(grassTiles, p + Vector3Int.up, grass[Random.Range(0, grass.Length)]);
            }

            float temp3 = Random.Range(0f, 10f);

            if (temp3 <= 9)
            {
                checkAndSetTile(wallTiles, p + Vector3Int.up, defaultSection);
            }
            else if (temp3 > 9 && temp3 <= 9.5f)
            {
                checkAndSetTile(wallTiles, p + Vector3Int.up, middleSection);
            }
            else if (temp3 > 9.5f)
            {
                checkAndSetTile(wallTiles, p + Vector3Int.up, endSection);
            }

            checkAndSetTile(groundTiles, p, topLayer);
            checkAndSetTile(groundTiles, p - Vector3Int.up, underLayer);
            checkAndSetTile(groundTiles, p - (2 * Vector3Int.up), underLayer);

            if (!generated.Contains(p.x))
            {
                generated.Add(p.x);
            }
        }
    }

    private void checkAndSetTile(Tilemap m, Vector3Int p, Tile t)
    {
        if (!m.GetTile(p))
        {
            m.SetTile(p, t);
        }
    }
}
