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
    private int _enemiesSpawned;
    private int _currentWave;
    private Waves _wavesObj;
    private UIManager _uIManager;

    void Start()
    {
        _posToSpawnEnemy = new Vector3(Random.Range(-8f, 8f), 7, 0);
        _posToSpawnPowerup = new Vector3(Random.Range(-8f, 8f), 7, 0);
        _wavesObj = GameObject.Find("Waves").GetComponent<Waves>();
        _uIManager = GameObject.Find("Canvas").GetComponent<UIManager>();
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

            if (_enemiesSpawned < _wavesObj.waves[_currentWave].GetEnemiesInWave())
            {
                _enemiesSpawned++;
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
        int number = Random.Range(0, 10);
        int randomPowerup;

        if (number < 9)
        {
            randomPowerup = Random.Range(0, 5);
        }
        else
        {
            randomPowerup = 5;
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
        _enemiesSpawned = 0;
        _currentWave++;

        if (_currentWave >= _wavesObj.waves.Count)
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
        _uIManager.UpdateAndDisplayWaveText(_currentWave + 1);
        yield return new WaitForSeconds(5.0f);
        _stopSpawningEnemy = false;
        StartCoroutine(SpawnEnemyRoutine());
    }

    public int GetCurrentWave()
    {
        return _currentWave;
    }
}
