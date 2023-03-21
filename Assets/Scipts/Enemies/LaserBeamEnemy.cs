using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeamEnemy : MonoBehaviour
{
    [SerializeField] private float _speed = 3.0f;
    [SerializeField] private GameObject _laserBeamPrefab;
    [SerializeField] private GameObject _explosion;
    [SerializeField] private GameObject _blast;
    
    private Collider2D _collider2D;
    private float _fireRate = 3.0f;
    private float _canShoot = -1;
    private bool _hasMovedHorizontally = false;
    private Vector3[] vectors = new Vector3[2];
    private Vector3 randomVector;
    private Vector2 randomVector2;
    private Player _player;
    private SpawnManager _spawnManger;
    private GameManager _gameManager;
    private bool _isDestroyed = false;

    private void Start()
    {
        _collider2D = GetComponent<Collider2D>();
        if (_collider2D == null)
        {
            Debug.LogError("Enemy collider is NULL");
        }

        _player = GameObject.Find("Player").transform.GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("The Player was not found inside the Enemy Script");
        }

        _spawnManger = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        if (_spawnManger == null)
        {
            Debug.LogError("Spawn Manager in Enemy is NULL");
        }

        _gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("Game Manager in EnemyMove is NULL");
        }

        vectors[0] = Vector3.left;
        vectors[1] = Vector3.right;
        randomVector = vectors[Random.Range(0, 2)];
        randomVector2 = new Vector2(Random.Range(-8, 8), 3.5f);
        
    }

    private void Update()
    {
        EnemyBehavior();
        EnemyBounds();
        
       
        
    }

    void EnemyBehavior()
    {
        if (transform.position.y > 3.5f)
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }
        else
        {
            if (_hasMovedHorizontally)
            {
                transform.Translate(Vector3.down * _speed * Time.deltaTime);
            }
            else
            {
                if (Vector2.Distance(transform.position, randomVector2) > 0.1)
                {
                    transform.Translate(randomVector * _speed * Time.deltaTime);
                }
                else
                {
                    ShootLaserBeam();
                }
                
            }
           
        }

        if (transform.position.y < -5.6f)
        {
            float randomX = Random.Range(-9, 9);
            transform.position = new Vector3(randomX, 7.5f, 0);
            _hasMovedHorizontally = false;
            randomVector = vectors[Random.Range(0, 2)];
            randomVector2 = new Vector2(Random.Range(-8, 8), 3.5f);
        }

    }

    IEnumerator ShootLaserRoutine()
    {
        yield return new WaitForSeconds(2.0f);
        _hasMovedHorizontally = true;

    }

    private void ShootLaserBeam()
    {
        if (Time.time > _canShoot)
        {
            _fireRate = Random.Range(3f, 7f);
            _canShoot = Time.time + _fireRate;

            GameObject blast = Instantiate(_blast, transform.position + new Vector3(0.06f, -0.3f, 0), Quaternion.identity);
            StartCoroutine(ScaleBlastRoutine(blast));
        }
        
    }

    IEnumerator ScaleBlastRoutine(GameObject blast)
    {
        float value = 0;
        bool exitWhileLoop = false;
        while (value < 0.91 && exitWhileLoop == false && Time.timeScale != 0)
        {

            value += 0.005f;
            blast.transform.localScale = new Vector3(value, value, value);
            if (_isDestroyed)
            {
                Destroy(blast);
                exitWhileLoop = true;
            }
            yield return new WaitForEndOfFrame();
        }
        float positionToInstatiate;
        Destroy(blast, 0.7f);
        positionToInstatiate = transform.position.y - 0.3f;
        Instantiate(_laserBeamPrefab, new Vector3(transform.position.x, positionToInstatiate, 0), Quaternion.identity);
        StartCoroutine(ShootLaserRoutine());

    }

    private void EnemyBounds()
    {
        if (transform.position.x > 11.3f)
        {
            transform.position = new Vector3(-11.3f, transform.position.y);
        }
        else if (transform.position.x < -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {

            if (_player != null)
                _player.Damage();
            _speed = 0;
            Enemy.EnemiesEliminated++;
            CheckForNextWave();
            Instantiate(_explosion, transform.position, Quaternion.identity);
            _isDestroyed = true;
            Destroy(_collider2D);
            Destroy(this.gameObject, 0.2f);
        }

        if (other.CompareTag("Laser"))
        {
            if (_player != null)
                _player.AddScore(10);

            _speed = 0;
            Enemy.EnemiesEliminated++;
            CheckForNextWave();
            Instantiate(_explosion, transform.position, Quaternion.identity);
            _isDestroyed = true;
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
}
