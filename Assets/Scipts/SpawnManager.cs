using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject[] _powerups;

    private Vector3 _posToSpawnEnemy;
    private Vector3 _posToSpawnPowerup;
    private bool _stopSpawningEnemy = false;
    private bool _stopSpawningPowerup = false;
    private Waves _wavesObj;
    private UIManager _uIManager;

    public static int CurrentWave { get; private set; }
    public static int EnemiesSpawned { get; private set; }

    void Start()
    {
        _posToSpawnEnemy = new Vector3(Random.Range(-8f, 8f), 7, 0);
        _posToSpawnPowerup = new Vector3(Random.Range(-8f, 8f), 7, 0);

        _wavesObj = GameObject.Find("Waves").GetComponent<Waves>();
        if(_wavesObj == null)
        {
            Debug.LogError("Waves in SpawnManager is NULL");
        }

        _uIManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if(_uIManager == null)
        {
            Debug.LogError("UI Manager in Spawn Manager is NULL");
        }

        CurrentWave = 0;
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
            _posToSpawnEnemy = new Vector3(Random.Range(-8f, 8f), 7, 0);
            _posToSpawnPowerup = new Vector3(Random.Range(-8f, 8f), 7, 0);
            yield return new WaitForSeconds(2.8f);
        }
    }

    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        while (_stopSpawningEnemy == false)
        {
            if (EnemiesSpawned < _wavesObj.waves[CurrentWave].GetEnemiesInWave())
            {
                EnemiesSpawned++;
                GameObject newEnemy = Instantiate(_enemyPrefab, _posToSpawnEnemy, Quaternion.identity);
                newEnemy.transform.parent = _enemyContainer.transform;
                
            }
            else
            {
                _stopSpawningEnemy = true;                
            }

            yield return new WaitForSeconds(5.0f);
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        while (_stopSpawningPowerup == false)
        {
            Instantiate(_powerups[GetPowerupIndex()], _posToSpawnPowerup, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(3, 8));
        }
        
    }


    private int GetPowerupIndex()
    {
        int number = Random.Range(0, 20);
        int randomPowerup;

        if (number < 19)
        {
            randomPowerup = Random.Range(0, 6);
        }
        else
        {
            randomPowerup = 6;
        }

        return randomPowerup;
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

        if (CurrentWave >= _wavesObj.waves.Count)
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

    IEnumerator WaveCoolDownRoutine()
    {
        _uIManager.UpdateAndDisplayWaveText(CurrentWave + 1);
        yield return new WaitForSeconds(2.5f);
        _stopSpawningEnemy = false;
        StartCoroutine(SpawnEnemyRoutine());
    }

    public void GameOver()
    {
        _stopSpawningEnemy = true;
        _stopSpawningPowerup = true;
    }
}
