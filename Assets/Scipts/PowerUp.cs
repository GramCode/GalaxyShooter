using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField] private float _speed = 3.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
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
                player.TrippleShotActive();
            }
           
            Destroy(this.gameObject);
        }
    }
}
