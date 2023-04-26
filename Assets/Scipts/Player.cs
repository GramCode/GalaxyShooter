using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum State { turningLeft, backFromLeft, turningRight, backFromRight, balanced }

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private int _lives = 3;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _trippleShotPrefab;
    [SerializeField] private GameObject _spreadShotPrefab;
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private GameObject _shieldGameObject;
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private GameObject _thruster;
    [SerializeField] private AudioClip _audioClip; 
    [SerializeField] private GameObject[] _fireBallDamaged;
    [SerializeField] private List<Material> _shieldMaterials;
    [SerializeField] private GameObject _targetRange;

    private Animator _anim;
    private SpawnManager _spawnManager;
    private UIManager _uiManager;
    private CameraBehavior _camera;
    private AudioSource _audioSource;
    private GameManager _gameManager;
    private Asteroid _asterioid;
    private GameObject _laser;
    private GameObject _closestTarget = null;
    public GameObject projectile { get; private set; }

    private float _speedMultiplier = 2f;
    private float _fireRate = 0.5f;
    private float _canFire = -1f;

    private bool _isTripleShotActive = false;
    private bool _isSpeedBoostActive = false;
    private bool _isShieldActive = false;
    private bool _isSpreadShotActive = false;
    private bool _isNegativeSpeedActive = false;
    private bool _canSpeedUp = true;
    private bool _isProjectileActive = false;
    private bool _canShootProjectile = false;
    private bool _leftShiftPressed = false;
    

    private int _score;
    private int _shieldLives = 3;
    private int _ammoCount = 15;
    private int _uiLasersCount;

    private bool _projectileHasBeenShot = false;

    private State _state = State.balanced;

    private void Start()
    {
        _anim = GetComponent<Animator>();
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

        _camera = GameObject.Find("Main Camera").GetComponent<CameraBehavior>();

        if (_camera == null)
        {
            Debug.LogError("The Camera is NULL");
        }

        _gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("Game Manager in Player is NULL");
        }

        _asterioid = GameObject.Find("Asteroid").GetComponent<Asteroid>();
        if(_asterioid == null)
        {
            Debug.LogError("Asteroid in Player is NULL");
        }

        projectile = null;
    }

    private void Update()
    {
        SpeedRate();
        CalculateMovement();

        if (_isProjectileActive)
        {
            ClosestEnemy();
        }

        ShootLaser();
        PlayerBounds();
        CanShootProjectile();

        if (_isSpreadShotActive)
        {
            _uiManager.DisplayProhibitionSign();
        }
        if (_isTripleShotActive)
        {
            _uiManager.DisplayProhibitionSign();
        }
    }

    private void SpeedRate()
    {
        if (_isSpeedBoostActive == false && _isNegativeSpeedActive == false)
        {
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                _leftShiftPressed = false;
                _uiManager.UpdateBar(false);
            }
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                _leftShiftPressed = true;
                _uiManager.UpdateBar(true);
            }

        }

    }

    private void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 distance = new Vector3(horizontalInput, verticalInput, 0);

        if (_canSpeedUp)
        {
            if (_isSpeedBoostActive)
            {
                transform.Translate(distance * (_speed * _speedMultiplier) * Time.deltaTime, 0);
                _thruster.SetActive(true);
                ManageState(distance.x);
                return;
            }

            if (_leftShiftPressed)
            {
                transform.Translate(distance * (_speed * _speedMultiplier) * Time.deltaTime, 0);
                _thruster.SetActive(true);
                ManageState(distance.x);
                return;
            }
           
        }


        if (_isNegativeSpeedActive)
        {
            transform.Translate(distance * (_speed / _speedMultiplier) * Time.deltaTime, 0);
            _thruster.SetActive(false);
            ManageState(distance.x);
            return;
        }

        transform.Translate(distance * _speed * Time.deltaTime, 0);
        _thruster.SetActive(false);

        ManageState(distance.x);
    }

    private void ClosestEnemy()
    {
        float minDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        if (_spawnManager.BossHasSpawned)
        {
            _closestTarget = _spawnManager.InstantiatedBoss;
        }
        else
        {
            if (_spawnManager.enemies.Count != 0)
            {
                foreach (GameObject enemy in _spawnManager.enemies)
                {
                    if (enemy != null && enemy.transform.position.y > -2)
                    {
                        _canShootProjectile = true;
                        float distance = Vector3.Distance(enemy.transform.position, currentPosition);
                        if (distance < minDistance)
                        {
                            _closestTarget = enemy;
                            minDistance = distance;

                        }
                    }
                    else if (enemy != null && enemy.transform.position.y >= -2)
                    {
                        _canShootProjectile = false;
                    }

                }

            }
        }
    }

    private void ShootLaser()
    {
        
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire) //Can fire equals 0.5s
        {
            _canFire = Time.time + _fireRate;

            if (_ammoCount == 0)
            {
                _uiManager.AmmoTextActive();
                _uiManager.DisplayNoAmmoText();
            }

            else
            {
                if (_isTripleShotActive)
                {
                    //Fire three lasers
                    Instantiate(_trippleShotPrefab, transform.position, Quaternion.identity);
                }
                else if (_isSpreadShotActive)
                {
                    //Fire spread shot (five spread lasers)
                    Instantiate(_spreadShotPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
                }
                else if (_isProjectileActive && _canShootProjectile && _closestTarget != null && _closestTarget.transform.position.y > -1)
                {
                    //Fire Projectile
                    if (_projectileHasBeenShot == false)
                    {
                        projectile = Instantiate(_projectilePrefab, transform.position + new Vector3(0.01f, 1.2f, 0), Quaternion.identity);
                        _isProjectileActive = false;
                        _uiManager.HideProjectile();
                        _projectileHasBeenShot = true;
                        StartCoroutine(ProjectileRoutine());
                    }
                }
                else
                {
                        //Fire just one laser
                        _laser = Instantiate(_laserPrefab, transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
                }

                if (_asterioid.HasDestroyedAsteroid)
                {
                    _ammoCount--;
                    _uiManager.UpdateAmmoText(_ammoCount);

                    if (_ammoCount <= 5)
                    {
                        _uiManager.SetTextColor(Color.red);
                    }
                    else if (_ammoCount <= 10)
                    {
                        _uiManager.SetTextColor(Color.white);
                    }

                    if (_ammoCount % 3 == 0) 
                    {
                        _uiManager.HideBullet(_uiLasersCount);
                        _uiLasersCount++;

                        if (_uiLasersCount > 4)
                        {
                            _uiLasersCount = 0;

                        }
                    }
                }
            }
        }
    }

    private IEnumerator ProjectileRoutine()
    {
        yield return new WaitForSeconds(2.0f);
        _projectileHasBeenShot = false;
    }

    private void PlayerBounds()
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

    private void CanShootProjectile()
    {

        if (_isProjectileActive)
        {

            if (_spawnManager.BossHasSpawned)
            {
                _uiManager.HideProhibitionSign();
                _canShootProjectile = true;
                return;
            }

            if (_spawnManager.EnemiesSpawned == 0 || Enemy.EnemiesEliminated == _spawnManager.EnemiesSpawned || _projectileHasBeenShot == true || _canShootProjectile == false || _ammoCount == 0)
            {

                _uiManager.DisplayProhibitionSign();
                _canShootProjectile = false;
                return;
            }
            if (_spawnManager.EnemiesSpawned > 0 || _projectileHasBeenShot == false || _canShootProjectile == true)
            {
                _uiManager.HideProhibitionSign();
                _canShootProjectile = true;
            }
            if (_closestTarget != null && Enemy.EnemiesEliminated + 1 == _spawnManager.EnemiesSpawned)
            {
               
                if (_closestTarget.transform.position.y <= -1.5)
                {
                    _uiManager.DisplayProhibitionSign();  
                    _canShootProjectile = false;
                }
                else if (_closestTarget.transform.position.y > -1.5)
                {
                    _uiManager.HideProhibitionSign();
                    _canShootProjectile = true;
                }
                
            }
            else if (_closestTarget != null && Enemy.EnemiesEliminated < _spawnManager.EnemiesSpawned)
            {
             
                if (_closestTarget.transform.position.y <= -1)
                {
                    _uiManager.DisplayProhibitionSign();
                    _canShootProjectile = false;
                }
                else if (_closestTarget.transform.position.y > -1)
                {
                    _uiManager.HideProhibitionSign();
                    _canShootProjectile = true;
                }
            }

        }

    }

    private void ManageState(float horizontalInput)
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) == false && Input.GetKeyDown(KeyCode.D) == false)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                _state = State.turningRight;
            }
            
            if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A))
            {
                _state = State.backFromRight;
                
            }
            
        }
       
        if (Input.GetKeyDown(KeyCode.LeftArrow) == false && Input.GetKeyDown(KeyCode.A) == false)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                Debug.Log("Turning Right");
                _state = State.turningLeft;
            }

            if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D))
            {
                _state = State.backFromLeft;
            }
        }

        if(horizontalInput == 0)
        {
            Invoke("BackToBalanced", 0.9f);
        }
        else
        {
            CancelInvoke();
        }

        _anim.SetInteger("state", (int)_state);
    }

    private void BackToBalanced()
    {
        _state = State.balanced;
        _anim.SetInteger("state", (int)_state);
    }

    public void LeftShiftReleased()
    {
        _leftShiftPressed = false;
    }

    public void DontShootProjectile()
    {
        _canShootProjectile = false;
    }

    public void ShootProjectile()
    {
        _canShootProjectile = true;
    }

    public void ProjectileNotShooted()
    {
        _projectileHasBeenShot = false;
    }

    public void Damage()
    {

        if (_isShieldActive)
        {
            ShieldLives();
            return;
        }

        _camera.ShakeCamera();

        _lives--;

        if (_lives == 2)
        {
            _fireBallDamaged[0].SetActive(true);
        }
        else if (_lives == 1)
        {
            _fireBallDamaged[1].SetActive(true);
        }

        if (_lives >= 0)
        {
            _uiManager.UpdateLives(_lives);
        }


        if (_lives < 1)
        {
            _spawnManager.OnPlayerDeath();
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);

            Destroy(this.gameObject, 0.2f);
        }
    }

    private void ShieldLives()
    {
        _shieldLives--;
        
        if (_shieldLives == 2)
        {
            _shieldGameObject.GetComponent<SpriteRenderer>().material = _shieldMaterials[1];
            StartCoroutine(ShieldRoutine());
        }
        else if (_shieldLives == 1)
        {
            _shieldGameObject.GetComponent<SpriteRenderer>().material = _shieldMaterials[0];
        }
        else
        {
            _shieldGameObject.SetActive(false);
            _isShieldActive = false;
        }
        
    }

    IEnumerator ShieldRoutine()
    {
        float seconds1, seconds2;

        while (_isShieldActive && _shieldLives < 3)
        {
            if (_shieldLives == 2)
            {
                seconds1 = 0.1f;
                seconds2 = 1.0f;
            }
            else
            {
                seconds1 = 0.1f;
                seconds2 = 0.5f;
            }

            _shieldGameObject.SetActive(false);
            yield return new WaitForSeconds(seconds1);
            _shieldGameObject.SetActive(true);
            yield return new WaitForSeconds(seconds2);
        }

        if (_isShieldActive == false)
        {
            _shieldGameObject.SetActive(false);
        }
    }

    public GameObject GetTarget()
    {
        return _closestTarget;
    }

    public void ResetShieldLives()
    {
        _shieldLives = 3;
        _shieldGameObject.GetComponent<SpriteRenderer>().material = _shieldMaterials[2];
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isTripleShotActive = false;
    }

    public bool GetTripleShotActive()
    {
        return _isTripleShotActive;
    }

    public void TripleShotNotActive()
    {
        _isTripleShotActive = false;
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

    public void RefillAmmo()
    {
        
        _ammoCount = 15;
        _uiLasersCount = 0;
        _uiManager.SetTextColor(Color.green);
        _uiManager.DisplayBullets();
        _uiManager.UpdateAmmoText(_ammoCount);
        _uiManager.HideNoAmmoText();

    }

    public void AddLive()
    {
        if (_lives < 3)
        {
            _lives++;
            if (_lives == 2)
            {
                _fireBallDamaged[1].SetActive(false);
            }
            else if (_lives == 3)
            {
                _fireBallDamaged[0].SetActive(false);
            }
            _uiManager.UpdateLives(_lives);
        }

    }

    public void SpreadShotActive()
    {
        _isSpreadShotActive = true;
        StartCoroutine(SpreadShotPowerDownRoutine());
    }

    IEnumerator SpreadShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isSpreadShotActive = false;
    }

    public bool GetSpreadShotActive()
    {
        return _isSpreadShotActive;
    }

    public void SpreadShotNotActive()
    {
        _isSpreadShotActive = false;
    }
    
    public void CanSpeedUp(bool canSpeed)
    {
        if (!_isNegativeSpeedActive)
            _canSpeedUp = canSpeed;
    }
    
    public void NegativeSpeedActive()
    {
        _isSpeedBoostActive = false;
        _isNegativeSpeedActive = true;
        _canSpeedUp = false;
        _thruster.SetActive(false);
        _uiManager.ResetBar();
        _uiManager.UpdateThrusterBarColor(true);
        StartCoroutine(NegativeSpeedPowerDownRoutine());
    }

    IEnumerator NegativeSpeedPowerDownRoutine()
    {
        yield return new WaitForSeconds(4.0f);
        _isNegativeSpeedActive = false;
        _uiManager.UpdateThrusterBarColor(false);
    }

    public bool GetNegativeSpeed()
    {
        return _isNegativeSpeedActive;
    }

    public GameObject LaserPosition()
    {
        if (_laser != null)
        {
            return _laser;
        }
        else
        {
            return null;
        }
    }

    public void ProjectileActive()
    {
        _isProjectileActive = true;
        _canShootProjectile = true;
    }

    public void DisplayTargetRange()
    {
        _targetRange.SetActive(true);
    }

    public void HideTargetRange()
    {
        if (_isProjectileActive == false)
        {
            _targetRange.SetActive(false);
        }
    }

    public void ProjectileDestroyed()
    {
        _projectileHasBeenShot = false;
    }

    public int PlayerLives()
    {
        return _lives;
    }
}
