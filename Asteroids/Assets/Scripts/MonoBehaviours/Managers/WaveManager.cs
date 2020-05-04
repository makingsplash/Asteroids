using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
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

    [Header("Rocket")]
    [SerializeField] private RocketController _rocket;

    [Header("Enemies")]
    [SerializeField] private ObjectPool _warningsPool;

    [Header("EnemyWaves")]
    [SerializeField] private WavesOfEmemies_SO _enemyWaves;
    private byte _currentWave = 0;
    
    [Header("Meteorites")]
    [SerializeField] private ObjectPool _meteoritesPool;


    private ObjectPool _ufoLaserPool;
    private float _camOrtSize;
    private float _camAspect;
    private static bool _gameOver = false;


    private void OnEnable()
    {
        BaseEnemy.EnemyKilled += CheckForNextWave;

        _camOrtSize = CameraInfo.Instance.CamOrtSize;
        _camAspect = CameraInfo.Instance.CamAspect;

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
                    GameObject _UFOLaserPoolObj = Instantiate(wave.ufoInfo.UfoLaserPool);
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
                Vector2 spawnPos = RandomOutsideSpawnPosition();
                Vector2 lookTo = new Vector2(
                    Random.Range(-_camAspect * _camOrtSize / 2, _camAspect * _camOrtSize / 2),
                    Random.Range(-_camOrtSize / 2, _camOrtSize / 2));
                Vector2 direction = lookTo - spawnPos;
                float rotationAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;

                GameObject meteorite = _meteoritesPool.SpawnObject(spawnPos, rotationAngle);
                meteorite.transform.localScale = metInfo.transform.localScale;
                meteorite.GetComponent<SpriteRenderer>().sprite = metInfo.sprite;
                meteorite.GetComponent<PolygonCollider2D>().points = metInfo.polygonCollider2d.points;

                AddWarning(meteorite);

                Meteorite metComponent = meteorite.GetComponent<Meteorite>();
                metComponent.Health = metInfo.health;
                metComponent.Speed = metInfo.speed;
                metComponent.HitScorePoints = metInfo.hitScorePoints;
                metComponent.DeathScorePoints = metInfo.deathScorePoints;
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
                    RandomOutsideSpawnPosition(),
                    Quaternion.identity);

                AddWarning(ufo);

                Ufo ufoComponent = ufo.GetComponent<Ufo>();
                ufoComponent.LaserPool = _ufoLaserPool;

                yield return wait;
            }
            else
                yield break;
        }
    }

    private void AddWarning(GameObject enemy)
    {
        EnemyWarning warning = _warningsPool.SpawnObject(Vector2.zero, 0).GetComponent<EnemyWarning>();
        warning.EnemyObject = enemy;
    }

    private Vector2 RandomOutsideSpawnPosition()
    {
        float spawnRadius = _camOrtSize * _camAspect * 1.4f;
        float spawnX = Random.Range(-spawnRadius, spawnRadius);
        float spawnY = Mathf.Sqrt((spawnRadius * spawnRadius) - (spawnX * spawnX));
        spawnY *= Random.Range(0, 2) * 2 - 1;

        Vector2 spawnPos = new Vector2(spawnX, spawnY);
        return spawnPos;
    }

    public IEnumerator NextWave()
    {
        yield return StartCoroutine(_rocket.UseScanner());
        StartCoroutine(SpawnWave());
    }

    private void CheckForNextWave()
    {
        EnemiesKilled++;
        if (EnemiesKilled == EnemiesSpawned)
        {
            EnemiesKilled = 0;
            EnemiesSpawned = 0;

            StartCoroutine(NextWave());
        }
    }
}
