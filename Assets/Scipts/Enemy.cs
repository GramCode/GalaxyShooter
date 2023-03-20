using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    [SerializeField] private float _speed = 3.0f;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private AudioClip _laserAudioClip;
    [SerializeField] private GameObject _laserBeamPrefab;
    [SerializeField] private GameObject _explosion;

    private AudioSource _audioSource;
    private Player _player;
    private Collider2D _collider2D;
    private Animator _anim;
    private float _fireRate = 3.0f;
    private float _canShoot = -1;
    private bool _isDestroyed = false;
    private SpawnManager _spawnManger;
    private GameManager _gameManager;

    public static int EnemiesEliminated { get; set; }

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

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("AudioSource on Enemy is NULL.");
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

    }



    void Update()
    {

        EnemyBehavior();

        if (!_isDestroyed)
        {
            FireLaser();
        }

        CheckForNextWave();
        
    }

    void EnemyBehavior()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -5.6f)
        {
            float randomX = Random.Range(-9, 9);
            transform.position = new Vector3(randomX, 7.5f, 0);
        }

    }

    public void FireLaser()
    {
        if (Time.time > _canShoot)
        {
            float positionToInstatiate;
            _fireRate = Random.Range(3f, 7f);
            _canShoot = Time.time + _fireRate;

            positionToInstatiate = transform.position.y - 0.6f;
            GameObject enemyLaser = Instantiate(_laserPrefab, new Vector3(transform.position.x, positionToInstatiate, 0), Quaternion.identity);
            Laser lasers = enemyLaser.GetComponent<Laser>();
            lasers.AssignEnemyLaser();
        }
    }

    private void CheckForNextWave()
    {
        if (!_gameManager.GameCompleted)
        {
            if (EnemiesEliminated == _spawnManger.wavesEnemies[_spawnManger.CurrentWave])
            {
                _spawnManger.CompletedWave();
                EnemiesEliminated = 0;

                if (SpawnManager.WavesCount == _spawnManger.CurrentWave)
                {
                    _gameManager.CompletedGame();
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
            EnemiesEliminated++;
            _audioSource.Play();
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
            EnemiesEliminated++;
            _audioSource.Play();
            Destroy(_collider2D);
            Destroy(this.gameObject, 2.8f);
            Destroy(other.gameObject);
        }

    }

    public void ResetEliminatedEnemies()
    {
        EnemiesEliminated = 0;
    }

}
