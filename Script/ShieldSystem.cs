using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSystem : MonoBehaviour
{
    [Header("Shield Settings")]
    [SerializeField] private KeyCode shieldKey = KeyCode.CapsLock; // Tombol untuk mengaktifkan perisai
    [SerializeField] private GameObject shieldPrefab; // Prefab kubah perisai (opsional)
    [SerializeField] private float shieldRadius = 2f; // Radius kubah perisai
    [SerializeField] private Color shieldColor = new Color(0.2f, 0.5f, 1f, 0.5f); // Warna kubah perisai
    
    [Header("Energy Settings")]
    [SerializeField] private float maxEnergy = 100f; // Energi maksimal untuk perisai
    [SerializeField] private float energyDrainRate = 20f; // Kecepatan berkurangnya energi saat perisai aktif
    [SerializeField] private float energyRegenRate = 10f; // Kecepatan regenerasi energi saat perisai nonaktif
    [SerializeField] private float minEnergyToActivate = 20f; // Energi minimal untuk bisa mengaktifkan perisai
    
    private float currentEnergy; // Energi saat ini
    private bool shieldActive = false; // Status perisai
    private GameObject shieldObject; // Objek perisai
    
    void Start()
    {
        currentEnergy = maxEnergy; // Mulai dengan energi penuh
        
        // Buat objek perisai jika prefab tidak disediakan
        if (shieldPrefab == null)
        {
            CreateDefaultShield();
        }
        else
        {
            // Spawn shield dari prefab
            shieldObject = Instantiate(shieldPrefab, transform.position, Quaternion.identity);
            shieldObject.transform.parent = transform; // Jadikan child dari player
            shieldObject.SetActive(false); // Nonaktifkan pada awalnya
        }
        
        // Pastikan shield tidak mempengaruhi fisika karakter
        if (shieldObject != null && shieldObject.GetComponent<Rigidbody2D>() == null)
        {
            // Tambahkan komponen Rigidbody2D khusus untuk shield
            Rigidbody2D shieldRb = shieldObject.AddComponent<Rigidbody2D>();
            shieldRb.isKinematic = true; // Tidak terpengaruh gravitasi atau gaya
            shieldRb.gravityScale = 0f;  // Tidak terpengaruh gravitasi
        }
    }
    
    void Update()
    {
        HandleShieldInput();
        UpdateEnergy();
        UpdateShieldPosition();
        
        // Debug info untuk energi
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Shield Energy: " + currentEnergy + "/" + maxEnergy);
        }
    }
    
    void HandleShieldInput()
    {
        // Aktifkan perisai saat tombol ditekan dan energi cukup
        if (Input.GetKey(shieldKey) && currentEnergy > minEnergyToActivate)
        {
            ActivateShield();
        }
        // Nonaktifkan perisai saat tombol dilepas atau energi habis
        else if (shieldActive && (Input.GetKeyUp(shieldKey) || currentEnergy <= 0))
        {
            DeactivateShield();
        }
    }
    
    void UpdateEnergy()
    {
        // Kurangi energi saat perisai aktif
        if (shieldActive)
        {
            currentEnergy -= energyDrainRate * Time.deltaTime;
            currentEnergy = Mathf.Max(0, currentEnergy); // Pastikan tidak kurang dari 0
        }
        // Isi energi saat perisai nonaktif
        else if (currentEnergy < maxEnergy)
        {
            currentEnergy += energyRegenRate * Time.deltaTime;
            currentEnergy = Mathf.Min(currentEnergy, maxEnergy); // Pastikan tidak lebih dari maksimal
        }
    }
    
    void UpdateShieldPosition()
    {
        if (shieldObject != null)
        {
            // Update posisi perisai agar selalu di tengah karakter
            shieldObject.transform.position = transform.position;
        }
    }
    
    void ActivateShield()
    {
        if (!shieldActive)
        {
            shieldActive = true;
            shieldObject.SetActive(true);
        }
    }
    
    void DeactivateShield()
    {
        shieldActive = false;
        shieldObject.SetActive(false);
    }
    
    void CreateDefaultShield()
    {
        // Buat game object baru untuk perisai
        shieldObject = new GameObject("Shield");
        shieldObject.transform.parent = transform; // Jadikan child dari player
        shieldObject.transform.localPosition = Vector3.zero; // Posisikan di tengah player
        
        // Tambahkan SpriteRenderer untuk visualisasi
        SpriteRenderer renderer = shieldObject.AddComponent<SpriteRenderer>();
        renderer.sprite = CreateCircleSprite();
        renderer.color = shieldColor;
        renderer.sortingOrder = 5; // Pastikan terlihat di depan kebanyakan objek
        
        // Tambahkan CircleCollider2D untuk menahan objek lain
        CircleCollider2D collider = shieldObject.AddComponent<CircleCollider2D>();
        collider.radius = shieldRadius;
        collider.isTrigger = true; // Gunakan trigger agar tidak mendorong karakter
        
        // Set skala sesuai radius yang diinginkan
        float scaleFactor = shieldRadius * 2; // Diameter = 2 * radius
        shieldObject.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1);
        
        // Nonaktifkan pada awalnya
        shieldObject.SetActive(false);
    }

    // Fungsi untuk membuat sprite lingkaran secara dinamis
    Sprite CreateCircleSprite()
    {
        // Ukuran tekstur
        int textureSize = 256;
        
        // Buat tekstur baru
        Texture2D texture = new Texture2D(textureSize, textureSize);
        
        // Radius lingkaran dalam piksel
        float radius = textureSize / 2f;
        
        // Titik tengah tekstur
        Vector2 center = new Vector2(textureSize / 2f, textureSize / 2f);
        
        // Isi tekstur dengan lingkaran
        for (int y = 0; y < textureSize; y++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                // Hitung jarak dari titik saat ini ke titik tengah
                float distance = Vector2.Distance(new Vector2(x, y), center);
                
                // Warna piksel (putih dengan alpha berdasarkan jarak ke tepi)
                Color pixelColor;
                
                if (distance < radius)
                {
                    // Bagian dalam lingkaran (putih solid)
                    pixelColor = Color.white;
                    
                    // Buat efek glow di tepi
                    if (distance > radius * 0.8f)
                    {
                        // Fade out di tepi
                        float alpha = 1f - ((distance - radius * 0.8f) / (radius * 0.2f));
                        pixelColor = new Color(1, 1, 1, alpha);
                    }
                }
                else
                {
                    // Bagian luar lingkaran (transparan)
                    pixelColor = Color.clear;
                }
                
                texture.SetPixel(x, y, pixelColor);
            }
        }
        
        // Terapkan perubahan
        texture.Apply();
        
        // Buat sprite dari tekstur
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, textureSize, textureSize), new Vector2(0.5f, 0.5f), 100f);
        
        return sprite;
    }
    
    // Method untuk mengecek apakah perisai sedang aktif
    public bool IsShieldActive()
    {
        return shieldActive;
    }
    
    // Method untuk mendapatkan persentase energi
    public float GetEnergyPercentage()
    {
        return currentEnergy / maxEnergy;
    }
    
    // Method untuk mendapatkan objek perisai
    public GameObject GetShieldObject()
    {
        return shieldObject;
    }
    
    // Untuk visualisasi di editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 0.5f, 1f, 0.3f);
        Gizmos.DrawSphere(transform.position, shieldRadius);
    }
    
    // Method untuk mendeteksi collision dengan projectile atau bomb
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Jika shield aktif, cek collision dengan bomb
        if (shieldActive && collision.gameObject.GetComponent<Bomb>() != null)
        {
            // Tidak ada efek fisika, tapi kita bisa menambahkan efek visual disini
            // Contoh: Shield berkilau saat terkena bomb
            StartCoroutine(ShieldHitEffect());
        }
    }
    
    // Efek visual ketika shield terkena hit
    private IEnumerator ShieldHitEffect()
    {
        if (shieldObject != null && shieldObject.GetComponent<SpriteRenderer>() != null)
        {
            SpriteRenderer renderer = shieldObject.GetComponent<SpriteRenderer>();
            Color originalColor = renderer.color;
            
            // Kilauan warna putih
            renderer.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            
            // Kembali ke warna asli
            renderer.color = originalColor;
        }
        else
        {
            yield return null;
        }
    }
}