using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class PowerUp : MonoBehaviour
{
    [SerializeField] protected GameObject _particleEffect;
    [SerializeField] protected float _fallSpeed = 0;
    public abstract PowerUpType powerUpType { get; }

    public enum PowerUpType
    {
        TRIPLE_SHOT,
        SPEED_BOOST,
        SHIELD
    }

    // Start is called before the first frame update
    void Start()
    {
        if (_particleEffect == null)
            Debug.LogError("_particleEffect is null");

        if (_fallSpeed == 0)
            Debug.LogWarning("_speed is 0");

        transform.position = GameDataSO.RandomSpawnPositionNearBounds();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _fallSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null)
            return;

        if (collision.CompareTag("Player") == false)
            return;

        Player player = collision.GetComponent<Player>();

        if (player == null)
            return;

        
        if (powerUpType != PowerUpType.SHIELD)
            Instantiate(_particleEffect, transform.position, Quaternion.identity);
        
        player.CollectPowerUp(powerUpType);
        Destroy(this.gameObject);
    }
}
