using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapSpawner : MonoBehaviour
{
    [Header("Tilemap Configuration")]
    [SerializeField] private TileBase tileType;
    
    private Tilemap tilemap;

    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        if (tileType == null || tilemap == null)
        {
            Debug.LogError("Tilemap references are not set!");
            enabled = false;
            return;
        }

        // Fill the tilemap with the specified tile type
        for (int x = -25; x <= 25; x++)
        {
            for (int y = -25; y <= 25; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                tilemap.SetTile(tilePosition, tileType);
            }
        }
        Debug.Log("Tilemap filled with " + tileType.name + " tiles.");
    }

    void Update()
    {
        // Debug.Log("Tilemap spawner updating...");
    }
}
