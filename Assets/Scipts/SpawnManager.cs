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
    private bool _stopSpawning = false;

    void Start()
    {
        _posToSpawnEnemy = new Vector3(Random.Range(-8f, 8f), 7, 0);
        _posToSpawnPowerup = new Vector3(Random.Range(-8f, 8f), 7, 0);
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

        while (_stopSpawning == false)
        {
            
            GameObject newEnemy = Instantiate(_enemyPrefab, _posToSpawnEnemy, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(5.0f);
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        while (_stopSpawning == false)
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
        _stopSpawning = true;
    }

  
}
