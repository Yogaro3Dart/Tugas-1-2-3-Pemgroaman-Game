using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    [Header("Tulis Nama Scene di sini")]
    [Tooltip("Nama scene harus sama persis dengan nama file scene, misal: '1-1 Grid Version'")]
    public string gameplaySceneName; // Tulis nama scene di Inspector

    public void OnMainButton()
    {
        if (!string.IsNullOrEmpty(gameplaySceneName))
        {
            // Cukup panggil nama scene yang sudah Anda tentukan.
            // Ini berfungsi di Editor DAN di Build, selama scene ada di Build Settings.
            SceneManager.LoadScene(gameplaySceneName);
        }
        else
        {
            Debug.LogError("Nama Scene Gameplay belum diisi di Inspector ButtonManager!");
        }
    }

    public void OnExitButton()
    {
        // Logika ini sudah benar, tidak perlu diubah.
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}