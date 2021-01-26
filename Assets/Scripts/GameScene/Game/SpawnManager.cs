using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SpawnManager : MonoBehaviour
{
    [Space(5)]
    [Header("Config")]
    [SerializeField] private float _enemySpawnRate = 0f;
    [SerializeField] private float _powerUpSpawnRate = 0f;
    [SerializeField] private float _collectableSpawnRate = 0f;
    // Prefabs
    [SerializeField] private GameObject _enemyContainer = null;
    [SerializeField] private GameObject _enemyPrefab = null;
    [SerializeField] private bool _isSpawning = false;
    [SerializeField] private PowerUpsSO _powerUpsPrefabs = null;
    [SerializeField] private CollectablesSO _collectablesPrefabs = null;

    public static Action GameOver;

    // Start is called before the first frame update
    void Start()
    {
        if (_enemySpawnRate == 0)
            Debug.LogWarning("_enemySpawnRate is 0");

        if (_powerUpSpawnRate == 0)
            Debug.LogWarning("_powerUpSpawnRate is 0");

        if (_collectableSpawnRate == 0)
            Debug.LogWarning("_powerUpSpawnRate is 0");

        if (_enemyPrefab == null)
            Debug.LogError("_enemyPrefab is null");
        
        if (_enemyContainer == null)
            Debug.LogError("_enemyContainer is null");

        if (_powerUpsPrefabs == null)
            Debug.LogError("_powerUpsPrefabs is null");

        Player.playerDied += StopSpawning;
        Asteroid.AsteroidDestroyed += StartSpawning;

        // Spawn all the time, in case of start screen
        StartCoroutine(SpawnGameObjectRandomly(true, new List<GameObject> { _collectablesPrefabs.ammo }, _collectableSpawnRate));
    }

    private void OnDisable()
    {
        Player.playerDied -= StopSpawning;
        Asteroid.AsteroidDestroyed -= StartSpawning;
    }

    void StartSpawnerCoRoutines()
    {
        // Enemy
        StartCoroutine(SpawnGameObjectRandomly(_isSpawning, new List<GameObject> { _enemyPrefab }, _enemySpawnRate, _enemyContainer.transform.parent));

        // RandomPowerUp
        StartCoroutine(SpawnGameObjectRandomly(_isSpawning, new List<GameObject> { 
            _powerUpsPrefabs.tripleShot, _powerUpsPrefabs.shield, _powerUpsPrefabs.speedBoost
        }, _powerUpSpawnRate));
    }
    
    IEnumerator SpawnGameObjectRandomly(bool condition, List<GameObject> randomPrefab, float interval, Transform parent = null, bool forceSpawn = false)
    {
        while(condition)
        {
            Vector3 nextPos = GameDataSO.RandomSpawnPositionNearBounds();
            GameObject prefab = randomPrefab[UnityEngine.Random.Range(0, randomPrefab.Count)];

            GameObject gameObj = Instantiate(prefab, nextPos, Quaternion.identity);

            if (parent != null)
                gameObj.transform.parent = parent;
            
            yield return new WaitForSeconds(interval);
        }
    }

    public void StopSpawning()
    {
        _isSpawning = false;
    }

    public void StartSpawning()
    {
        _isSpawning = true;
        StartSpawnerCoRoutines();
    }
}
