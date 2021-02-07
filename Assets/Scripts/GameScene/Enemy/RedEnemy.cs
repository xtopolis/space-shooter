using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RedEnemy : MonoBehaviour, IDestroyable
{
    [Header("Prefabs")]
    [SerializeField] private GameObject _projectilePrefab = null;

    [Header("Config")]
    [SerializeField] private float _speed = 0;

    public event Action<int> killedByPlayer;

    private float _canFire = -1f;
    private float _fireRate = 3f;
    private bool _isDestroyed = false;

    // Side to Side fields
    private float _yStopPos = 0f;

    // Angler
    GameObject player = null;

    public event Action OnWillDestroy;

    // Start is called before the first frame update
    void Start()
    {
        if (_speed == 0)
            Debug.LogWarning("_speed is 0");

        transform.position = GameDataSO.RandomSpawnPositionNearBounds();
        _yStopPos = UnityEngine.Random.Range(GameDataSO.yRange - 2, GameDataSO.yRange - 4);


        player = GameObject.Find("Player");
        if (player == null)
            Debug.LogError("player is null");
    }

    // Update is called once per frame
    void Update()
    {
        Movement();

        if (transform.position.y > _yStopPos)
            return;
        
        // Don't fire until in position
        if (Time.time > _canFire)
            FireLaser();
    }

    void Movement()
    {
        if (transform.position.y >= _yStopPos)
        {
            MoveNormally();
            return;
        }

        LookAtPlayer();
    }

    void LookAtPlayer()
    {
        if (player == null)
            return;

        Vector3 direction = player.transform.position - this.transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x);
        float deg = angle * Mathf.Rad2Deg;

        if (player.transform.position.y <= transform.position.y)
        {
            if (deg < 0)
            {
                deg = 90 + deg;
            }
            else
            {
                deg = 90 - deg;
            }
        }
        else
        {
            if (deg > 0)
            {
                deg = 90 + deg;
            }
            else
            {
                deg = 90 - deg;
            }
        }

        Quaternion nextRotation = Quaternion.Euler(0f, 0f, deg);
        transform.rotation = Quaternion.Slerp(transform.rotation, nextRotation, 2f * Time.deltaTime);
    }

    void MoveNormally()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
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

        StartCoroutine(FireLasers(rotation));

    }

    IEnumerator FireLasers(Quaternion rotation)
    {
        yield return new WaitForSeconds(0.25f);

        for (int i = 0; i < 3; i++)
        {
            float randomX = UnityEngine.Random.Range(transform.position.x - 0.5f, transform.position.x + 0.5f);
            float randomY = UnityEngine.Random.Range(transform.position.y - 0.1f, transform.position.y + 0.1f);
            Vector3 offsetPosition = new Vector3(randomX, randomY, 0);

            GameObject laser = Instantiate(_projectilePrefab, offsetPosition, rotation);
            laser.tag = "Enemy_Projectile";
            yield return new WaitForSecondsRealtime(UnityEngine.Random.Range(0.1f, 0.25f));
        }
        StopCoroutine(FireLasers(Quaternion.identity));
    }

    private void OnDisable()
    {
        OnWillDestroy?.Invoke();
    }
}
