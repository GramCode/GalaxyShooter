using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 4.0f;
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private AudioClip _laserAudioClip;

    private AudioSource _audioSource;
    private Player _player;
    private Collider2D _collider2D;
    private Animator _anim;

    private float _fireRate = 3.0f;
    private float _canShoot = -1;

    private void Start()
    {
        _player = GameObject.Find("Player").transform.GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("The Player was not found inside the Enemy Script");
        }

        _anim = GetComponent<Animator>();
        if (_anim == null)
        {
            Debug.LogError("Animator component of Enemy not found.");
        }

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("AudioSource on Enemy is NULL.");
        }
       
        _collider2D = GetComponent<Collider2D>();
        if (_collider2D == null)
        {
            Debug.LogError("Enemy collider is NULL");
        }
        //start coroutine

    }

    void Update()
    {
        EnemyBehavior();

        if (Time.time > _canShoot)
        {
            _fireRate = Random.Range(3f, 7f);
            _canShoot = Time.time + _fireRate;
            float positionToInstatiate = transform.position.y - 0.6f;
            GameObject enemyLaser = Instantiate(_laserPrefab, new Vector3(transform.position.x, positionToInstatiate, 0), Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

            foreach(var laser in lasers)
            {
                laser.AssignEnemyLaser();
            }
        }
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
            _anim.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _audioSource.Play();
            Destroy(_collider2D);
            Destroy(this.gameObject, 2.8f);
        }

        if (other.CompareTag("Laser"))
        {
            if (_player != null)
                _player.AddScore(10);
            
            _anim.SetTrigger("OnEnemyDeath");
            _speed = 0;
            _audioSource.Play();
            Destroy(_collider2D);
            Destroy(this.gameObject, 2.8f);
            Destroy(other.gameObject);
        }

        
    }

    
    
}
