using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    [Header("Referensi NPC")]
    public FollowerAI npcYangAkanBicara;

    [Header("Konten Dialog")]
    // Variabel untuk menyimpan teks yang spesifik untuk trigger ini
    // Atribut [TextArea] membuat kotak input di Inspector menjadi lebih besar dan mudah diedit
    [TextArea(3, 10)]
    public string dialogYangAkanDitampilkan;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (npcYangAkanBicara != null)
            {
                // Kirimkan teks dari variabel 'dialogYangAkanDitampilkan' ke fungsi TampilkanDialog milik NPC
                npcYangAkanBicara.TampilkanDialog(dialogYangAkanDitampilkan);
            }
            else
            {
                Debug.LogWarning("Referensi NPC belum di-set pada DialogTrigger!");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (npcYangAkanBicara != null)
            {
                npcYangAkanBicara.SembunyikanDialog();
            }
        }
    }
}