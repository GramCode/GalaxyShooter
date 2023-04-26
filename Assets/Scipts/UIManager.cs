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
    [SerializeField] private TMP_Text _wavesCountText;
    [SerializeField] private Image _barImage;
    [SerializeField] private List<GameObject> _laserImageUI;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _projectileBackground;
    [SerializeField] private GameObject _prohibitionSign;
    [SerializeField] private Sprite[] _bossLivesSprites;
    [SerializeField] private Image _bossLivesImage;
    [SerializeField] private Image _countDownFillImage;
    [SerializeField] private GameObject _countDownGameObject;

    private GameManager _gameManager;
    private Player _player;
    private Asteroid _asteroid;
    private Animator _wavesCountTextAnim;

    private bool _isNoAmmoTextActive = false;
    private bool _routineRunning = false;
    private bool _shouldEmptyBar = false;
    private bool _shouldStopFillingUpBar = false;
    private bool _isFillingUpBar = false;
    private bool _isWavesTextShowing = false;
    private Vector3 _wavesTextStartingPosition;
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

        _wavesCountTextAnim = _wavesCountText.gameObject.GetComponent<Animator>();
        if (_wavesCountTextAnim == null)
        {
            Debug.LogError("The Animator for Waves Count in UI Manager is NULL");
        }

        _ammoText.color = Color.green;
        _thrusterBarColor = _barImage.color;
        _wavesTextStartingPosition = new Vector3(-331.5f, 250.43f);

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
        _resetSceneText.text = "Press the 'R' key to restart the game.";
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
                _player.CanSpeedUp(false);
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
            _barImage.fillAmount += 0.004f;

            if (_barImage.fillAmount == 1)
            {
                _shouldStopFillingUpBar = true;
                _player.CanSpeedUp(true);
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

    IEnumerator WavesFlickerRoutine(int wave)
    {
        int times = 0;
        while (_isWavesTextShowing)
        {
            times++;
            _wavesText.text = "";
            _wavesCountText.text = "";
            yield return new WaitForSeconds(0.5f);
            _wavesText.text = "Wave " + wave;
            
            yield return new WaitForSeconds(0.5f);
            

            if (times == 3)
            {
                _wavesText.text = "Wave";
                _wavesCountText.gameObject.SetActive(true);
                _wavesCountText.text = wave.ToString();
                _wavesCountTextAnim.SetTrigger("Move");
            }
        }
    }

    IEnumerator HideWavesTextRoutine()
    {
        yield return new WaitForSeconds(3.0f);
        _wavesText.gameObject.SetActive(false);
        _isWavesTextShowing = false;
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

    public void StopDisplayingNoAmmoText()
    {
        _noAmmoText.gameObject.SetActive(false);
        _isNoAmmoTextActive = false;
    }

    public void HideWaveText()
    {
        _wavesText.gameObject.SetActive(false);
    }

    public void AmmoCountEnabledOnStart()
    {
        _ammoText.text = "15 / 15";
    }

    public void PauseMenu()
    {
        Instantiate(_pauseMenu, Vector2.zero, Quaternion.identity);
        Time.timeScale = 0;
    }

    public void DisplayProjectile()
    {
        _projectileBackground.SetActive(true);
    }


    public void HideProjectile()
    {
        _projectileBackground.SetActive(false);
    }

    public void DisplayProhibitionSign()
    {
        _prohibitionSign.SetActive(true);
    }

    public void HideProhibitionSign()
    {
        _prohibitionSign.SetActive(false);
    }

    public void UpdateBossLives(int currentLives)
    {
        _bossLivesImage.sprite = _bossLivesSprites[currentLives];
    }

    public void DisplayBossLives()
    {
        _bossLivesImage.gameObject.SetActive(true);
    }

    public void HideBossLives()
    {
        _bossLivesImage.gameObject.SetActive(false);
    }

    public void HideWavesCount()
    {
        _wavesCountText.gameObject.SetActive(false);
    }

    public void ResetWaveCountTextPosition()
    {
        _wavesCountText.transform.position = _wavesTextStartingPosition;
    }

    public void DisplayWavesCount()
    {
        _wavesCountText.gameObject.SetActive(true);
    }

    public void CountDown(float duration)
    {
        _countDownGameObject.SetActive(true);

        if (_countDownFillImage.fillAmount == 0)
        {
            StartCoroutine(CountDownRoutine(duration));
        }
        else
        {
            _countDownFillImage.fillAmount = 0;
        }
    }

    IEnumerator CountDownRoutine(float duration)
    {
        float elapsedTime = 0;
        while(_countDownFillImage.fillAmount < 1)
        {
            // _countDownFillImage.fillAmount += 0.002f;
            _countDownFillImage.fillAmount += 1.0f / duration * Time.deltaTime;
             elapsedTime += Time.deltaTime;
            yield return null;
        }
        _countDownGameObject.SetActive(false);
        _countDownFillImage.fillAmount = 0;
    }
}
