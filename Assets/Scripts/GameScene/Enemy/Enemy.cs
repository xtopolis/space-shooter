using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MonoBehaviour, IDestroyable
{
    [Header("Prefabs")]
    [SerializeField] private GameObject _laserPrefab = null;

    [Header("Config")]
    [SerializeField] private float _speed = 0;

    public static Action<int> killedByPlayer;

    private AudioSource _audioSource = null;

    private enum EnemyType
    {
        NORMAL,
        SIDE_TO_SIDE,
        ANGLER
    }

    private float _canFire = -1f;
    private float _fireRate = 3f;
    private bool _isDestroyed = false;
    private EnemyType _enemyType = EnemyType.NORMAL;

    // Side to Side fields
    private bool _movingLeft = true;
    private float _yStopPos = 0f;

    // Angler
    GameObject player = null;

    public event Action OnWillDestroy;

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = transform.GetComponent<AudioSource>();

        if (_audioSource == null)
            Debug.LogWarning("_audioSource is null");

        if (_speed == 0)
            Debug.LogWarning("_speed is 0");

        transform.position = GameDataSO.RandomSpawnPositionNearBounds();
        _yStopPos = UnityEngine.Random.Range(GameDataSO.yRange - 2, GameDataSO.yRange - 4);

        SetEnemyType();

        //Angler
        if (_enemyType == EnemyType.ANGLER)
        {
            player = GameObject.Find("Player");
            if (player == null)
                Debug.LogError("player is null");

            Quaternion rotation = Quaternion.identity;
            rotation.eulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(-30f, 30f));
            transform.rotation = rotation;
        }
    }

    void SetEnemyType()
    {
        Array values = Enum.GetValues(typeof(EnemyType));
        System.Random random = new System.Random();
        EnemyType randomEnemy = (EnemyType)values.GetValue(random.Next(values.Length));
        _enemyType = randomEnemy;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

        if (Time.time > _canFire)
            FireLaser();
    }

    void Movement()
    {
        switch (_enemyType)
        {
            case EnemyType.NORMAL:
            case EnemyType.ANGLER:
                MoveNormally();
                break;
            case EnemyType.SIDE_TO_SIDE:
                MoveSideToSide();
                break;
        }

        if (transform.position.y < -(GameDataSO.yRange + 5f)) // 5f buffer
            transform.position = GameDataSO.RandomSpawnPositionNearBounds();
    }

    void MoveNormally()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
    }

    void MoveSideToSide()
    {
        if (transform.position.y >= _yStopPos)
        {
            MoveNormally();
            return;
        }

        Vector3 direction = _movingLeft ? Vector3.left : Vector3.right;
        direction *= _speed * Time.deltaTime;

        transform.Translate(direction);

        if (_movingLeft)
        {
            if (transform.position.x <= -GameDataSO.xRange)
                _movingLeft = false;
        }
        else
        {
            if (transform.position.x >= GameDataSO.xRange)
                _movingLeft = true;
        }
    }

    void MoveAtAngle()
    {
        MoveNormally();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null)
            return;

        if (_isDestroyed)
            return;

        switch (other.tag)
        {
            case "Player":
                // Animating explosion, don't hurt player

                Player player = other.GetComponent<Player>();
                if (player != null)
                    player.TakeDamage();
                DestroyComponentWithAnimation();
                break;

            case "Projectile_Laser":
                if (killedByPlayer != null)
                    killedByPlayer(10);
                Destroy(other.gameObject);
                DestroyComponentWithAnimation();

                break;

            case "Enemy":
                break;

            default:
                //Debug.LogWarning($"Collided with unhandled tag: {other.tag}");
                break;
        }
    }

    private void DestroyComponentWithAnimation()
    {
        Animator anim = this.gameObject.GetComponent<Animator>();
        if (anim != null)
            anim.SetTrigger("onEnemyDeath");

        if (_audioSource != null)
            _audioSource.Play();

        _isDestroyed = true;
        _speed = 1f;

        Destroy(this.gameObject, anim.GetCurrentAnimatorStateInfo(0).length);
    }

    void FireLaser()
    {
        _fireRate = UnityEngine.Random.Range(3f, 7f);
        _canFire = Time.time + _fireRate;

        Quaternion rotation = Quaternion.identity;

        if (transform.rotation.eulerAngles.z == 0)
        {
            rotation.eulerAngles = new Vector3(0, 0, 180);
        }
        else
        {
            float z = transform.rotation.eulerAngles.z;
            float newAngle = z > 0 ? 180 + z : 180 - z;
            rotation.eulerAngles = new Vector3(0, 0, newAngle);
        }

        GameObject laser = Instantiate(_laserPrefab, transform.position, rotation);
        laser.tag = "Enemy_Projectile";

    }

    private void OnDisable()
    {
        OnWillDestroy?.Invoke();
    }
}
