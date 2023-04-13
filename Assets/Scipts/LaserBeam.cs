using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    [SerializeField] private float _speed = 8.0f;

    private Vector3 _laserBeamSize;
    private float _elapsedTimeY;
    private float _elapsedTimeX;
    private float _durationY = 0.7f;
    private float _durationX = 0.8f;
    private AudioSource _audioSource;
    private bool _havePlayedAudio = false;
    private Collider2D _collider;
    
    void Start()
    {
        _laserBeamSize = transform.localScale;
        _audioSource = GetComponent<AudioSource>();

        if (_audioSource == null)
        {
            Debug.LogError("Audio Source is null in LaserBeam");
        }

        _collider = GetComponent<Collider2D>();
    }

    void Update()
    {
        ShootLaserBeam();
        if (transform.position.y < -10.9f)
        {
            Destroy(this.gameObject);
        }
    }

    private void ShootLaserBeam()
    {
        
        float currentValue;

        if (transform.position.y > -2.0f)
        {
            _elapsedTimeY += Time.deltaTime;

            currentValue = _elapsedTimeY / _durationY;
            transform.localScale = Vector3.Lerp(_laserBeamSize, new Vector3(2.3f, 4, 1), currentValue);

        }
        else
        {
            _elapsedTimeX += Time.deltaTime;

            currentValue = _elapsedTimeX / _durationX;
            transform.localScale = Vector3.Lerp(new Vector3(2.3f, 4, 1), new Vector3(0.5f, 4, 1), currentValue);

        }

        transform.Translate(Vector3.down * _speed * Time.deltaTime);

      

        if (!_audioSource.isPlaying && !_havePlayedAudio)
        {
            _havePlayedAudio = true;
            _audioSource.Play();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                player.Damage();
                Destroy(_collider);
            }
        }
        
        if (other.tag == "Powerup")
        {
            Destroy(other.gameObject);
        }
    }
}
