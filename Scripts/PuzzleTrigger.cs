using UnityEngine;

public class PuzzleTrigger : MonoBehaviour
{
    public GameObject puzzlePanel; // UI pop-up
    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E)) // atau tombol virtual
        {
            puzzlePanel.SetActive(true); // munculkan pop-up
            Time.timeScale = 0f; // pause game
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}
