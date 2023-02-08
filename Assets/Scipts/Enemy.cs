using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 4.0f;
    private Player _player;

    private void Start()
    {
        _player = GameObject.Find("Player").transform.GetComponent<Player>();
    }

    void Update()
    {
        EnemyBehavior();
    }

    void EnemyBehavior()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -5.6f)
        {
            float randomX = Random.Range(-9, 9);
            transform.position = new Vector3(randomX, 7.5f, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
           

            if (_player != null)
                _player.Damage();

            Destroy(this.gameObject);
        }

        if (other.CompareTag("Laser"))
        {
            //Add 10 to score
            if (_player != null)
                _player.AddScore(10);
            Destroy(this.gameObject);
            Destroy(other.gameObject);
        }
    }

    
    
}
