using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 3.0f;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private AudioClip _laserAudioClip;

    private AudioSource _audioSource;
    private Player _player;
    private Collider2D _collider2D;
    private Animator _anim;
    private float _fireRate = 3.0f;
    private float _canShoot = -1;
    private bool _isDestroyed = false;

    private bool _completedCicle = false;
    private int _waypointIndex;
    private int _waypointsCount;
    static private int _enemiesEliminated;

    private GameObject[] _waypoints = new GameObject[4];
    private GameObject _container;
    private SpawnManager _spawnManger;
    private GameManager _gameManager;
    private Waves _waves;
    private bool _isWaypointDestroyed = false;

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

        _waves = GameObject.Find("Waves").GetComponent<Waves>();
        if (_waves == null)
        {
            Debug.LogError("Waves in Enemy is NULL");
        }

        //_container = new GameObject("Container");
        _container = GameObject.Find("Waypoints Container");
        _waypoints[0] = new GameObject("waypoint 1");
        _waypoints[1] = new GameObject("waypoint 2");
        _waypoints[2] = new GameObject("waypoint 3");
        _waypoints[3] = new GameObject("waypoint 4");

        float enemyValueX = transform.position.x;

        _waypoints[0].transform.position = new Vector3(enemyValueX, 4.0f, 0);
        _waypoints[1].transform.position = new Vector3(enemyValueX - 2.0f, 2.0f, 0);
        _waypoints[2].transform.position = new Vector3(enemyValueX, 0, 0);
        _waypoints[3].transform.position = new Vector3(enemyValueX + 2.0f, 2.0f, 0);

        foreach(var waypoint in _waypoints)
        {
            waypoint.transform.parent = _container.transform;
        }

        _isWaypointDestroyed = false;
    }

    void Update()
    {
        EnemyBehavior();

        if (!_isDestroyed)
        {
            FireLaser();
        }
        if (!_gameManager.HaveCompletedGame())
        {
            if (_enemiesEliminated == _waves.waves[_spawnManger.GetCurrentWave()].GetEnemiesInWave())
            {
                _spawnManger.CompletedWave();
                _enemiesEliminated = 0;

                if (_waves.waves.Count == _spawnManger.GetCurrentWave())
                {
                    _gameManager.CompletedGame();
                }
            }
        }
        
    }

    void EnemyBehavior()
    {
        if (transform.position.y > 4.0f)
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }
        else
        {
            if (_completedCicle)
            {
                transform.Translate(Vector3.down * _speed * Time.deltaTime);
            }
            else
            {
                NewMovement();
            }

        }


        if (transform.position.y < -5.6f)
        {
            float randomX = Random.Range(-9, 9);
            transform.position = new Vector3(randomX, 7.5f, 0);
            WaypointsX(randomX);
            _completedCicle = false;
        }

    }

    private void NewMovement()
    {
        if (!_isWaypointDestroyed)
        {
            if (Vector2.Distance(transform.position, _waypoints[_waypointIndex].transform.position) < 0.1f)
            {
                if (_waypointIndex >= 3)
                {
                    _waypointIndex = 0; ;
                }
                else
                {
                    _waypointsCount++;
                    _waypointIndex++;
                }

                if (_waypointsCount == 4)
                {
                    _completedCicle = true;
                    _waypointsCount = 0;
                }

            }

            var currentWaypoint = _waypoints[_waypointIndex];
            transform.position = Vector2.MoveTowards(transform.position, currentWaypoint.transform.position, _speed * Time.deltaTime);

        }

    }


    private void FireLaser()
    {
        if (Time.time > _canShoot)
        {
            _fireRate = Random.Range(3f, 7f);
            _canShoot = Time.time + _fireRate;
            float positionToInstatiate = transform.position.y - 0.6f;
            GameObject enemyLaser = Instantiate(_laserPrefab, new Vector3(transform.position.x, positionToInstatiate, 0), Quaternion.identity);
            Laser lasers = enemyLaser.GetComponent<Laser>();
            lasers.AssignEnemyLaser();
        }
    }

    private void WaypointsX(float randX)
    {
        _waypointIndex = 0;
        _waypointsCount = 0;
        _waypoints[0].transform.position = new Vector3(randX, 4.0f, 0);
        _waypoints[1].transform.position = new Vector3(randX - 2.0f, 2.0f, 0);
        _waypoints[2].transform.position = new Vector3(randX, 0, 0);
        _waypoints[3].transform.position = new Vector3(randX + 2.0f, 2.0f, 0);
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
            _enemiesEliminated++;
            DestroyWaypointsGameObjects();
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
            _enemiesEliminated++;
            DestroyWaypointsGameObjects();
            _audioSource.Play();
            Destroy(_collider2D);
            Destroy(this.gameObject, 2.8f);
            Destroy(other.gameObject);
        }


    }

    private void DestroyWaypointsGameObjects()
    {
        foreach (var waypoint in _waypoints)
        {
            Destroy(waypoint);
        }
        _isWaypointDestroyed = true;
    }

    public int EliminatedEnemies()
    {
        return _enemiesEliminated;
    }
    
}
