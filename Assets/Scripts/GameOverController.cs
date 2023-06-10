using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverController : MonoBehaviour
{
    public Text scoreText;

    // Retrieves and displays the player's score
    void Start()
    {
        int finalScore = PlayerPrefs.GetInt("FinalScore");
        scoreText.text = "YOUR SCORE: " + finalScore;
    }

    // Load on new game on Restart button
    public void RestartGame()
    {
        SceneManager.LoadScene("Basketball-scene");
    }

    // Quit the game
    public void Quit()
    {
        Application.Quit();
    }
}
