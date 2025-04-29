using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadTugas1()
    {
        SceneManager.LoadScene("Tugas1");
    }

    public void LoadTugas2()
    {
        SceneManager.LoadScene("Tugas2");
    }

    public void LoadTugas3()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void ExitGame()
    {
        // Untuk editor Unity
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        // Untuk build game
        Application.Quit();
        #endif
    }
}