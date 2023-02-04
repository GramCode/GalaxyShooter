using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private float _speed = 3.0f;
    [SerializeField] private int _powerupID; //0 = Triple Shot, 1 = Speed, 2 = Shields

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
            //Collect
            Player player = other.gameObject.GetComponent<Player>();

            if(player != null)
            {
                switch (_powerupID)
                {
                    case 0:
                        player.TrippleShotActive();
                        break;
                    case 1:
                        player.SpeedBoostActive();
                        break;
                    case 2:
                        break;
                }
                
            }
           
            Destroy(this.gameObject);
        }
    }

}
