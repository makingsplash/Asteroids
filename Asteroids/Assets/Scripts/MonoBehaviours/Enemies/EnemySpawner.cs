using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private static byte _enemiesSpawned;
    public static byte EnemiesSpawned
    {
        get
        {
            return _enemiesSpawned;
        }
        set
        {
            _enemiesSpawned = value;
            UIManager.Instance.ResizeWaveBarMaxValue();
        }
    }

    private static byte _enemiesKilled;
    public static byte EnemiesKilled 
    {
        get
        {
            return _enemiesKilled;
        }
        private set
        {
            _enemiesKilled = value;
            UIManager.Instance.DecreaseWaveBarCurrentValue();
        } 
    }

    [Header("EnemyWaves")]
    [SerializeField] private WavesOfEmemies_SO _enemyWaves;
    [Header("Meteorites")]
    [SerializeField] private GameObject _baseMeteorite;

    private static bool _gameOver = false;

    private float _camOrtSize;
    private float _camAspect;

    private byte _currentWave = 0;

    private GameObject _UFOLaserPool;


    private void OnEnable()
    {
        BaseEnemy.EnemyKilled += CheckForNextWave;

        _camOrtSize = Camera.main.orthographicSize;
        _camAspect = Camera.main.aspect;

        _enemiesSpawned = 0;
        _enemiesKilled = 0;
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
            foreach (WavesOfEmemies_SO.MeteoritesInfo meteoritesInfo in wave.meteoriteTypes)
            {
                //StartCoroutine(StartSpawnMeteorites(
                //    meteoritesInfo.Amount, meteoritesInfo.Frequency, meteoritesInfo.Prefab));
                StartCoroutine(StartSpawnMeteorites(
                    meteoritesInfo.Amount, meteoritesInfo.Frequency, meteoritesInfo.meteoriteInfo));
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

            UIManager.Instance.ResizeWaveBarMaxValue();
            UIManager.Instance.ChangeWaveCounter(_currentWave);
        }
        else
            SceneManager.Instance.GameWin();

        yield return null;
    }

    private IEnumerator StartSpawnMeteorites(byte amount, float frequency, MeteoriteType_SO metInfo)
    {
        WaitForSeconds wait = new WaitForSeconds(frequency);
        EnemiesSpawned += amount;
        
        for (byte i = 0; i < amount; i++)
        {
            if(!_gameOver)
            {
                GameObject newGO = Instantiate(_baseMeteorite);
                newGO.transform.position = new Vector2((Random.Range(0, 2) * 2 - 1) * _camOrtSize * _camAspect - 1.5f, Random.Range(-_camOrtSize, _camOrtSize));
                newGO.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
                newGO.transform.localScale = metInfo.transform.localScale;
                newGO.GetComponent<SpriteRenderer>().sprite = metInfo.sprite;
                newGO.GetComponent<PolygonCollider2D>().points = metInfo.polygonCollider2d.points;

                Meteorite GOmetScr = newGO.GetComponent<Meteorite>();
                GOmetScr.Speed = metInfo.speed;
                GOmetScr.ScorePoints = metInfo.scorePoints;
                GOmetScr.SmallerMeteoritesInfo = metInfo.smallerMeteoritesSO;
            }

            yield return wait;
        }
    }

    private IEnumerator StartSpawnUfos(byte amount, float frequency, GameObject prefab)
    {
        EnemiesSpawned += amount;
        for (byte i = 0; i < amount; i++)
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
        EnemiesKilled++;
        if (EnemiesKilled == EnemiesSpawned)
        {
            EnemiesKilled = 0;
            EnemiesSpawned = 0;

            NextWave();
        }
    }
}
