using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool _isGameOver = false;
    private bool _completedGame = false;
    private UIManager _uiManager;
    private SpawnManager _spawnManager;
    [SerializeField] private Enemy _enemy;

    private void Start()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.LogError("UIManager on Game Manager is NULL");
        }

        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("Spawn Manager in Game Manager is NULL");
        }

    }

    private void Update()
    {
        if (_isGameOver || _completedGame)
        {
            _uiManager.UpdateThrusterBarColor(false);
            _uiManager.StopDisplayingAllText();
            _spawnManager.GameOver();
        }

        if (Input.GetKeyDown(KeyCode.R) && _isGameOver || Input.GetKeyDown(KeyCode.R) && _completedGame)
        {
            _enemy.ResetEliminatedEnemies();
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

    public bool GameHasEnded()
    {
        if (_isGameOver)
        {
            return true;
        }

        if (_completedGame)
        {
            return true;
        }

        return false;
    }
}
