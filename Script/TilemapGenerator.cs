using System.Collections.Generic;
using UnityEngine;

public class TilemapGenerator : MonoBehaviour
{
    [Header("Tilemap Settings")]
    public GameObject tilemapPrefab; // Prefab dengan lebar 16 unit
    public float tilemapWidth = 16f;

    [Header("Player Tracking")]
    public Transform player;
    public float spawnTriggerOffset = 8f; // Trigger spawn di 50% tilemap terakhir

    private List<GameObject> activeTilemaps = new List<GameObject>();
    private float nextSpawnX;
    private int currentPlayerTilemapIndex;

    void Start()
    {
        // Inisialisasi 2 tilemap awal (0-16 dan 16-32)
        SpawnTilemap(0f);
        SpawnTilemap(16f);
        nextSpawnX = 32f; // Posisi spawn berikutnya: 32-48
    }

    void Update()
    {
        currentPlayerTilemapIndex = Mathf.FloorToInt(player.position.x / tilemapWidth);
        
        // Spawn tilemap baru saat player di tengah tilemap terakhir
        if (player.position.x > nextSpawnX - tilemapWidth - spawnTriggerOffset)
        {
            SpawnTilemap(nextSpawnX);
            nextSpawnX += tilemapWidth;
        }

        // Hapus tilemap yang 2 indeks di belakang player
        DeleteOldTilemaps();
    }

    void DeleteOldTilemaps()
    {
        if (currentPlayerTilemapIndex < 2) return; // Tidak hapus jika player di tilemap 0/1

        // Hitung indeks tilemap yang harus dihapus
        int targetIndex = currentPlayerTilemapIndex - 2;
        float targetX = targetIndex * tilemapWidth;

        // Cari tilemap yang sesuai dengan targetX
        for (int i = activeTilemaps.Count - 1; i >= 0; i--)
        {
            float tilemapX = activeTilemaps[i].transform.position.x;
            
            if (Mathf.Approximately(tilemapX, targetX))
            {
                Destroy(activeTilemaps[i]);
                activeTilemaps.RemoveAt(i);
                Debug.Log($"Dihapus tilemap {targetX}-{targetX + tilemapWidth}");
                break; // Hapus hanya satu tilemap per frame
            }
        }
    }

    void SpawnTilemap(float xPosition)
    {
        GameObject newTilemap = Instantiate(
            tilemapPrefab,
            new Vector3(xPosition, 0f, 0f),
            Quaternion.identity
        );
        
        activeTilemaps.Add(newTilemap);
        Debug.Log($"Dibuat tilemap {xPosition}-{xPosition + tilemapWidth}");
    }
}