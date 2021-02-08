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
    [SerializeField] private bool _isSpawning = true;
    [SerializeField] private PowerUpsSO _powerUpsPrefabs = null;
    [SerializeField] private CollectablesSO _collectablesPrefabs = null;

    public static Action GameOver;

    private WaveManager _waveManager = null;

    // Start is called before the first frame update
    void Start()
    {
        Wat();
        _waveManager = GetComponent<WaveManager>();
        _waveManager.OnAllWavesDestroyed += JobsDone;

        if (_waveManager == null)
            Debug.LogError("_waveManager is null");

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
        StartCoroutine(SpawnGameObjectRandomly(true, new List<GameObject> {
            _collectablesPrefabs.ammo, _collectablesPrefabs.health
        }, _collectableSpawnRate));
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
        while (condition)
        {
            Vector3 nextPos = GameDataSO.RandomSpawnPositionNearBounds();
            GameObject prefab = randomPrefab[UnityEngine.Random.Range(0, randomPrefab.Count)];

            GameObject gameObj = Instantiate(prefab, nextPos, Quaternion.identity);

            if (parent != null)
                gameObj.transform.parent = parent;

            yield return new WaitForSeconds(interval);
        }
    }

    IEnumerator SpawnCollectables()
    {
        while (_isSpawning)
        {
            Vector3 nextPos = GameDataSO.RandomSpawnPositionNearBounds();
            GameObject prefab = PrefabFrom(CollectableItemFromTable());

            GameObject gameObj = Instantiate(prefab, nextPos, Quaternion.identity);

            if (_enemyContainer.transform.parent != null)
                gameObj.transform.parent = _enemyContainer.transform.parent;

            yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 5f));
        }
    }

    public void StopSpawning()
    {
        _isSpawning = false;
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnCollectables());
        _waveManager.StartWaves();
    }

    public void JobsDone()
    {
        print("Spawn manager:: Jobs done");
    }

    private Collectable.CollectableType CollectableItemFromTable()
    {
        int randy = UnityEngine.Random.Range(0, 100);

        if (randy <= 60)
        {
            return Collectable.CollectableType.AMMO;
        }
        else if (randy <= 90)
        {
            return Collectable.CollectableType.ANTIMATTER;
        }
        else
        {
            return Collectable.CollectableType.HEALTH;
        }
    }

    private GameObject PrefabFrom(Collectable.CollectableType collectibleType)
    {
        switch (collectibleType)
        {
            case Collectable.CollectableType.AMMO:
                return _collectablesPrefabs.ammo;
            case Collectable.CollectableType.HEALTH:
                return _collectablesPrefabs.health;
            case Collectable.CollectableType.ANTIMATTER:
                return _collectablesPrefabs.antiMatter;
            default:
                Debug.LogError($"Unhandled collectibleType: {collectibleType}");
                return new GameObject();
        }
    }

    void Wat()
    {
        Dictionary<Collectable.CollectableType, int> debug = new Dictionary<Collectable.CollectableType, int> {
            [Collectable.CollectableType.AMMO] = 0,
            [Collectable.CollectableType.HEALTH] = 0,
            [Collectable.CollectableType.ANTIMATTER] = 0
        };

        for(int i = 0; i< 10000; i++)
        {
            Collectable.CollectableType c = CollectableItemFromTable();
            debug[c] = debug[c] + 1;
        }

        print($"AMMO: {debug[Collectable.CollectableType.AMMO]} | ANTIMATTER: {debug[Collectable.CollectableType.ANTIMATTER]} | HEALTH: {debug[Collectable.CollectableType.HEALTH]}");
    }
}
