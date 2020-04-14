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
    
    [Header("MeteoritesPool")]
    [SerializeField] private ObjectPool _meteoritesPool;
    
    private ObjectPool _ufoLaserPool;

    private static bool _gameOver = false;

    private float _camOrtSize;
    private float _camAspect;

    private byte _currentWave = 0;


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
                StartCoroutine(StartSpawnMeteorites(
                    meteoritesInfo.Amount, meteoritesInfo.Frequency, meteoritesInfo.meteoriteInfo));
            }
        
            // Spawn Ufos
            if (wave.ufoInfo.Amount > 0)
            {
                if (_ufoLaserPool == null)
                {
                    GameObject _UFOLaserPoolObj = Instantiate(wave.ufoInfo.LaserPoolPrefab);
                    _UFOLaserPoolObj.SetActive(true);

                    _ufoLaserPool = _UFOLaserPoolObj.GetComponent<ObjectPool>();
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
            if (!_gameOver)
            {
                GameObject meteorite = _meteoritesPool.SpawnObject(
                    new Vector2((Random.Range(0, 2) * 2 - 1) * _camOrtSize * _camAspect - 0.5f, // pick random screen side
                    Random.Range(-_camOrtSize, _camOrtSize)), // pick random heigth
                    Random.Range(0, 360));

                meteorite.transform.localScale = metInfo.transform.localScale;
                meteorite.GetComponent<SpriteRenderer>().sprite = metInfo.sprite;
                meteorite.GetComponent<PolygonCollider2D>().points = metInfo.polygonCollider2d.points;

                Meteorite metComponent = meteorite.GetComponent<Meteorite>();
                metComponent.Speed = metInfo.speed;
                metComponent.ScorePoints = metInfo.scorePoints;
                metComponent.SmallerMeteoritesInfo = metInfo.smallerMeteoritesSO;
            }
            else
                yield break;

            yield return wait;
        }
    }

    private IEnumerator StartSpawnUfos(byte amount, float frequency, GameObject prefab)
    {
        WaitForSeconds wait = new WaitForSeconds(frequency);
        EnemiesSpawned += amount;
        for (byte i = 0; i < amount; i++)
        {
            if (!_gameOver)
            {
                GameObject ufo = Instantiate(prefab,
                    new Vector2((Random.Range(0, 2) * 2 - 1) * _camOrtSize * _camAspect, _camOrtSize - 1),
                    Quaternion.identity);

                UFO ufoComponent = ufo.GetComponent<UFO>();
                ufoComponent.LaserPool = _ufoLaserPool;

                yield return wait;
            }
            else
                yield break;
        }
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
