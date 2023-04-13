using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _enemiesPrefab; //0 = Default, 1 = Laser Beam, 2 = Fire Twice, 3 = Shoot Backward, 4 = Avoid Shot
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject[] _powerups; //0 = Triple Shot, 1 = Speed Positive, 2 = Shield, 3 = Speed Negative, 4 = Life, 5 = Ammo Refill, 6 = Spreead Shot, 7 = Projectile
    [SerializeField]
    private GameObject _boss;

    private Vector3 _posToSpawnEnemy;
    private Vector3 _posToSpawnPowerup;
    private bool _stopSpawningEnemy = false;
    private bool _stopSpawningPowerup = false;
    private UIManager _uIManager;
    private GameManager _gameManager;
    private GameObject _enemy;
    private GameObject _spawnedPowerup;
    public GameObject InstantiatedBoss { get; private set; }

    [HideInInspector]
    public GameObject EnemySpawned;
    [HideInInspector]
    public List<int> wavesEnemies = new List<int>();
    [HideInInspector]
    public List<GameObject> enemies;

    public static int WavesCount { get; private set; } 
    public int CurrentWave { get; private set; }
    public int EnemiesSpawned { get; private set; }
    public static int EnemyID { get; private set; }
    public bool BossHasSpawned { get; private set; }

    enum EnemyType
    {
        Default,
        LaserBeam,
        FireTwice,
        ShootBackward,
        AvoidShot
    }

    private EnemyType[] _enemiesType = new EnemyType[5];

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

        WavesCount = 8;

        for (int i = 1; i <= WavesCount; i++)
        {
            wavesEnemies.Add(i * 2);
        }

        CurrentWave = 0;
        BossHasSpawned = false;

        _enemiesType[0] = EnemyType.Default;
        _enemiesType[1] = EnemyType.LaserBeam;
        _enemiesType[2] = EnemyType.FireTwice;
        _enemiesType[3] = EnemyType.ShootBackward;
        _enemiesType[4] = EnemyType.AvoidShot;
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
                EnemySpawned = Instantiate(_enemy, _posToSpawnEnemy, Quaternion.identity);
                EnemySpawned.transform.parent = _enemyContainer.transform;
                enemies.Add(EnemySpawned);
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
            case EnemyType.AvoidShot:
                _enemy = _enemiesPrefab[4];
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

        if (percent <= 40)
        {
            enemyType = EnemyType.Default;
        }
        else if (percent > 40 && percent <= 55)
        {
            enemyType = EnemyType.LaserBeam;
        }
        else if (percent > 55 && percent <= 70)
        {
            enemyType = EnemyType.ShootBackward;
        }
        else if (percent > 70 && percent <= 80)
        {
            enemyType = EnemyType.FireTwice;
        }
        else
        {
            enemyType = EnemyType.AvoidShot;
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
            randomPowerup = Random.Range(6, 8);
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
        enemies.Clear();
    }

    public void CompletedWave()
    {
        _stopSpawningEnemy = true;
        EnemiesSpawned = 0;
        CurrentWave++;

        _uIManager.HideWavesCount();
        _uIManager.ResetWaveCountTextPosition();
        _uIManager.DisplayWavesCount();

        if (_gameManager.IsGameOver)
        {
            EnemiesSpawned = 0;
        }
        else
        {
            if (CurrentWave >= WavesCount)
            {
                InstantiatedBoss = Instantiate(_boss, new Vector3(0, 13, 0), Quaternion.identity);
                _uIManager.DisplayBossLives();
                _stopSpawningEnemy = true;
                BossHasSpawned = true;
                GameObject.Find("Player").GetComponent<Player>().ShootProjectile();
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
        Transform enemy;
        Vector3 powerupPosition;


        if (_spawnedPowerup != null && EnemySpawned != null)
        {
            PowerUp powerUp = _spawnedPowerup.GetComponent<PowerUp>();
            EnemyShootBackwards enemyShootBackwards = EnemySpawned.GetComponent<EnemyShootBackwards>();
            Enemy enemyScript = EnemySpawned.GetComponent<Enemy>();
            AvoidShotEnemy avoidShotEnemy = EnemySpawned.GetComponent<AvoidShotEnemy>();

            if (enemyShootBackwards != null)
            {
                enemy = enemyShootBackwards.GetTransform();
                powerupPosition = powerUp.GetPosition();


                if (powerupPosition.x > enemy.position.x - 0.8f && powerupPosition.x < enemy.position.x + 0.8f && powerupPosition.y < enemy.position.y - 2.0f)
                {

                    enemyShootBackwards.Shoot();
                    enemyShootBackwards.ShootPowerUp();

                }
            }

            if (enemyScript != null)
            {
                enemy = enemyScript.GetTransform();
                powerupPosition = powerUp.GetPosition();


                if (powerupPosition.x > enemy.position.x - 0.8f && powerupPosition.x < enemy.position.x + 0.8f && powerupPosition.y < enemy.position.y - 2.0f)
                {

                    enemyScript.Shoot();
                    enemyScript.ShootPowerUp();

                }
            }

            if (avoidShotEnemy != null)
            {
                enemy = avoidShotEnemy.GetTransform();
                powerupPosition = powerUp.GetPosition();


                if (powerupPosition.x > enemy.position.x - 0.8f && powerupPosition.x < enemy.position.x + 0.8f && powerupPosition.y < enemy.position.y - 2.0f)
                {

                    avoidShotEnemy.Shoot();
                    avoidShotEnemy.ShootPowerUp();

                }
            }

           
        }

    }


}
