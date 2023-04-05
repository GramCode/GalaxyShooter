using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float _speed = 2;
    [SerializeField]
    private GameObject _target;

    private Player _playerScript;
    private SpawnManager _spawnManager;
    private Rigidbody2D _rigi;
    private GameObject _instantiatedTarget = null;
    private GameObject _lockedTarget;
    

    void Start()
    {
        _playerScript = GameObject.Find("Player").GetComponent<Player>();

        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("Spawn Manager in Projectile is NULL");
        }

        _rigi = GetComponent<Rigidbody2D>();
        if (_rigi == null)
        {
            Debug.Log("No Rigidbody on the Projectile");
        }

        SetTarget();
        StartCoroutine(IncreaseSpeedRoutine());
    }

    IEnumerator IncreaseSpeedRoutine()
    {
        while (_speed < 15)
        {
            yield return new WaitForSeconds(0.2f);
            _speed = _speed * 1.5f;
        }
        
    }

    void Update()
    {
        MoveTowardsEnemy();
        LookAtTarget();
    }

    private void MoveTowardsEnemy()
    {
        transform.position = Vector3.MoveTowards(transform.position, _lockedTarget.transform.position, _speed * Time.deltaTime);
    }

    private void LookAtTarget()
    {
        Vector3 direction = transform.position - _lockedTarget.transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _rigi.rotation = angle;
    }

    public void SetTarget()
    {
        _lockedTarget = _playerScript.GetTarget();
        _instantiatedTarget = Instantiate(_target, _lockedTarget.transform.position, Quaternion.identity);
        _instantiatedTarget.transform.parent = _lockedTarget.transform;
    }

    public void HideTarget()
    {
        _instantiatedTarget.SetActive(false);
    }

    public Transform GetPosition()
    {
        return transform;
    }


}
