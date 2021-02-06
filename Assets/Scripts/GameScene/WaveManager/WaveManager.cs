using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WaveManager : MonoBehaviour
{
    // Prefabs
    [SerializeField] private GameObject _enemyContainer = null;
    // Waves
    [Header("Wave System")]
    [SerializeField] private List<WaveSO> _waves = null;

    private int _currentWaveIndex = 0;
    private int _enemiesRemainingInCurrentWave = 0;

    public event Action OnAllWavesDestroyed;

    private void OnEnable()
    {
        if (_enemyContainer == null)
            Debug.LogError("Enemy Container is null");

        if (_waves.Count == 0)
            Debug.LogError("No waves in WaveManager");
    }

    public void StartWaves()
    {
        if (_waves.Count == 0)
        {
            Debug.LogError("No waves in WaveManager");
            return;
        }

        //_isSpawning = true;
        StartCoroutine(SpawnWaves());
    }

    // Events
    private void TrackEnemyDestroyed()
    {
        _enemiesRemainingInCurrentWave--;
    }

    IEnumerator SpawnWaves()
    {
        foreach (var wave in _waves)
        {
            _enemiesRemainingInCurrentWave = wave.enemies.Count;
            yield return SpawnWave(wave);
        }
    }

    IEnumerator SpawnWave(WaveSO wave)
    {

        foreach (var enemy in wave.enemies)
        {
            GameObject newEnemy = Instantiate(enemy, Vector3.zero, Quaternion.identity, _enemyContainer.transform);

            IDestroyable detroyable = newEnemy.GetComponent<IDestroyable>();
            if (detroyable != null)
                detroyable.OnWillDestroy += TrackEnemyDestroyed;

            switch (wave.spawnMethod)
            {
                case WaveSO.SpawnFormat.ALL_AT_ONCE:
                    yield return new WaitForSeconds(0.15f);
                    break;
                case WaveSO.SpawnFormat.SEQUENCE:
                    yield return new WaitForSeconds(1);
                    break;
            }
        }

        while(_enemiesRemainingInCurrentWave > 0)
        {
            yield return null;
        }

        _currentWaveIndex++;

        if (_currentWaveIndex >= _waves.Count)
        {
            //_isSpawning = false;
            StopCoroutine(SpawnWaves());
            OnAllWavesDestroyed?.Invoke();
        }
        yield return null;
    }
}
