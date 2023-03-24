using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDiamondMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 3.5f;
    [SerializeField] private GameObject _explosion;
    [SerializeField] private GameObject _laserPrefab;

    private GameObject[] _waypoints = new GameObject[4];
    private GameObject _container;
    private bool _isWaypointDestroyed = false;
    private bool _isCiclying = false;
    private bool _completedCicle = false;
    private int _waypointIndex;
    private int _waypointIndexNegative = 3;
    private int _waypointsCount;
    private Player _player;
    private SpawnManager _spawnManger;
    private GameManager _gameManager;
    private Collider2D _collider2D;
    private int randomChoice;
    private float _fireRate = 3.0f;
    private float _canShoot = -1;
    private bool _isShieldActive = true;

    void Start()
    {
        _player = GameObject.Find("Player").transform.GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("The Player was not found inside the EnemyMove Script");
        }

        _spawnManger = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        if (_spawnManger == null)
        {
            Debug.LogError("Spawn Manager in EnemyMove is NULL");
        }

        _gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("Game Manager in EnemyMove is NULL");
        }

        _collider2D = GetComponent<Collider2D>();
        if (_collider2D == null)
        {
            Debug.LogError("EnemyMovement collider is NULL");
        }

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

        foreach (var waypoint in _waypoints)
        {
            waypoint.transform.parent = _container.transform;
        }

        _isWaypointDestroyed = false;
        randomChoice  = Random.Range(0, 2);

    }

    void Update()
    {
        EnemyBehavior();
        FireLaser();
                
    }

    void EnemyBehavior()
    {
        if (transform.position.y > 4.0f)
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }
        else
        {
            if (_completedCicle || transform.position.x < -6.0 && _isCiclying == false || transform.position.x > 6.0 && _isCiclying == false)
            {
                transform.Translate(Vector3.down * _speed * Time.deltaTime);
            }
            else
            {
                if (randomChoice < 1)
                {
                    MoveLeft();
                }
                else
                {
                    MoveRight();
                }
               
            }

        }


        if (transform.position.y < -5.6f)
        {
            float randomX = Random.Range(-9, 9);
            transform.position = new Vector3(randomX, 7.5f, 0);
            WaypointsX(randomX);
            _completedCicle = false;
            _isCiclying = false;
            randomChoice = Random.Range(0, 2);
            _collider2D.enabled = true;
        }

    }


    private void MoveLeft()
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
            _isCiclying = true;
        }

    }

    private void MoveRight()
    {
        if (!_isWaypointDestroyed)
        {
            if (Vector2.Distance(transform.position, _waypoints[_waypointIndexNegative].transform.position) < 0.1f)
            {
                if (_waypointIndexNegative <= 0)
                {
                    _waypointsCount++;
                }
                else
                {
                    _waypointsCount++;
                    _waypointIndexNegative--;
                }
                if (_waypointsCount == 4)
                {
                    _completedCicle = true;
                    _waypointsCount = 0;
                    _waypointIndexNegative = 3;
                }
                
            }

            var currentWaypoint = _waypoints[_waypointIndexNegative];
            transform.position = Vector2.MoveTowards(transform.position, currentWaypoint.transform.position, _speed * Time.deltaTime);
            _isCiclying = true;
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

    public void FireLaser()
    {
        if (Time.time > _canShoot)
        {
            _fireRate = Random.Range(3f, 7f);
            _canShoot = Time.time + _fireRate;

            float positionToInstantiateY = transform.position.y - 1.04f;
            float positionToInstantiateX = transform.position.x + 0.01f;
            GameObject enemyLaser = Instantiate(_laserPrefab, new Vector3(positionToInstantiateX, positionToInstantiateY, 0), Quaternion.identity);
            Laser lasers = enemyLaser.GetComponent<Laser>();
            lasers.AssignEnemyLaser();
            StartCoroutine(FireSecondLaserRoutine());
        }
    }

    IEnumerator FireSecondLaserRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        float positionToInstatiate = transform.position.y - 0.6f;
        GameObject enemyLaser = Instantiate(_laserPrefab, new Vector3(transform.position.x, positionToInstatiate, 0), Quaternion.identity);
        Laser lasers = enemyLaser.GetComponent<Laser>();
        lasers.AssignEnemyLaser();
    }

    private void DestroyWaypointsGameObjects()
    {
        foreach (var waypoint in _waypoints)
        {
            Destroy(waypoint);
        }
        _isWaypointDestroyed = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (_isShieldActive)
            {
                if (_player != null)
                    _player.Damage();
                HideShield();
                _collider2D.enabled = false;
                return;
            }

            if (_player != null)
                _player.Damage();

            _speed = 0;
            Enemy.EnemiesEliminated++;
            CheckForNextWave();
            Instantiate(_explosion, transform.position, Quaternion.identity);
            DestroyWaypointsGameObjects();
            Destroy(_collider2D);
            Destroy(this.gameObject, 0.2f);
            
            
        }

        if (other.CompareTag("Laser"))
        {

            if (_isShieldActive)
            {
                HideShield();
                Destroy(other.gameObject);
                return;
            }
            
            if (_player != null)
                _player.AddScore(10);

            _speed = 0;
            Enemy.EnemiesEliminated++;
            CheckForNextWave();
            Instantiate(_explosion, transform.position, Quaternion.identity);
            DestroyWaypointsGameObjects();
            Destroy(_collider2D);
            Destroy(this.gameObject, 0.2f);
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

    private void HideShield()
    {
        _isShieldActive = false;
        GameObject.FindGameObjectWithTag("EnemyShield").SetActive(false);
    }
}
