using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    private Vector2 lastCheckpointPos;
    public GameObject player;
    public float respawnDelay = 0.5f;

    private void Start()
    {
        // Atur posisi awal sebagai checkpoint awal
        lastCheckpointPos = player.transform.position;
    }

    public void SetCheckpoint(Vector2 newPos)
    {
        lastCheckpointPos = newPos;
    }

    public void RespawnPlayer()
    {
        StartCoroutine(RespawnDelay());
    }

    private System.Collections.IEnumerator RespawnDelay()
    {
        player.SetActive(false); // Matikan player sementara
        yield return new WaitForSeconds(respawnDelay);
        player.transform.position = lastCheckpointPos;
        player.SetActive(true);
    }
}
