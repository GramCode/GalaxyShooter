using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private float _rotateSpeed = 20.0f;
    [SerializeField] private GameObject _explosionAnim;

    private SpawnManager _spawnManager;

    private void Start()
    {
        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
    }

    void Update()
    {
        transform.Rotate(Vector3.forward * _rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Laser")
        {
            Instantiate(_explosionAnim, transform.position, Quaternion.identity);
            Destroy(collision.gameObject);
            
            _spawnManager.StartSpawning();
            Destroy(this.gameObject, 0.2f);
            
        }
    }
}
