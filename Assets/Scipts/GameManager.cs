using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool IsGameOver { get; private set; }
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
        IsGameOver = false;
    }

    private void Update()
    {
        if (IsGameOver || GameCompleted)
        {
            _uiManager.HideBossLives();
            _uiManager.UpdateThrusterBarColor(false);
            _uiManager.StopDisplayingNoAmmoText();
            _spawnManager.StopSpawning();
        }

        if (Input.GetKeyDown(KeyCode.R) && IsGameOver || Input.GetKeyDown(KeyCode.R) && GameCompleted)
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
        IsGameOver = true;
    }

    public void CompletedGame()
    {
        GameCompleted = true;
    }

}
