using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Settings")]
    public Transform playerTransform; // Referensi ke transform player
    public float cameraWidth = 4.5f; // Setengah lebar viewport (untuk kamera dengan size 4.5)
    public float minDistanceToFollow = 0; // Jarak minimal player dari tengah kamera sebelum kamera mulai mengikuti
    
    private float lastPositionX;
    private bool isFollowing = false;
    private Camera mainCamera;
    
    private void Start()
    {
        mainCamera = GetComponent<Camera>();
        
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
        // Simpan posisi awal kamera
        lastPositionX = transform.position.x;
    }
    
    private void LateUpdate()
    {
        if (playerTransform == null) return;
        
        float playerCenterOffsetX = playerTransform.position.x - transform.position.x;
        
        // Mulai mengikuti jika player sudah mencapai tengah kamera
        if (!isFollowing && playerCenterOffsetX >= minDistanceToFollow)
        {
            isFollowing = true;
        }
        
        // Jika sudah mengikuti, kamera hanya bergerak ke kanan mengikuti player
        if (isFollowing)
        {
            // Hanya bergerak ke kanan (tidak pernah ke kiri)
            float targetX = Mathf.Max(lastPositionX, playerTransform.position.x);
            
            // Mengupdate posisi kamera
            Vector3 newPosition = transform.position;
            newPosition.x = targetX;
            transform.position = newPosition;
            
            // Simpan posisi terakhir
            lastPositionX = transform.position.x;
        }
    }
}