using UnityEngine;
using TMPro;

public class FollowerAI : MonoBehaviour
{
    [Header("Referensi")]
    public Transform playerTransform;
    public GameObject popUpBubble;
    public TextMeshProUGUI popUpText;

    // --- BAGIAN YANG HILANG ADA DI SINI ---
    [Header("Pengaturan Gerak")]
    public float moveSpeed = 3f;
    public float stoppingDistance = 1.5f;
    public float retreatDistance = 1f;

    private Rigidbody2D rb;
    private Vector2 movement;
    // --- AKHIR BAGIAN YANG HILANG ---


    #region Gerakan NPC
    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // 'rb' butuh deklarasi di atas

        // Kode cari player yang lengkap (jangan pakai komentar)
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
            else
            {
                Debug.LogError("FollowerAI Error: Tidak dapat menemukan objek dengan Tag 'Player'.");
                this.enabled = false;
            }
        }

        if (popUpBubble != null) { popUpBubble.SetActive(false); }
    }

    void Update()
    {
        if (playerTransform != null)
        {
            Vector2 directionToPlayer = playerTransform.position - transform.position;
            directionToPlayer.Normalize();
            float distance = Vector2.Distance(transform.position, playerTransform.position);
            
            // Variabel 'stoppingDistance', 'retreatDistance', dan 'movement' butuh deklarasi di atas
            if (distance > stoppingDistance) { movement = directionToPlayer; }
            else if (distance < retreatDistance) { movement = -directionToPlayer; }
            else { movement = Vector2.zero; }

            FlipSprite(directionToPlayer.x);
        }
    }

    void FixedUpdate()
    {
        if (playerTransform != null) {
            // 'rb', 'movement', dan 'moveSpeed' butuh deklarasi di atas
            rb.MovePosition((Vector2)transform.position + (movement * moveSpeed * Time.fixedDeltaTime));
        }
    }

    void FlipSprite(float horizontalDirection)
    {
        if (horizontalDirection > 0 && transform.localScale.x < 0) { transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); }
        else if (horizontalDirection < 0 && transform.localScale.x > 0) { transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z); }
    }
    #endregion


    #region Fungsi Dialog
    // Fungsi ini sekarang menerima sebuah parameter string bernama 'pesan'
    public void TampilkanDialog(string pesan)
    {
        if (popUpBubble != null && popUpText != null)
        {
            // 1. Ubah teks di dalam bubble sesuai dengan 'pesan' yang diterima
            popUpText.text = pesan;

            // 2. Tampilkan bubble-nya
            popUpBubble.SetActive(true);
        }
    }

    public void SembunyikanDialog()
    {
        if (popUpBubble != null)
        {
            popUpBubble.SetActive(false);
        }
    }
    #endregion
}