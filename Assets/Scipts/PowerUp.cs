using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private float _speed = 3.0f;
    [SerializeField] private int _powerupID; //0 = Triple Shot, 1 = Speed, 2 = Shields, 3 = Bullets, 4 = life, 5 = negative speed, 6 = spread shot
    [SerializeField] private AudioClip _clip;

    private UIManager _uiManager;
    private bool _moveTowardsPlayer = false;
    private GameObject _player;

    private void Start()
    {
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.LogError("UI Manager in PowerUp is NULL");
        }

        _player = GameObject.Find("Player");
        if (_player == null)
        {
            Debug.LogError("The player game object is NULL in the powerup script");
        }
    }

    private void Update()
    {
        PowerUpBehavior();
        MoveToPlayer();
    }

    private void PowerUpBehavior()
    {
        float pos = -5.8f;

        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < pos)
        {
            Destroy(this.gameObject);
        }
    }

    private void MoveToPlayer()
    {

        if (Input.GetKey(KeyCode.C))
        {
            _moveTowardsPlayer = true;
        }

        if (_moveTowardsPlayer == true)
        {
            Vector3 playerPosition = _player.transform.position;
            transform.position = Vector3.MoveTowards(transform.position, playerPosition, (_speed * 2) * Time.deltaTime);
        }
        
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.gameObject.GetComponent<Player>();
            AudioSource.PlayClipAtPoint(_clip, transform.position);

            if(player != null)
            {
                switch (_powerupID)
                {
                    case 0:
                        if (player.GetSpreadShotActive())
                        {
                            player.SpreadShotNotActive();
                        }
                        player.TripleShotActive();
                        break;
                    case 1:
                        player.SpeedBoostActive();
                        break;
                    case 2:
                        player.ResetShieldLives();
                        player.ShieldActive();
                        break;
                    case 3:
                        player.RefillAmmo();
                        break;
                    case 4:
                        player.AddLive();
                        break;
                    case 5:
                        if (player.GetTripleShotActive())
                        {
                            player.TripleShotNotActive();
                        }
                        player.SpreadShotActive();
                        break;
                    case 6:
                        player.NegativeSpeedActive();
                        break;
                }
                
            }

            Destroy(this.gameObject);
        }

        if (other.CompareTag("EnemyLaser"))
        {
            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}
