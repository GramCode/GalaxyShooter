using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    [SerializeField]
    private GameObject _bossLasersPrefab;
    [SerializeField]
    private Transform _shootPoint;
    [SerializeField]
    private GameObject _laserBeam;
    [SerializeField]
    private GameObject _boss;

    private float _maxRotation = 38.0f;
    private float _minRotation = -38.0f;
    private float _elapsedTime = 0;
    private float _moveLeftDuration = 0;
    private float _coroutineTime;
    private float _startingRotationDuration = 1.8f;
    private float _fullRotationDuration = 3.6f;
    private float _backToStart;
    private bool _shouldStartBehavior = false;
    private bool _moveRight = false;
    private bool _moveLeft = false;
    private bool _backToCenter = false;

    private Boss _bossScript;

    void Start()
    {
        _bossScript = _boss.GetComponent<Boss>();
        if (_bossScript == null)
        {
            Debug.LogError("The Boss in Cannon is NULL");
        }
        StartCoroutine(StartingRoutine());
    }

    void Update()
    {
        
        if (_shouldStartBehavior && _moveRight == false)
        {
            StartingRotation();
        }

        if (_moveRight && _moveLeft == false)
        {
            RotateRight();
        }

        if (_moveLeft && _moveRight == false)
        {
            RotateLeft();
        }

        if (_backToCenter)
        {
            BackToStartingRotation();
        }
        

    }

    private void StartingRotation()
    {
        float currentValue = _elapsedTime / _startingRotationDuration;
        _elapsedTime += Time.deltaTime;
        transform.eulerAngles = Vector3.Lerp(Vector3.zero, new Vector3(0, 0, _minRotation), currentValue);
    }

    private void RotateRight()
    {
        float currentValue = _elapsedTime / _fullRotationDuration;
        _elapsedTime += Time.deltaTime;
        transform.eulerAngles = Vector3.Lerp(new Vector3(0, 0, _minRotation), new Vector3(0, 0, _maxRotation), currentValue);
    }

    private void RotateLeft()
    {
        float currentValue = _moveLeftDuration / _fullRotationDuration;
        _moveLeftDuration += Time.deltaTime;
        transform.eulerAngles = Vector3.Lerp(new Vector3(0, 0, _maxRotation), new Vector3(0, 0, _minRotation), currentValue);
    }

    private void BackToStartingRotation()
    {
        float currentValue = _backToStart / _startingRotationDuration;
        _backToStart += Time.deltaTime;
        transform.eulerAngles = Vector3.Lerp(new Vector3(0, 0, _minRotation), Vector3.zero, currentValue);
    }

    IEnumerator StartingRoutine()
    {
        yield return new WaitForSeconds(4.3f);
        _shouldStartBehavior = false;
        _elapsedTime = 0;
        _moveRight = true;
        StartCoroutine("StartShootingRoutine");
    }

    IEnumerator StartShootingRoutine()
    {
        while (_coroutineTime < _fullRotationDuration * 2 + 1)
        {
            _coroutineTime += 0.5f;
            if (_coroutineTime > _fullRotationDuration)
            {
                _moveLeft = true;
                _moveRight = false;
            }
            GameObject bossLaser = Instantiate(_bossLasersPrefab, _shootPoint.position, transform.rotation);
            Laser lasers = bossLaser.GetComponent<Laser>();
            lasers.AssignEnemyLaser();
            yield return new WaitForSeconds(0.5f);
        }
        _backToCenter = true;
        yield return new WaitForSeconds(1.8f);
        _moveLeft = false;
        _moveRight = false;
        _bossScript.ShootBeam();
    }

    public void Rotate()
    {
        _shouldStartBehavior = true;
    }

    public void ResetCicle()
    {
        _backToCenter = false;
        _moveRight = false;
        _moveLeft = false;
        _shouldStartBehavior = true;
        _elapsedTime = 0;
        _moveLeftDuration = 0;
        _coroutineTime = 0;
        _backToStart = 0;
        StartCoroutine(RepeatCoroutine());
    }

    IEnumerator RepeatCoroutine()
    {
        yield return new WaitForSeconds(1.9f);
        StartCoroutine("StartShootingRoutine");
        _shouldStartBehavior = false;
        _elapsedTime = 0;
        _moveRight = true;
    }
}
