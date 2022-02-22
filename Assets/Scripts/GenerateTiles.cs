using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GenerateTiles : MonoBehaviour
{
    public int tileLoadOffset = 10;

    [Header("Ground")]
    public Tilemap groundTiles;
    public Tile topLayer, underLayer;

    [Header("Wall")]
    public Tilemap wallTiles;
    public Tile defaultSection, middleSection, endSection;

    [Header("Grass")]
    public Tilemap grassTiles;
    public Tile bigGrass, middleGrass, smallGrass;

    private PlayerController player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    private void FixedUpdate()
    {
        for (int i = -tileLoadOffset; i < tileLoadOffset; i++)
        {
            setColumn(new Vector3Int((int)player.gameObject.transform.position.x + i, 0, 0));
        }
    }

    private void setColumn(Vector3Int p)
    {
        checkAndSetTile(groundTiles, p, topLayer);
        checkAndSetTile(groundTiles, p - Vector3Int.up, underLayer);
        checkAndSetTile(groundTiles, p - (2 * Vector3Int.up), underLayer);

        float temp = Random.Range(0f, 10f);

        if (temp <= 9)
        {
            checkAndSetTile(wallTiles, p + Vector3Int.up, defaultSection);
        }
        else if (temp > 9 && temp <= 9.5f)
        {
            checkAndSetTile(wallTiles, p + Vector3Int.up, middleSection);
        }
        else if (temp > 9.5f)
        {
            checkAndSetTile(wallTiles, p + Vector3Int.up, endSection);
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
