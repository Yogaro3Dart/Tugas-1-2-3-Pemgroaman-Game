using UnityEngine;

public class CheckpointTilemapSwitcher : MonoBehaviour
{
    public GameObject tilemapLevel1; // Tilemap Level 1 (aktif di awal)
    public GameObject tilemapLevel2; // Tilemap Level 2 (muncul setelah checkpoint)

    void Start()
    {
        // Pastikan hanya Level 1 yang aktif di awal
        tilemapLevel1.SetActive(true);
        tilemapLevel2.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SwapTilemap();
        }
    }

    void SwapTilemap()
    {
        tilemapLevel1.SetActive(false);
        tilemapLevel2.SetActive(true);
    }
}