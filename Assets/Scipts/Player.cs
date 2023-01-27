using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private GameObject _laserPrefab;

    [SerializeField] private float _fireRate = 0.5f;

    private float _canFire = -1f;

    void Update()
    {
        CalculateMovement();
        ShootLaser();
    }

    void ShootLaser()
    {
        //Check if the space key is pressed and 0.5s has passed
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            _canFire = Time.time + _fireRate;
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 0.8f,0), Quaternion.identity);
        }
    }

    void CalculateMovement()
    {
        float horizontalAxis = Input.GetAxis("Horizontal");
        float verticalAxis = Input.GetAxis("Vertical");

        transform.Translate(new Vector3(horizontalAxis * _speed * Time.deltaTime, verticalAxis * _speed * Time.deltaTime, 0));
    }
}
