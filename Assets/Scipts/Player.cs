using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;


    [SerializeField] private GameObject _laserPrefab;

    void Update()
    {
        CalculateMovement();
        ShootLaser();
    }

    void ShootLaser()
    {
        //Check if the space key is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(_laserPrefab, transform.position, Quaternion.identity);
        }
    }

    void CalculateMovement()
    {
        float horizontalAxis = Input.GetAxis("Horizontal");
        float verticalAxis = Input.GetAxis("Vertical");

        transform.Translate(new Vector3(horizontalAxis * _speed * Time.deltaTime, verticalAxis * _speed * Time.deltaTime, 0));
    }
}
