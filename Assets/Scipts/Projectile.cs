using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float _speed = 2;
    [SerializeField]
    private GameObject _target;
    [SerializeField]
    private GameObject _projectileExplosion;

    private Player _playerScript;
    private SpawnManager _spawnManager;
    private Rigidbody2D _rigi;
    private GameObject _instantiatedTarget = null;
    private GameObject _lockedTarget;
    private bool _isTargetDestroyed;

    void Start()
    {
        _playerScript = GameObject.Find("Player").GetComponent<Player>();
        if (_playerScript == null)
        {
            Debug.LogError("The Player in Projectile is NULL");
        }

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
        transform.rotation = new Quaternion(0, 0, -90, 1);
        SetTarget();
        StartCoroutine(IncreaseSpeedRoutine());
        StartCoroutine(DestroyProjectileRoutine());
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
        if (_isTargetDestroyed)
        {
            transform.Translate(Vector2.up * _speed * Time.deltaTime);
            Destroy(_instantiatedTarget);
        }
        if (_lockedTarget != null)
        {
            MoveTowardsEnemy();
            LookAtTarget();
        }
        else
        {
            transform.Translate(Vector2.up * _speed * Time.deltaTime);
        }
        
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

    private IEnumerator DestroyProjectileRoutine()
    {
        yield return new WaitForSeconds(2.3f);
        DestroyTarget();
        DestroyProjectile();
    }

    private void SetTarget()
    {
        if (_spawnManager.BossHasSpawned)
        {
            _lockedTarget = _spawnManager.InstantiatedBoss;
            _instantiatedTarget = Instantiate(_target, _lockedTarget.transform.position, Quaternion.identity);
            _instantiatedTarget.transform.parent = _lockedTarget.transform;
        }
        else
        {
            if (_playerScript.GetTarget() == null)
            {
                Debug.Log("target is null");
                DestroyProjectile();
                DestroyTarget();
            }
            else
            {
                _lockedTarget = _playerScript.GetTarget();
                _instantiatedTarget = Instantiate(_target, _lockedTarget.transform.position, Quaternion.identity);
                _instantiatedTarget.transform.parent = _lockedTarget.transform;
            }
            
        }
       
    }

    public void DestroyProjectile()
    {
        Destroy(gameObject.GetComponent<Collider2D>());
        Destroy(this.gameObject, 0.4f);
        Instantiate(_projectileExplosion, transform.position, Quaternion.identity);
    }

    public void EnemyTargetDestroyed()
    {
        _isTargetDestroyed = true;
    }

    public void DestroyTarget()
    {
        Destroy(_instantiatedTarget);
    }
}
