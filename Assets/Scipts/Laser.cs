using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{

    [SerializeField] private float _speed = 8.0f;
    [SerializeField] private bool _isSpreadShot;
    [SerializeField] private bool[] _lasers;
    [SerializeField] private AudioClip _audioClip;
    
    private GameObject _camera;
    private Vector3 _distance;
    private bool _isEnemyLaser = false;
    private bool _isEnemyShootingBackward = false;

    private void Start()
    {
        _camera = GameObject.Find("Main Camera");

        if (_isSpreadShot)
        {
            _distance = transform.position;
            AudioSource.PlayClipAtPoint(_audioClip, _camera.transform.position, 1.0f);
        }
        else
        {
            AudioSource.PlayClipAtPoint(_audioClip, _camera.transform.position, 1.0f);
        }
    }

    void Update()
    {

        if (_isEnemyLaser == false || _isEnemyShootingBackward)
        {
            MoveUp();
        }
        else
        {
            MoveDown();
        }
       
    }

    private void MoveUp()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        if (_isSpreadShot)
        {
            if (transform.position.y > _distance.y + 5.0f)
            {
                if (transform.parent != null)
                {
                    Destroy(transform.parent.gameObject);
                }

                Destroy(this.gameObject);
            }
        }

        if (transform.position.y > 8.0f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }

            Destroy(this.gameObject);
        }
    }

    private void MoveDown()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -8.0f)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }

            Destroy(this.gameObject);
        }
    }

    public void AssignEnemyLaser()
    {
        _isEnemyLaser = true;
    }

    public void ShootingBackwards()
    {
        _isEnemyShootingBackward = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && _isEnemyLaser == true)
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
                Destroy(this.gameObject);
            }
        }
    }

    public Vector3 LaserPosition()
    {
        return transform.position;
    }
}