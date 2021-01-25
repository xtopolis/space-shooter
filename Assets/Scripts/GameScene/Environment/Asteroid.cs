using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Asteroid : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject _explosionPrefab = null;
    [SerializeField] private GameObject _asteroidPrefab = null;
    [Header("Config")]
    [SerializeField] private float _health = 5f;
    [SerializeField] private float _rotateSpeed = 0f;

    public static Action AsteroidDestroyed;
    // Start is called before the first frame update
    void Start()
    {
        if (_explosionPrefab == null)
            Debug.LogError("_explosionPrefab is null");

        if (_asteroidPrefab == null)
            Debug.LogError("_asteroidPrefab is null");

        if (_rotateSpeed == 0)
            Debug.LogWarning("_rotateSpeed is 0");
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate on Zed
        transform.Rotate(Vector3.forward * _rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Projectile_Laser") == false)
            return;
        Destroy(collision.gameObject);

        if(_health > 0)
            TakeDamage();
    }

    void TakeDamage()
    {
        _health -= 1;
        
        if(_health <= 0)
        {
            StartCoroutine(AsteroidDestroyedCleanup());
            GameObject explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Destroy(explosion, 1f);
        }
    }

    IEnumerator AsteroidDestroyedCleanup()
    {
        yield return new WaitForSeconds(0.25f);
        AsteroidDestroyed?.Invoke();
        Destroy(this.gameObject);
    }
}
