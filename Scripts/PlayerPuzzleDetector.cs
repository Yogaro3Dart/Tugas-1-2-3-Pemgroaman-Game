using UnityEngine;

public class PlayerPuzzleDetector : MonoBehaviour
{
    private TileMatchingController puzzleController;

    // Penanda bahwa sesi puzzle sedang berjalan
    private bool isPuzzleSessionActive = false;

    // Penanda bahwa puzzle ini sudah selesai
    private bool isPuzzleCompleted = false;

    private PlayerMovement playerMovement;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        puzzleController = FindAnyObjectByType<TileMatchingController>();
        if (puzzleController == null)
        {
            Debug.LogError("PlayerPuzzleDetector tidak bisa menemukan objek dengan script TileMatchingController di scene!");
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Cek apakah yang disentuh adalah pemicu
        if (other.CompareTag("PuzzleTrigger"))
        {
            // Trigger hanya aktif jika player berhenti, sesi puzzle tidak aktif, DAN puzzle belum pernah selesai.
            if (playerMovement.grounded && !isPuzzleSessionActive && !isPuzzleCompleted)
            {
                Debug.Log("Player berada di tile pemicu! Mengaktifkan puzzle...");

                // Kunci pergerakan player
                playerMovement.enabled = false;

                if (puzzleController != null)
                {
                    puzzleController.ActivatePuzzle();
                    isPuzzleSessionActive = true;
                }

                // Berlangganan event puzzle selesai
                TileMatchingController.OnPuzzleCompleted += OnPuzzleFinished;
            }
        }
    }

    private void OnPuzzleFinished()
    {
        Debug.Log("DETEKTOR: Puzzle selesai! Membuka kunci player.");

        // Aktifkan kembali pergerakan player
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        isPuzzleSessionActive = false;

        // Tandai bahwa puzzle ini sudah selesai
        isPuzzleCompleted = true;

        // Berhenti berlangganan event
        TileMatchingController.OnPuzzleCompleted -= OnPuzzleFinished;
    }

    private void OnDestroy()
    {
        // Pastikan kita selalu berhenti berlangganan event
        if (puzzleController != null)
        {
            TileMatchingController.OnPuzzleCompleted -= OnPuzzleFinished;
        }
    }
}