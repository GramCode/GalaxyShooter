using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{

    private Vector3 _startingPositon;
    private float _shakeDuration = 0.3f;
    private float _elapsedTime = 0f;

    void Start()
    {
        _startingPositon = transform.position;
    }

    public void ShakeCamera()
    {
        StartCoroutine(ShakeCameraRoutine());
    }

    IEnumerator ShakeCameraRoutine()
    {
        while (_elapsedTime < _shakeDuration)
        {
            _elapsedTime += Time.deltaTime;
            float randPos = Random.Range(-0.5f, 0.5f);
            Vector3 randVector = new Vector3(randPos, randPos);
            transform.position = _startingPositon + randVector;
            yield return null;
        }
        transform.position = _startingPositon;
        _elapsedTime = 0;
    }
}
