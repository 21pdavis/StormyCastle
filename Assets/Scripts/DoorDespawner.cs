using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DoorDespawner : MonoBehaviour
{
    public TileBase doorTile;
    public TileBase emptyTile;
    private Tilemap tilemap;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player") || collider.gameObject.CompareTag("Enemy"))
        {
            List<Vector3Int> surroundingTiles = getSurroundingTiles(collider.transform.position);

            foreach (Vector3Int vec3 in surroundingTiles)
            {
                if (tilemap.GetTile(vec3) == doorTile)
                {
                    tilemap.SetTile(vec3, emptyTile);
                }
            }
        }
    }   

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player") || collider.gameObject.CompareTag("Enemy"))
        {
            List<Vector3Int> surroundingTiles = getSurroundingTiles(collider.transform.position);

            foreach (Vector3Int vec3 in surroundingTiles)
            {
                if (tilemap.GetTile(vec3) == emptyTile)
                {
                    tilemap.SetTile(vec3, doorTile);
                }
            }
        }
    }

    private List<Vector3Int> getSurroundingTiles(Vector3 pos)
    {
        List<Vector3Int> surroundingTiles = new List<Vector3Int>();

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                Vector3Int at = tilemap.WorldToCell(pos + new Vector3(i, j, 0));
                surroundingTiles.Add(at);
            }
        }

        return surroundingTiles;
    }
}
