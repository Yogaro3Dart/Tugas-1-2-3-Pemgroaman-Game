using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTugas1Loader : MonoBehaviour
{
    // Fungsi untuk button Obstacle (ke Tugas 1A)
    public void LoadTugas1A()
    {
        SceneManager.LoadScene("Tugas1A");
    }

    // Fungsi untuk button Hole (ke Tugas 1B)
    public void LoadTugas1B()
    {
        SceneManager.LoadScene("Tugas1B");
    }

    // Fungsi untuk button Back (ke HomeScreen)
    public void BackToHome()
    {
        SceneManager.LoadScene("HomeScreen");
    }
}