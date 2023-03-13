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
    [SerializeField] private TMP_Text _wavesText;
    [SerializeField] private Image _barImage;
    [SerializeField] private List<GameObject> _laserImageUI;

    private GameManager _gameManager;
    private Player _player;
    private Asteroid _asteroid;

    private bool _isNoAmmoTextActive = false;
    private bool _routineRunning = false;
    private bool _shouldEmptyBar = false;
    private bool _shouldStopFillingUpBar = false;
    private bool _isFillingUpBar = false;
    private bool _isWavesTextShowing = false;

    private Color _thrusterBarColor;

    void Start()
    { 
        _scoreText.text = "Score: " + 0;
        _gameOverText.gameObject.SetActive(false);
        _resetSceneText.gameObject.SetActive(false);
        _noAmmoText.gameObject.SetActive(false);
        _wavesText.gameObject.SetActive(false);

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

        _asteroid = GameObject.Find("Asteroid").GetComponent<Asteroid>();
        if (_asteroid == null)
        {
            Debug.LogError("Asteroid in UIManager is NULL");
        }

        _ammoText.color = Color.green;
        _thrusterBarColor = _barImage.color;

    }

    private void Update()
    {
        if (_asteroid.HasDestroyedAsteroid)
        {
            _ammoText.gameObject.SetActive(true);
        }
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
        _resetSceneText.text = "Press the 'R' key to restart the level";
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
        _isNoAmmoTextActive = false;
        _noAmmoText.gameObject.SetActive(false);
    }

    public void SetTextColor(Color color)
    {
        _ammoText.color = color;
    }

    public void AmmoTextActive()
    {
        _isNoAmmoTextActive = true;
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
        _ammoText.text = ammoCount.ToString() + " / 15";
    }

    public void DisplayNoAmmoText()
    {
        if (!_routineRunning)
        {
            _isNoAmmoTextActive = true;
            _routineRunning = true;
            _noAmmoText.gameObject.SetActive(true);
            StartCoroutine(NoAmmoFlickerRoutine());
        }
        
    }

    IEnumerator NoAmmoFlickerRoutine()
    {
        while (_isNoAmmoTextActive)
        {
            _noAmmoText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.3f);
            _noAmmoText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.6f);
        }
        _routineRunning = false;
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

    public void EmptyBar(bool emptyBar)
    {
        if (emptyBar)
        {
            _shouldEmptyBar = true;
        }
        else
        {
            _shouldEmptyBar = false;
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

            if (_player.GetNegativeSpeed())
            {
                _shouldEmptyBar = false;
                ResetBar();
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

    public void UpdateAndDisplayWaveText(int wave)
    {
        _wavesText.text = "Wave " + wave;
        _wavesText.gameObject.SetActive(true);
        _isWavesTextShowing = true;
        StartCoroutine(WavesFlickerRoutine(wave));
        StartCoroutine(HideWavesTextRoutine());
    }

    IEnumerator HideWavesTextRoutine()
    {
        yield return new WaitForSeconds(3.5f);
        _wavesText.gameObject.SetActive(false);
        _isWavesTextShowing = false;
    }

    IEnumerator WavesFlickerRoutine(int wave)
    {
        while (_isWavesTextShowing)
        {
            _wavesText.text = "Wave " + wave;
            yield return new WaitForSeconds(0.5f);
            _wavesText.text = "";
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void AllWavesCompleted()
    {
        _wavesText.text = "All waves completed!";
        _wavesText.gameObject.SetActive(true);
        _resetSceneText.text = "Press the 'R' key to restart the game";
        _resetSceneText.gameObject.SetActive(true);
        _gameManager.CompletedGame();
        _noAmmoText.gameObject.SetActive(false);
    }

    public void UpdateThrusterBarColor(bool disabled)
    {
        if (disabled)
        {
            _barImage.color = Color.gray;
        }
        else
        {
            _barImage.color = _thrusterBarColor;
        }
    }

    public void ResetBar()
    {
        _barImage.fillAmount = 1.0f;
    }

    public void StopDisplayingAllText()
    {
        _noAmmoText.gameObject.SetActive(false);
        _wavesText.gameObject.SetActive(false);
        _isNoAmmoTextActive = false;
    }

    public void AmmoCountEnabledOnStart()
    {
        _ammoText.text = "15 / 15";
    }
}
