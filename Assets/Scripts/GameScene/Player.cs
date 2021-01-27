using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Player : MonoBehaviour
{
    // Prefab holders
    [Header("Prefabs")]
    [SerializeField] private ProjectilesSO projectilePrefabs = null;
    [SerializeField] private GameObject _activeProjectilesContainer = null;
    [SerializeField] private GameObject _shieldsPrefab = null;
    [SerializeField] private GameObject _explosionPrefab = null;
    [SerializeField] private List<GameObject>_playerDamagedPrefabs = null;

    [Header("Audio")]
    [SerializeField] private AudioClip _shieldSound = null;
    [SerializeField] private AudioClip _takeDamageSound = null;
    [SerializeField] private AudioClip _powerupCollected = null;
    [SerializeField] private AudioClip _outOfAmmoSound = null;
    [SerializeField] private AudioClip _collectablePickupSound = null;

    // Configuration
    [Space(5)]
    [Header("Configuration")]
    [SerializeField] private Vector3 _startPosition = new Vector3(0, -3, 0);
    [SerializeField] private float _speed = 0;
    [SerializeField] private float _fireRate = 0f;
    [SerializeField] private int _lives = 0;
    [SerializeField] private float _thrustersMultiplier = 1.5f;
    [SerializeField] private int _ammoQuantity = 15;
    [SerializeField] private bool _blackHoleAvailable = false;
    [SerializeField] private int _enemiesKilled = 0;
    [SerializeField] private int _enemiesKilledPerBlackHole = 20;

    // Actions
    public static Action playerDied;
    public static Action<int> livesChanged;
    public static Action<bool> blackHoleAvailable;

    private AudioSource _audioSource = null;
    private GameObject _mainCamera = null;

    private float _gameObjectWidth = 0;
    private float nextFireTime = 0f;
    private HashSet<PowerUp.PowerUpType> _activePowerUps = new HashSet<PowerUp.PowerUpType>();
    private Shields _shieldsComponent = null;

    void Start()
    {
        _activeProjectilesContainer = GameObject.Find("ProjectilesContainer");
        _audioSource = transform.GetComponent<AudioSource>();
        _mainCamera = GameObject.Find("Main Camera");
        _shieldsComponent = _shieldsPrefab.GetComponent<Shields>();

        // Safety dance
        if (_shieldsComponent == null)
            Debug.LogError("_shieldsComponent is null");

        if (_mainCamera == null)
            Debug.LogError("_mainCamera is null");

        if (_audioSource == null)
            Debug.LogWarning("_audioSource is null");

        if (_takeDamageSound == null || _shieldSound == null || _powerupCollected == null 
            || _outOfAmmoSound == null || _collectablePickupSound == null)
            Debug.LogWarning("Sound clips are null");

        if (_explosionPrefab == null)
            Debug.LogError("_explosionPrefab is null");

        if (_playerDamagedPrefabs == null)
            Debug.LogError("_playerDamagedPrefabs is null");

        if (_shieldsPrefab == null)
            Debug.LogError("_shieldsPrefab is null");

        if (projectilePrefabs == null)
            Debug.LogError("projectilePrefabs is null");

        if (_activeProjectilesContainer == null)
            Debug.LogError("_activeProjectilesContainer is null");

        // Gotchas
        if (_speed == 0)
            Debug.LogWarning($"FYI: _speed is set to {_speed}]");

        if (_fireRate == 0)
            Debug.LogWarning("_fireRate is 0");

        if (_lives == 0)
            Debug.LogWarning("_lives is 0");


        // Initialization
        transform.position = _startPosition;
        // Get width of player to use for looping X pos
        _gameObjectWidth = GetComponent<BoxCollider2D>().bounds.size.x / 2;

        Enemy.killedByPlayer += EnemyKilled;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

        if (Input.GetKeyDown(KeyCode.Space) && CanFire())
            FireLaser();

        if (_blackHoleAvailable && Input.GetKeyDown(KeyCode.B))
            FireBlackHole();
    }
    
    // Movement

    void Movement()
    {      
        float horiz = Input.GetAxis("Horizontal");
        float vert = Input.GetAxis("Vertical");

        float speed = CalculateSpeed();
        
        // Begin to move the GameObject
        Vector3 nextVect = new Vector3(horiz, vert, 0) * speed * Time.deltaTime;
        transform.Translate(nextVect);

        // Clamp to Y range
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -GameDataSO.yRange, GameDataSO.yRange), 0);

        // Wrap X
        float xWrapBoundary = GameDataSO.xRange + _gameObjectWidth;

        if(transform.position.x > xWrapBoundary)
            transform.position = new Vector3(-GameDataSO.xRange, transform.position.y, 0);

        if (transform.position.x < -xWrapBoundary)
            transform.position = new Vector3(GameDataSO.xRange, transform.position.y, 0);
    }

    // Attacking

    void FireLaser()
    {
        if(_ammoQuantity <= 0)
        {
            _audioSource.clip = _outOfAmmoSound;
            _audioSource.Play();
            return;
        }

        GameObject nextShotPrefab = FireStrategy();
        GameObject activeProjectile = Instantiate(nextShotPrefab, transform.position, Quaternion.identity);
        activeProjectile.transform.parent = _activeProjectilesContainer.transform;

        _ammoQuantity -= 1;
        nextFireTime = Time.time + _fireRate;
    }

    bool CanFire()
    {
        if (Time.time < nextFireTime)
            return false;

        return true;
    }
    GameObject FireStrategy()
    {
        if (_activePowerUps.Contains(PowerUp.PowerUpType.TRIPLE_SHOT))
            return projectilePrefabs.tripleShot;

        return projectilePrefabs.laser;
    }

    // Public Gameplay Methods
    public void TakeDamage(int damageAmount = 1)
    {
        if(_activePowerUps.Contains(PowerUp.PowerUpType.SHIELD))
        {
            //StartCoroutine(DeactivatePowerUp(PowerUp.PowerUpType.SHIELD, 0));
            if (_shieldsComponent != null)
                _shieldsComponent.TakeDamage();

            if (_audioSource != null)
            {
                _audioSource.clip = _shieldSound;
                _audioSource.Play(0);
            }

            return;
        }

        _lives -= damageAmount;
        VisualizeDamage();

        if(_mainCamera != null)
        {
            Animator anim = _mainCamera.GetComponent<Animator>();
            if (anim != null)
                anim.SetTrigger("shakeCamera");
        }

        if (_audioSource != null)
        {
            _audioSource.clip = _takeDamageSound;
            _audioSource.Play(0);
        }

        if (livesChanged != null)
            livesChanged(_lives);
        
        if(_lives <= 0)
        {
            playerDied();
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Destroy(this.gameObject, 0.25f);
        }
    }

    void VisualizeDamage()
    {
        GameObject nextDamagePrefab = _playerDamagedPrefabs.Where(pref => pref.gameObject.activeSelf == false).FirstOrDefault();
        
        if (nextDamagePrefab != null)
            nextDamagePrefab.SetActive(true);
    }

    void RemoveDamage()
    {
        if (livesChanged != null)
            livesChanged(_lives);

        GameObject nextActivePrefab = _playerDamagedPrefabs.Where(pref => pref.gameObject.activeSelf == true).FirstOrDefault();

        if (nextActivePrefab != null)
            nextActivePrefab.SetActive(false);
    }

    public void CollectPowerUp(PowerUp.PowerUpType powerUpType)
    {
        if(_audioSource != null && _powerupCollected != null)
        {
            _audioSource.clip = _powerupCollected;
            _audioSource.Play();
        }

        ActivatePowerUp(powerUpType);
        if (powerUpType == PowerUp.PowerUpType.SHIELD)
            Shields.OnShieldDestroyed += ShieldDestroyed;
    }

    void ActivatePowerUp(PowerUp.PowerUpType powerUpType)
    {
        _activePowerUps.Add(powerUpType);

        switch (powerUpType)
        {
            case PowerUp.PowerUpType.TRIPLE_SHOT:
                StartCoroutine(DeactivatePowerUp(powerUpType, 5));
                break;
            case PowerUp.PowerUpType.SPEED_BOOST:
                StartCoroutine(DeactivatePowerUp(powerUpType, 3));
                break;
            case PowerUp.PowerUpType.SHIELD:
                if (_shieldsPrefab != null)
                    _shieldsPrefab.SetActive(true);
                break;
            default:
                Debug.LogWarning($"Unhandled powerup: {powerUpType}");
                break;
        }
    }

    IEnumerator DeactivatePowerUp(PowerUp.PowerUpType powerUpType, int seconds)
    {
        yield return new WaitForSeconds(seconds);
        _activePowerUps.Remove(powerUpType);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy_Projectile") == false)
            return;

        TakeDamage();
    }

    private float CalculateSpeed()
    {
        float speed = _activePowerUps.Contains(PowerUp.PowerUpType.SPEED_BOOST) ? _speed * 2f : _speed;

        if (Input.GetKey(KeyCode.LeftShift))
            speed = speed * _thrustersMultiplier;

        return speed;
    }

    private void ShieldDestroyed()
    {
        Shields.OnShieldDestroyed -= ShieldDestroyed;

        if (_shieldsPrefab != null)
            _shieldsPrefab.SetActive(false);

        StartCoroutine(DeactivatePowerUp(PowerUp.PowerUpType.SHIELD, 0));
    }

    public void CollectCollectable(Collectable.CollectableType collectableType)
    {
        if (_audioSource != null && _powerupCollected != null)
        {
            _audioSource.clip = _collectablePickupSound;
            _audioSource.Play();
        }

        switch (collectableType)
        {
            case Collectable.CollectableType.AMMO:
                _ammoQuantity = 15;
                break;
            case Collectable.CollectableType.HEALTH:
                if (_lives >= 3)
                    return;
                _lives++;
                RemoveDamage();
                break;
            default:
                Debug.LogWarning($"Unhandled collectable: {collectableType}");
                break;
        }
    }

    void FireBlackHole()
    {
        _blackHoleAvailable = false;
        blackHoleAvailable?.Invoke(false);
        Instantiate(projectilePrefabs.blackHoleCreator, transform.position, Quaternion.identity);
    }

    void EnemyKilled(int score)
    {
        _enemiesKilled += 1;

        // Enable BlackHole ever 20 kills
        if(_blackHoleAvailable == false)
        {
            if (_enemiesKilled % _enemiesKilledPerBlackHole == 0)
            {
                _blackHoleAvailable = true;
                blackHoleAvailable?.Invoke(true);
            }
        }
    }
}
