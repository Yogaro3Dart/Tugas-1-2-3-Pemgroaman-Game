using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTugas2Loader : MonoBehaviour
{
    // Fungsi untuk button Obstacle (ke Tugas 1A)
    public void LoadTugas1A()
    {
        SceneManager.LoadScene("Tugas2A");
    }

    // Fungsi untuk button Hole (ke Tugas 1B)
    public void LoadTugas1B()
    {
        SceneManager.LoadScene("Tugas2B");
    }

    // Fungsi untuk button Back (ke HomeScreen)
    public void BackToHome()
    {
        SceneManager.LoadScene("HomeScreen");
    }
}