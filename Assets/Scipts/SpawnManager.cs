using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _enemiesPrefab; //Default, Blaster, Follower, Shoot Backward
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject[] _powerups;
    
    private Vector3 _posToSpawnEnemy;
    private Vector3 _posToSpawnPowerup;
    private bool _stopSpawningEnemy = false;
    private bool _stopSpawningPowerup = false;
    private UIManager _uIManager;
    private GameManager _gameManager;
    private GameObject _enemy;
    private GameObject _spawnedPowerup;
    private GameObject _enemySpawned;
    private EnemyShootBackwards _enemyShootBackwards;

    [HideInInspector]
    public List<int> wavesEnemies = new List<int>();

    public static int WavesCount { get; private set; } 
    public int CurrentWave { get; private set; }
    public int EnemiesSpawned { get; private set; }
    public static int EnemyID { get; private set; }

    enum EnemyType
    {
        Default,
        LaserBeam,
        FireTwice,
        ShootBackward
    }

    private EnemyType[] _enemiesType = new EnemyType[4];

    void Start()
    {
        _posToSpawnEnemy = new Vector3(Random.Range(-8f, 8f), 7, 0);
        _posToSpawnPowerup = new Vector3(Random.Range(-8f, 8f), 7, 0);

        _uIManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uIManager == null)
        {
            Debug.LogError("UI Manager in Spawn Manager is NULL");
        }

        _gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("GameManager in SpawnManager is NULL");
        }

        _enemyShootBackwards = _enemiesPrefab[3].GetComponent<EnemyShootBackwards>();
        if(_enemyShootBackwards == null)
        {
            Debug.LogError("EnemyShootBackwards in Spawn Manager is NULL");
        }


        WavesCount = 8;

        for (int i = 1; i <= WavesCount; i++)
        {
            wavesEnemies.Add(i * 2);
        }

        CurrentWave = 0;

        _enemiesType[0] = EnemyType.Default;
        _enemiesType[1] = EnemyType.LaserBeam;
        _enemiesType[2] = EnemyType.FireTwice;
        _enemiesType[3] = EnemyType.ShootBackward;
    }

    private void Update()
    {
        GetPosition();
    }

    public void StartSpawning()
    {
        StartCoroutine(RandomPositionRoutine());
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    IEnumerator RandomPositionRoutine()
    {
        while (true)
        {
            _posToSpawnEnemy = new Vector3(Random.Range(-8.0f, 8.0f), 7, 0);
            _posToSpawnPowerup = new Vector3(Random.Range(-8.0f, 8.0f), 7, 0);
            yield return new WaitForSeconds(2.8f);
        }
    }

    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        while (_stopSpawningEnemy == false)
        {
            if (EnemiesSpawned < wavesEnemies[CurrentWave]) 
            {
                TypeOfEnemy(GetEnemyType());
                EnemiesSpawned++;
                _enemySpawned = Instantiate(_enemy, _posToSpawnEnemy, Quaternion.identity);
                _enemySpawned.transform.parent = _enemyContainer.transform;                
            }
            else
            {
                _stopSpawningEnemy = true;
            }

            
            yield return new WaitForSeconds(5.0f);
        }
    }

    private void TypeOfEnemy(EnemyType enemyType)
    {
        switch (enemyType)
        {
            case EnemyType.Default:
                //Shoot normal laser
                _enemy = _enemiesPrefab[0];
                break;
            case EnemyType.LaserBeam:
                //Shoot LaserBeam
                _enemy = _enemiesPrefab[1];
                break;
            case EnemyType.FireTwice:
                //Fire Twice
                _enemy = _enemiesPrefab[2];
                break;
            case EnemyType.ShootBackward:
                _enemy = _enemiesPrefab[3];
                break;
            default:
                _enemy = _enemiesPrefab[0];
                break;
        }
    }

    private EnemyType GetEnemyType()
    {
        int percent = Random.Range(0, 101);
        EnemyType enemyType;

        if (percent <= 50)
        {
            enemyType = EnemyType.Default;
        }
        else if (percent > 50 && percent <= 70)
        {
            enemyType = EnemyType.LaserBeam;
        }
        else if (percent > 70 && percent <= 90)
        {
            enemyType = EnemyType.ShootBackward;
        }
        else
        {
            enemyType = EnemyType.FireTwice;
        }
        return enemyType;
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        while (_stopSpawningPowerup == false)
        {
            _spawnedPowerup = Instantiate(_powerups[GetPowerupIndex()], _posToSpawnPowerup, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(3, 8));
        }
        
    }


    private int GetPowerupIndex()
    {

        int number = Random.Range(0, 100);

        int randomPowerup;

        if (number <= 40)
        {
            randomPowerup = 5;
        }
        else if (number > 40 && number <= 70)
        {
            randomPowerup = Random.Range(0, 5);
        }
        else if (number > 70 && number <= 90)
        {
            randomPowerup = 6;
        }
        else
        {
            randomPowerup = 4;
        }
        return randomPowerup;
    }

    public Vector3 PowerupPosition()
    {
        return _spawnedPowerup.transform.position;
    }

    public void OnPlayerDeath()
    {
        _stopSpawningEnemy = true;
        _stopSpawningPowerup = true;
    }

    public void CompletedWave()
    {
        _stopSpawningEnemy = true;
        EnemiesSpawned = 0;
        CurrentWave++;

        if (_gameManager.IsGameOver)
        {
            EnemiesSpawned = 0;
        }
        else
        {
            if (CurrentWave >= WavesCount)
            {
                _uIManager.AllWavesCompleted();
                _stopSpawningEnemy = true;
                _stopSpawningPowerup = true;
            }
            else
            {
                StartCoroutine(WaveCoolDownRoutine());
            }
        }
        
    }

    IEnumerator WaveCoolDownRoutine()
    {
        _uIManager.UpdateAndDisplayWaveText(CurrentWave + 1);
        yield return new WaitForSeconds(2.5f);
        _stopSpawningEnemy = false;
        StartCoroutine(SpawnEnemyRoutine());
    }

    public void StopSpawning()
    {
        _stopSpawningEnemy = true;
        _stopSpawningPowerup = true;
    }

    private void GetPosition()
    {
        Vector3 enemyPosition;
        Vector3 powerupPosition;


        if (_spawnedPowerup != null && _enemySpawned != null)
        {
            PowerUp powerUp = _spawnedPowerup.GetComponent<PowerUp>();
            EnemyShootBackwards enemyShootBackwards = _enemySpawned.GetComponent<EnemyShootBackwards>();
            Enemy enemy = _enemySpawned.GetComponent<Enemy>();

            if (enemyShootBackwards != null)
            {
                enemyPosition = enemyShootBackwards.GetPosition();
                powerupPosition = powerUp.GetPosition();


                if (powerupPosition.x > enemyPosition.x - 0.8f && powerupPosition.x < enemyPosition.x + 0.8f && powerupPosition.y < enemyPosition.y - 2.0f)
                {

                    enemyShootBackwards.Shoot();
                    enemyShootBackwards.ShootPowerUp();

                }
            }

            if (enemy != null)
            {
                enemyPosition = enemy.GetPosition();
                powerupPosition = powerUp.GetPosition();


                if (powerupPosition.x > enemyPosition.x - 0.8f && powerupPosition.x < enemyPosition.x + 0.8f && powerupPosition.y < enemyPosition.y - 2.0f)
                {

                    enemy.Shoot();
                    enemy.ShootPowerUp();

                }
            }
            

        }

    }



}
