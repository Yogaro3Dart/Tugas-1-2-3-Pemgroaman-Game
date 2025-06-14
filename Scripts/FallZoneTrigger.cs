using UnityEngine;

public class FallZoneTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            FindAnyObjectByType<CheckpointManager>().RespawnPlayer();
        }
    }
}
