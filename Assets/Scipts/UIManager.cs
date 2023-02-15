using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private Sprite[] _liveSprites;
    [SerializeField] private Image _livesImage;
    [SerializeField] private TMP_Text _gameOverText;
    [SerializeField] private TMP_Text _resetSceneText;

    private GameManager _gameManager;

    void Start()
    { 
        _scoreText.text = "Score: " + 0;
        _gameOverText.gameObject.SetActive(false);
        _resetSceneText.gameObject.SetActive(false);

        _gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        if (_gameManager == null)
        {
            Debug.LogError("Game manager is NULL");
        }
    }

    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore.ToString();
    }

    public void UpdateLives(int currentLives)
    {
        _livesImage.sprite = _liveSprites[currentLives];

        if (currentLives == 0)
        {
            GameOverSequence();
        }
    }

    void GameOverSequence()
    {

        _gameManager.GameOver();

        _gameOverText.gameObject.SetActive(true);
        _resetSceneText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlickerRoutine());
        
    }

    IEnumerator GameOverFlickerRoutine()
    {
        while (true)
        {
            _gameOverText.text = "GAME OVER";
            yield return new WaitForSeconds(0.5f);
            _gameOverText.text = "";
            yield return new WaitForSeconds(0.5f);
        }
    }

}
