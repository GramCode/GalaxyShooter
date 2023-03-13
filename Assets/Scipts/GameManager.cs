using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool _isGameOver = false;
    private UIManager _uiManager;
    private SpawnManager _spawnManager;
    [SerializeField] private Enemy _enemy;

    public bool GameCompleted { get; private set; }

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

        GameCompleted = false;

    }

    private void Update()
    {
        if (_isGameOver || GameCompleted)
        {
            _uiManager.UpdateThrusterBarColor(false);
            _uiManager.StopDisplayingAllText();
            _spawnManager.GameOver();
        }

        if (Input.GetKeyDown(KeyCode.R) && _isGameOver || Input.GetKeyDown(KeyCode.R) && GameCompleted)
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
        GameCompleted = true;
    }

}
