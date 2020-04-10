using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static int EnemiesToSpawn;

    [SerializeField] private WavesOfEmemies_SO _enemyWaves;

    private static bool _gameOver = false;

    private float _camOrtSize;
    private float _camAspect;

    private int _currentWave = 0;
    private int _enemiesKilled;

    private GameObject _UFOLaserPool;


    private void OnEnable()
    {
        BaseEnemy.EnemyKilled += CheckForNextWave;

        _camOrtSize = Camera.main.orthographicSize;
        _camAspect = Camera.main.aspect;

        EnemiesToSpawn = 0;
    }
    private void OnDisable()
    {
        BaseEnemy.EnemyKilled -= CheckForNextWave;
    }

    private IEnumerator SpawnWave()
    {
        if (_currentWave < _enemyWaves.WavesArray.Length)
        {
            WavesOfEmemies_SO.Wave wave = _enemyWaves.WavesArray[_currentWave];

            // Spawn meteorites
            foreach (WavesOfEmemies_SO.BaseEnemyInfo meteoriteInfo in wave.meteoriteTypes)
            {
                StartCoroutine(StartSpawnMeteorites(
                    meteoriteInfo.Amount, meteoriteInfo.Frequency, meteoriteInfo.Prefab));
            }
        
            // Spawn Ufos
            if (wave.ufoInfo.Amount > 0)
            {
                if (_UFOLaserPool == null)
                {
                    _UFOLaserPool = Instantiate(wave.ufoInfo.LaserPoolPrefab);
                    _UFOLaserPool.SetActive(true);
                }

                StartCoroutine(StartSpawnUfos(
                    wave.ufoInfo.Amount, wave.ufoInfo.Frequency, wave.ufoInfo.Prefab));
            }

            _currentWave++;

            Debug.Log("Волна " + _currentWave + " из " + _enemyWaves.WavesArray.Length);
        }
        else
            SceneManager.Instance.GameWin();

        yield return null;
    }

    private IEnumerator StartSpawnMeteorites(int amount, float frequency, GameObject prefab)
    {
        EnemiesToSpawn += amount;
        for (int i = 0; i < amount; i++)
        {
            yield return StartCoroutine(SpawnEnemy(frequency, prefab,
                new Vector2((Random.Range(0, 2) * 2 - 1) * _camOrtSize * _camAspect - 1.5f, Random.Range(-_camOrtSize, _camOrtSize)),
                Quaternion.Euler(0, 0, Random.Range(0, 360))));
        }
    }

    private IEnumerator StartSpawnUfos(int amount, float frequency, GameObject prefab)
    {
        EnemiesToSpawn += amount;
        for (int i = 0; i < amount; i++)
        {
            yield return StartCoroutine(SpawnEnemy(frequency, prefab,
                new Vector2((Random.Range(0, 2) * 2 - 1) * _camOrtSize * _camAspect, _camOrtSize - 1),
                    Quaternion.identity));
        }
    }

    public static IEnumerator SpawnEnemy(float frequency, GameObject prefab, Vector2 position, Quaternion rotation)
    {
        WaitForSeconds spawnFrequency = new WaitForSeconds(frequency);

        if (!_gameOver)
        {
            Instantiate(prefab, position, rotation);
            yield return spawnFrequency;
        }
        else yield break;
    }

    public void NextWave() => StartCoroutine(SpawnWave());

    private void CheckForNextWave()
    {
        _enemiesKilled++;
        if (_enemiesKilled == EnemiesToSpawn)
        {
            _enemiesKilled = 0;
            EnemiesToSpawn = 0;

            NextWave();
        }
    }
}
