using UnityEngine;
public class PuzzleUIController : MonoBehaviour
{
    public GameObject puzzlePanel, pipePuzzlePanel;

    public void OnYesButton()
    {
        puzzlePanel.SetActive(false);
        pipePuzzlePanel.SetActive(true);
    }

    public void OnNoButton()
    {
        puzzlePanel.SetActive(false);
        Time.timeScale = 1f; // resume game
    }
}
