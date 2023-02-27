using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private float _speed = 3.0f;
    [SerializeField] private int _powerupID; //0 = Triple Shot, 1 = Speed, 2 = Shields, 3 = Bullets, 4 = life, 5 = spread shot
    [SerializeField] private AudioClip _clip;

    void Update()
    {
        PowerUpBehavior();
    }

    void PowerUpBehavior()
    {
        float pos = -5.8f;

        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < pos)
        {
            Destroy(this.gameObject);
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
                }
                
            }

            Destroy(this.gameObject);
        }
    }

}
