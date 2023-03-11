using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool _isGameOver = false;
    private bool _completedGame = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && _isGameOver || Input.GetKeyDown(KeyCode.R) && _completedGame)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void GameOver()
    {
        _isGameOver = true;
    }

    public void CompletedGame()
    {
        _completedGame = true;
    }

    public bool HaveCompletedGame()
    {
        return _completedGame;
    }
}
