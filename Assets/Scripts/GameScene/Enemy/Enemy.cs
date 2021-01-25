using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject _laserPrefab = null;
    
    [Header("Config")]
    [SerializeField] private float _speed = 0;

    public static Action<int> killedByPlayer;

    private AudioSource _audioSource = null;

    private float _canFire = -1f;
    private float _fireRate = 3f;
    private bool _isDestroyed = false;


    // Start is called before the first frame update
    void Start()
    {
        _audioSource = transform.GetComponent<AudioSource>();

        if (_audioSource == null)
            Debug.LogWarning("_audioSource is null");

        if (_speed == 0)
            Debug.LogWarning("_speed is 0");

        transform.position = GameDataSO.RandomSpawnPositionNearBounds();
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
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y < -(GameDataSO.yRange + 5f)) // 5f buffer
            transform.position = GameDataSO.RandomSpawnPositionNearBounds();
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
        rotation.eulerAngles = new Vector3(0, 0, 180);
            
        GameObject laser = Instantiate(_laserPrefab, transform.position, rotation);
        laser.tag = "Enemy_Projectile";
            
    }
}
