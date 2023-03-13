using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private float _rotateSpeed = 20.0f;
    [SerializeField] private GameObject _explosionAnim;

    private SpawnManager _spawnManager;
    private UIManager _uiManager;
    public bool HasDestroyedAsteroid { get; private set; }

    private void Start()
    {
        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        HasDestroyedAsteroid = false;
    }

    void Update()
    {
        transform.Rotate(Vector3.forward * _rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Laser")
        {
            _uiManager.UpdateAndDisplayWaveText(SpawnManager.CurrentWave + 1);
            Instantiate(_explosionAnim, transform.position, Quaternion.identity);
            Destroy(collision.gameObject);
            HasDestroyedAsteroid = true;
            _spawnManager.StartSpawning();
            Destroy(this.gameObject, 0.2f);
            
        }
    }
}
