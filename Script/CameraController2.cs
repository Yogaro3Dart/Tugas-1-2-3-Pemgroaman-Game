using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController2 : MonoBehaviour
{
    [Header("Settings")]
    public Transform playerTransform; // Referensi ke transform player
    public float cameraWidth = 4.5f; // Setengah lebar viewport (untuk kamera dengan size 4.5)
    public float minDistanceToFollow = 0; // Jarak minimal player dari tengah kamera sebelum kamera mulai mengikuti
    public float followSpeed = 3f; // Kecepatan mengikuti player
    public float verticalOffset = 1f; // Offset agar kamera tidak terlalu menempel ke player

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
        float playerCenterOffsetY = playerTransform.position.y - transform.position.y;

        // Mulai mengikuti jika player sudah mencapai tengah kamera
        if (!isFollowing && playerCenterOffsetX >= minDistanceToFollow)
        {
            isFollowing = true;
        }

        // Jika sudah mengikuti, kamera mengikuti pergerakan player ke kanan dan vertikal
        if (isFollowing)
        {
            float targetX = Mathf.Max(lastPositionX, playerTransform.position.x);
            float targetY = playerTransform.position.y + verticalOffset; // Kamera mengikuti posisi vertikal player dengan offset

            // Mengupdate posisi kamera dengan interpolasi agar lebih halus
            Vector3 newPosition = transform.position;
            newPosition.x = Mathf.Lerp(transform.position.x, targetX, Time.deltaTime * followSpeed);
            newPosition.y = Mathf.Lerp(transform.position.y, targetY, Time.deltaTime * followSpeed);

            transform.position = newPosition;

            // Simpan posisi terakhir
            lastPositionX = transform.position.x;
        }
    }
}