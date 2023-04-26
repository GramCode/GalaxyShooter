using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3f;
    [SerializeField]
    private GameObject[] _cannons;
    [SerializeField]
    private GameObject _blastPrefab;
    [SerializeField]
    private GameObject _laserBeamPrefab;
    [SerializeField]
    private GameObject _bossShield;
    [SerializeField]
    private GameObject _bossExplosion;

    private Player _player;
    private SpawnManager _spawnManager;
    private UIManager _uiManager;
    private GameManager _gameManagerScript;
    private CameraBehavior _camera;
    private Cannon _cannonScript;
    private List<Cannon> _scripts = new List<Cannon>();
    private bool _shootLaserBeam = false;
    private bool _isShieldActive = true;
    private bool _isBackToCycle = false;
    private bool _isDestroyed = false;
    private int _bossLives = 3;
    
    private void Start()
    {
        _camera = GameObject.Find("Main Camera").GetComponent<CameraBehavior>();
        if (_camera == null)
        {
            Debug.LogError("The Main Camera in Boss is NULL");
        }

        _gameManagerScript = GameObject.Find("Game Manager").GetComponent<GameManager>();
        if (_gameManagerScript == null)
        {
            Debug.LogError("The Game Manager in Boss is NULL");
        }

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager in Boss is NULL");
        }

        _spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("Spawn Manager in Boss is NULL");
        }

        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player in Boss is NULL");
        }

        foreach (var cannon in _cannons)
        {
            _cannonScript = cannon.GetComponent<Cannon>();
            if (_cannonScript == null)
            {
                Debug.LogError("The Cannon in Boss is NULL");
            }
            else
            {
                _scripts.Add(_cannonScript);
            }
        }

        _uiManager.HideWavesCount();
    }

    private void Update()
    {
        MovementBehavior();

        if (_shootLaserBeam)
        {
            _shootLaserBeam = false;
            _isBackToCycle = false;
            ShootLaserBeam();
        }
    }

    private void MovementBehavior()
    {
        if (transform.position.y > 5.6f)
        {
            transform.Translate(Vector2.down * _speed * Time.deltaTime);
        }
        else
        {
            foreach (var script in _scripts)
            {
                script.Rotate();
            }

        }
    }

    private void ShootLaserBeam()
    {

        GameObject blast = Instantiate(_blastPrefab, transform.position + new Vector3(0.09f, -3.81f, 0), Quaternion.identity);
        StartCoroutine(ScaleBlastRoutine(blast));
    }

    IEnumerator ScaleBlastRoutine(GameObject blast)
    {
        float value = 0;
        while (value < 1.0 && Time.timeScale != 0)
        {
            value += 0.01f;
            blast.transform.localScale = new Vector3(value, value, value);

            yield return new WaitForEndOfFrame();
        }
        float positionToInstatiate;
        Destroy(blast, 0.2f);
        positionToInstatiate = transform.position.y - 8.6f;
        Instantiate(_laserBeamPrefab, new Vector3(transform.position.x + 0.07f, positionToInstatiate, 0), Quaternion.identity);
        StartCoroutine(DisableShieldRoutine());

    }

    IEnumerator DisableShieldRoutine()
    {
        
        yield return new WaitForSeconds(0.8f);
        float time = 0;
        while (time < 0.8)
        {
            time += 0.2f;
            _bossShield.SetActive(false);
            yield return new WaitForSeconds(0.2f);
            _bossShield.SetActive(true);
            yield return new WaitForSeconds(0.2f);
        }
        _bossShield.SetActive(false);
        yield return null;
        _isShieldActive = false;

        StartCoroutine(EnableShielCoolDown());       
    }

    IEnumerator EnableShielCoolDown()
    {
        yield return new WaitForSeconds(2.5f);
        if (!_isDestroyed)
        {
            
            if (_scripts[0] != null && !_isBackToCycle)
            {
                _isBackToCycle = true;
                StartCoroutine(EnableShieldRoutine());
                _isShieldActive = true;
                foreach (var script in _scripts)
                {
                    script.ResetCicle();
                    script.Rotate();
                }
            }
            
        }
        
    }

    private void DamageBoss(GameObject other)
    {
        if (_isShieldActive)
        {
            Destroy(other);
            return;
        }
        else
        {
            _bossLives--;
            _uiManager.UpdateBossLives(_bossLives);
            _camera.ShakeCamera();
            Destroy(other);

            if (_bossLives == 0)
            {
                //Boss Defeated, Game Completed
                Destroy(this.gameObject.GetComponent<Collider2D>());
                for (int i = 0; i < 4; i++)
                {
                    Destroy(transform.GetChild(i).gameObject);
                }
                Destroy(this.gameObject, 0.8f);
                Instantiate(_bossExplosion, transform.position, Quaternion.identity);
                GameEnded();
            }
            else
            {
                if (_isBackToCycle == false)
                {
                    _isShieldActive = true;
                    _isBackToCycle = true;
                    StartCoroutine(EnableShieldRoutine());

                    foreach (var script in _scripts)
                    {
                        script.ResetCicle();
                        script.Rotate();
                    }
                }
                
            }
        }
    }

    IEnumerator EnableShieldRoutine()
    {
        float elapsedTime = 0;
        while (elapsedTime < 0.8f)
        {
            elapsedTime += 0.2f;
            _bossShield.SetActive(false);
            yield return new WaitForSeconds(0.2f);
            _bossShield.SetActive(true);
            yield return new WaitForSeconds(0.2f);
        }
        _bossShield.SetActive(true);
        _isShieldActive = true;
    }

    private void GameEnded()
    {
        _gameManagerScript.CompletedGame();
        _uiManager.AllWavesCompleted();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Laser"))
        {
            DamageBoss(other.gameObject);
        }

        if (other.CompareTag("Projectile"))
        {
            _player.DontShootProjectile();
            _player.HideTargetRange();
            other.GetComponent<Projectile>().DestroyProjectile();
            other.GetComponent<Projectile>().DestroyTarget();
            
            DamageBoss(other.gameObject);
        }
    }

    public void ShootBeam()
    {
        _shootLaserBeam = true;
    }
}
