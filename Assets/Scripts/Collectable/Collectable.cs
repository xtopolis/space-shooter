using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Collectable : MonoBehaviour
{
    [SerializeField] protected float _fallSpeed = 0;
    public abstract CollectableType collectableType { get; }

    public enum CollectableType
    {
        AMMO,
        HEALTH,
        ANTIMATTER
    }

    // Start is called before the first frame update
    void Start()
    {
        if (_fallSpeed == 0)
            Debug.LogWarning("_speed is 0");

        transform.position = GameDataSO.RandomSpawnPositionNearBounds();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _fallSpeed * Time.deltaTime, Space.Self);

        if (collectableType == CollectableType.AMMO)
            transform.Rotate(0f, 50f * Time.deltaTime, 0f);
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

        player.CollectCollectable(collectableType);
        Destroy(this.gameObject);
    }
}
