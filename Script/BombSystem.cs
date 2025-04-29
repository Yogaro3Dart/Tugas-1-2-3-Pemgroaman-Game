using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombSystem : MonoBehaviour
{
    [Header("Bomb Settings")]
    [SerializeField] private GameObject bombPrefab; // Prefab bomb yang akan dilempar
    [SerializeField] private KeyCode throwKey = KeyCode.Space; // Tombol untuk melempar bomb
    [SerializeField] private float throwForce = 8f; // Kekuatan lemparan (ditingkatkan)
    [SerializeField] private float spawnDistance = 1f; // Jarak spawn bomb dari player
    [SerializeField] private float cooldown = 1f; // Cooldown antar lemparan
    [SerializeField] private int maxBombs = 3; // Jumlah bomb maksimal
    
    [Header("Explosion Settings")]
    [SerializeField] private float explosionDelay = 3f; // Waktu sebelum meledak
    [SerializeField] private float explosionRadius = 3f; // Radius ledakan
    [SerializeField] private int damage = 50; // Damage ledakan
    [SerializeField] private GameObject explosionEffectPrefab; // Efek ledakan (opsional)
    [SerializeField] private AudioClip explosionSound; // Suara ledakan (opsional)
    
    private float lastThrowTime; // Waktu terakhir melempar bomb
    private List<GameObject> activeBombs = new List<GameObject>(); // Daftar bomb aktif
    
    void Update()
    {
        // Hapus referensi null dari daftar bomb aktif
        activeBombs.RemoveAll(bomb => bomb == null);
        
        // Cek apakah pemain bisa melempar bomb
        if (Input.GetKeyDown(throwKey) && 
            activeBombs.Count < maxBombs && 
            Time.time - lastThrowTime >= cooldown)
        {
            ThrowBomb();
        }
    }
    
    void ThrowBomb()
    {
        // Ambil arah lemparan (ke arah mouse di layar)
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        Vector2 throwDirection = (mousePosition - transform.position).normalized;
        
        // Posisi spawn bomb (di depan player, bukan di posisi player)
        Vector3 spawnPosition = transform.position + (Vector3)throwDirection * spawnDistance; // Gunakan jarak yang bisa diatur
        
        // Spawn bomb dan tambahkan ke daftar aktif
        GameObject newBomb = Instantiate(bombPrefab, spawnPosition, Quaternion.identity);
        activeBombs.Add(newBomb);
        lastThrowTime = Time.time;
        
        // Tambahkan komponen Bomb ke game object
        Bomb bombComponent = newBomb.AddComponent<Bomb>();
        bombComponent.Initialize(explosionDelay, explosionRadius, damage, explosionEffectPrefab, explosionSound);
        
        // Tambahkan Rigidbody2D jika belum ada
        Rigidbody2D rb = newBomb.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = newBomb.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0.5f; // Menambahkan sedikit gravitasi agar lebih realistis
        }
        
        // Lempar bomb dengan gaya yang lebih besar
        rb.AddForce(throwDirection * throwForce, ForceMode2D.Impulse);
    }
    
    // Untuk melihat radius ledakan di editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}

// Class Bomb yang dipakai oleh setiap prefab bomb
public class Bomb : MonoBehaviour
{
    private float explosionDelay;
    private float explosionRadius;
    private int damage;
    private GameObject explosionEffectPrefab;
    private AudioClip explosionSound;
    
    private float timer;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    
    public void Initialize(float delay, float radius, int dmg, GameObject effectPrefab, AudioClip sound)
    {
        explosionDelay = delay;
        explosionRadius = radius;
        damage = dmg;
        explosionEffectPrefab = effectPrefab;
        explosionSound = sound;
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }
    
    void Update()
    {
        timer += Time.deltaTime;
        
        // Efek kedipan saat mendekati ledakan
        if (spriteRenderer != null)
        {
            float blinkSpeed = Mathf.Lerp(1f, 10f, timer / explosionDelay);
            float alpha = Mathf.PingPong(Time.time * blinkSpeed, 1f);
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(0.5f, 1f, alpha));
        }
        
        // Meledak saat timer mencapai delay
        if (timer >= explosionDelay)
        {
            Explode();
        }
    }
    
    void Explode()
    {
        // Putar suara ledakan
        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position, 1f);
        }
        
        // Spawn efek ledakan
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }
        
        // Cari semua objek dalam radius ledakan
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        
        foreach (Collider2D hit in colliders)
        {
            // Cek apakah objek bisa terkena damage
            Damageable damageable = hit.GetComponent<Damageable>();
            if (damageable != null)
            {
                // Hitung damage berdasarkan jarak
                float distance = Vector2.Distance(transform.position, hit.transform.position);
                float damageMultiplier = 1f - (distance / explosionRadius);
                int finalDamage = Mathf.RoundToInt(damage * damageMultiplier);
                damageable.TakeDamage(finalDamage);
            }
            
            // Terapkan gaya dorong pada objek fisika
            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
            if (rb != null && hit.gameObject != gameObject)
            {
                Vector2 direction = (hit.transform.position - transform.position).normalized;
                float force = 10f * (1f - (Vector2.Distance(transform.position, hit.transform.position) / explosionRadius));
                rb.AddForce(direction * force, ForceMode2D.Impulse);
            }
        }
        
        // Hancurkan bomb
        Destroy(gameObject);
    }
}

// Class sederhana untuk objek yang bisa rusak
public class Damageable : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    
    void Start()
    {
        currentHealth = maxHealth;
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        if (currentHealth <= 0)
        {
            // Hancurkan objek ketika health = 0
            Destroy(gameObject);
        }
    }
}