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
    [SerializeField] private TMP_Text _ammoText;
    [SerializeField] private TMP_Text _noAmmoText;

    [SerializeField] private Image _barImage;

    [SerializeField] private List<GameObject> _laserImageUI;

    private GameManager _gameManager;
    private Player _player;

    private bool _shouldEmptyBar = false;
    private bool _shouldStopFillingUpBar = false;
    private bool _isFillingUpBar = false;
    
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

        _player = GameObject.Find("Player").GetComponent<Player>();

        if (_player == null)
        {
            Debug.LogError("Player is null in UIManager");
        }

        _ammoText.color = Color.green;
    }


    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore.ToString();
    }

    public void UpdateLives(int currentLives)
    {

        if (currentLives == 0)
        {
            GameOverSequence();
        }
        _livesImage.sprite = _liveSprites[currentLives];

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

    public void HideBullet(int index)
    {
        _laserImageUI[index].SetActive(false);
    }

    public void HideNoAmmoText()
    {
        _noAmmoText.gameObject.SetActive(false);
    }

    public void SetTextColor(Color color)
    {
        _ammoText.color = color;
    }

    public void DisplayBullets()
    {
        foreach (var bullet in _laserImageUI)
        {
            bullet.SetActive(true);
        }
    }

    public void UpdateAmmoText(int ammoCount)
    {
        _ammoText.text = ammoCount.ToString();
    }

    public void DisplayNoAmmoText()
    {
        _noAmmoText.gameObject.SetActive(true);
        StartCoroutine(NoAmmoFlickerRoutine());
    }

    IEnumerator NoAmmoFlickerRoutine()
    {
        while (_player.IsDisplayingNoAmmoText())
        {
            _noAmmoText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.3f);
            _noAmmoText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.6f);
        }
    }

    public void UpdateBar(bool leftShiftPressed)
    {
        if (leftShiftPressed == true)
        {
            _shouldStopFillingUpBar = true;
            _shouldEmptyBar = true;
            _player.CanSpeedUp(true);
            StartCoroutine(EmptyBarRoutine());
        }
        else
        {
            StartFillingUpBar();
        }
    }

    private void StartFillingUpBar()
    {
        _shouldEmptyBar = false;
        _shouldStopFillingUpBar = false;
        _player.CanSpeedUp(false);

        if (!_isFillingUpBar)
            StartCoroutine(FillUpBarRoutine());
    }

    IEnumerator EmptyBarRoutine()
    {
        while (_shouldEmptyBar)
        {
            yield return new WaitForEndOfFrame();
            _barImage.fillAmount -= 0.002f;

            if (_barImage.fillAmount == 0)
            {
                StartFillingUpBar();
            }
        }        
    }

    IEnumerator FillUpBarRoutine()
    {
        _isFillingUpBar = true;
        yield return new WaitForSeconds(0.5f);
        while (!_shouldStopFillingUpBar)
        {
            yield return new WaitForEndOfFrame();
            _barImage.fillAmount += 0.001f;

            if (_barImage.fillAmount == 1)
            {
                _shouldStopFillingUpBar = true;
            }
        }
        _isFillingUpBar = false;
    }
}
