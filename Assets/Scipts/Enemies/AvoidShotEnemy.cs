using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidShotEnemy : MonoBehaviour
{
    [SerializeField] private float _speed = 3.5f;
    [SerializeField] private GameObject _alienTwoLaserPrefab;
    [SerializeField] private GameObject _explosion;

    private Player _player;
    private Collider2D _collider2D;
    private Animator _anim;
    private float _fireRate = 3.0f;
    private float _canShoot = -1;
    private float _fireToPowerupRate = 4.0f;
    private float _canShootToPowerup = -1;
    private bool _isDestroyed = false;
    private bool _canShootPowerup = false;
    private SpawnManager _spawnManger;
    private GameManager _gameManager;
    private float _dir;

    private void Start()
    {
        _player = GameObject.Find("Player").transform.GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("The Player was not found inside the Enemy Script");
        }

        _anim = GetComponent<Animator>();
        if (_anim == null)
        {
            Debug.LogError("Animator component of Enemy not found.");
        }

        _collider2D = GetComponent<Collider2D>();
        if (_collider2D == null)
        {
            Debug.LogError("Enemy collider is NULL");
        }

        _spawnManger = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        if (_spawnManger == null)
        {
            Debug.LogError("Spawn Manager in Enemy is NULL");
        }

        _gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("Game Manager in Enemy is NULL");
        }

        _dir = Random.Range(0, 2);

    }



    void Update()
    {

        EnemyBehavior();

        if (!_isDestroyed)
        {
            FireLaser();
            AvoidLaser();
        }


    }

    void EnemyBehavior()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -5.6f)
        {
            float randomX = Random.Range(-8, 8);
            transform.position = new Vector3(randomX, 7.5f, 0);
            _dir = Random.Range(0, 2);
        }

    }

    private void FireLaser()
    {
        if (Time.time > _canShoot)
        {
            _fireRate = Random.Range(3f, 7f);
            _canShoot = Time.time + _fireRate;

            float positionToInstantiateY = transform.position.y - 0.7f;
            float positionToInstantiateX = transform.position.x + 0.065f;
            GameObject enemyLaser = Instantiate(_alienTwoLaserPrefab, new Vector3(positionToInstantiateX, positionToInstantiateY, 0), Quaternion.identity);
            Laser lasers = enemyLaser.GetComponent<Laser>();
            lasers.AssignEnemyLaser();
            _canShootPowerup = false;
        }
    }

    private void AvoidLaser()
    {
        if (_player.LaserPosition() != null)
        {
            if (_player.LaserPosition().transform.position.x < transform.position.x + 1f && _player.LaserPosition().transform.position.x > transform.position.x - 1f)
            {
                if (_dir == 0)
                {
                    transform.Translate(Vector3.left * (_speed * 1.5f) * Time.deltaTime);
                }
                else
                {
                    transform.Translate(Vector3.right * (_speed * 1.5f) * Time.deltaTime);
                }
            }

        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {

            if (_player != null)
                _player.Damage();
            _anim.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _isDestroyed = true;
            Enemy.EnemiesEliminated++;
            CheckForNextWave();
            Instantiate(_explosion, transform.position, Quaternion.identity);
            Destroy(_collider2D);
            Destroy(this.gameObject, 2.8f);
        }

        if (other.CompareTag("Laser"))
        {

            if (_player != null)
                _player.AddScore(10);

            _anim.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _isDestroyed = true;
            Enemy.EnemiesEliminated++;
            CheckForNextWave();
            Instantiate(_explosion, transform.position, Quaternion.identity);
            Destroy(_collider2D);
            Destroy(this.gameObject, 2.8f);
            Destroy(other.gameObject);
        }

    }

    private void CheckForNextWave()
    {
        if (!_gameManager.GameCompleted)
        {
            if (Enemy.EnemiesEliminated == _spawnManger.wavesEnemies[_spawnManger.CurrentWave])
            {
                _spawnManger.CompletedWave();
                Enemy.EnemiesEliminated = 0;

                if (SpawnManager.WavesCount == _spawnManger.CurrentWave)
                {
                    _gameManager.CompletedGame();
                }
            }
        }
    }

    public void ResetEliminatedEnemies()
    {
        Enemy.EnemiesEliminated = 0;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void ShootPowerUp()
    {
        if (_canShootPowerup && Time.time > _canShootToPowerup)
        {
            _canShootToPowerup = Time.time + _fireToPowerupRate;
            float positionToInstantiateY = transform.position.y - 0.7f;
            float positionToInstantiateX = transform.position.x + 0.065f;
            GameObject enemyLaser = Instantiate(_alienTwoLaserPrefab, new Vector3(positionToInstantiateX, positionToInstantiateY, 0), Quaternion.identity);
            Laser lasers = enemyLaser.GetComponent<Laser>();
            lasers.AssignEnemyLaser();
            _canShootPowerup = false;
        }
    }

    public void Shoot()
    {
        _canShootPowerup = true;
    }
}
