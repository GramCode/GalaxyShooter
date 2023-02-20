using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private int _lives = 3;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _trippleShotPrefab;
    [SerializeField] private GameObject _trippleShotPowerUp;
    [SerializeField] private GameObject _shieldGameObject;
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private AudioClip _audioClip; 
    [SerializeField] private GameObject[] _fireBallDamaged;


    private SpawnManager _spawnManager;
    private UIManager _uiManager;
    private AudioSource _audioSource;
    private float _speedMultiplier = 2f;
    private float _fireRate = 0.5f;
    private float _canFire = -1f;
    private bool _isTrippleShotActive = false;
    private bool _isSpeedBoostActive = false;
    private bool _isShieldActive = false;
    private int _score;


    private void Start()
    {
        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();
        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is null");
        }

        if (_uiManager == null)
        {
            Debug.LogError("The UIManager is NULL");
        }

        if (_audioSource == null)
        {
            Debug.LogError("AudioSource on the player is NULL.");
        }
        else
        {
            _audioSource.clip = _audioClip;
        }
   
    }

    void Update()
    {
        CalculateMovement();
        ShootLaser();
        PlayerBounds();

    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 distance = new Vector3(horizontalInput, verticalInput, 0);
        if (_isSpeedBoostActive)
        {
            transform.Translate(distance * (_speed * _speedMultiplier) * Time.deltaTime, 0);
        }
        else
        {
            transform.Translate(distance * _speed * Time.deltaTime, 0);
        }
        
    }

    void ShootLaser()
    {
        //Check if the space key is pressed and 0.5s has passed
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            _canFire = Time.time + _fireRate;

            if (_isTrippleShotActive)
            {
                //Fire three lasers
                Instantiate(_trippleShotPrefab, transform.position, Quaternion.identity);
            }
            else
            {
                //Fire just one laser
                Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
            }
            //Play laser audio clip
            _audioSource.Play();
        }
    }

    void PlayerBounds()
    {
        if (transform.position.y >= 0)
        {
            transform.position = new Vector3(transform.position.x, 0, 0);
        }
        else if (transform.position.y <= -3.8f)
        {
            transform.position = new Vector3(transform.position.x, -3.8f, 0);
        }

        if (transform.position.x > 11.3f)
        {
            transform.position = new Vector3(-11.3f, transform.position.y);
        }
        else if (transform.position.x < -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y, 0);
        }
    }

    public void Damage()
    {
        if (_isShieldActive)
        {
            //Shield becomes unactive
            _isShieldActive = false;
            _shieldGameObject.SetActive(false);
            return;
        }

        _lives--;

        if (_lives == 2)
        {
            _fireBallDamaged[0].SetActive(true);
        }
        else if (_lives == 1)
        {
            _fireBallDamaged[1].SetActive(true);
        }

        _uiManager.UpdateLives(_lives);

        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);

            Destroy(this.gameObject, 0.2f);
        }
    }

    public void TripleShotActive()
    {
        _isTrippleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isTrippleShotActive = false;
    }

    public void SpeedBoostActive()
    {
        _isSpeedBoostActive = true;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isSpeedBoostActive = false;
    }

    public void ShieldActive()
    {
        _isShieldActive = true;
        _shieldGameObject.SetActive(true);
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }

}
