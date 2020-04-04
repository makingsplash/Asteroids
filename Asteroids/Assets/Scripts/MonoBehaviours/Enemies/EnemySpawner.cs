using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private bool _gameOver = false;

    private float _camOrtSize;  // Camera.main.orthographicSize
    private float _camAspect;   // Camera.main.aspect

    [SerializeField] private EnemyWaves_SO _enemyWaves;

    [Header("Prefabs")]
    [SerializeField] private GameObject _bigMeteoritePrefab;
    [SerializeField] private GameObject _mediumMeteoritePrefab;
    [SerializeField] private GameObject _littleMeteoritePrefab;

    [SerializeField] private GameObject _ufoPrefab;
    [SerializeField] private GameObject _UFOLaserPoolPrefab;


    private void OnEnable()
    {
        BaseEnemy.OnNoOtherEnemies += OnGameOver;

        _camOrtSize = Camera.main.orthographicSize;
        _camAspect = Camera.main.aspect;

        Instantiate(_UFOLaserPoolPrefab);
    }
    private void OnDisable()
    {
        BaseEnemy.OnNoOtherEnemies += OnGameOver;
    }

    void OnGameOver() => _gameOver = true;

    /*
     * Использовать для спавна корутину, которая будет ждать очистки предыдущей волны
     * И потом продолжать свою работу(спавнить новую)
     * 
     * 
     * 
     */

    public void SpawnWave()
    {
        //StartSpawnBigMeteorites();
        //StartSpawnMediumMeteorites();
        //StartSpawnLittleMeteorites();
        //StartSpawnUFO();
    }

    // Написать общую функцию для именно спавна объекта
    private IEnumerator SpawnEnemy(int amount, float spawnRate, GameObject prefab, Vector2 position, Quaternion rotation)
    {
        WaitForSeconds frequency = new WaitForSeconds(10 / spawnRate);

        for (int i = 0; i < amount; i++)
        {
            if (!_gameOver)
            {
                Instantiate(prefab, position, rotation);
                yield return frequency;
            }
            else yield break;
        }
    }

    // Переписать в более обобщённую для метеоритов
    //public void StartSpawnBigMeteorites()
    //{
    //    int amount = _enemyWaves.EnemyWaves[0].BigMeteoriteAmount;
    //    if(amount > 0)
    //    {
    //        StartCoroutine(SpawnEnemy(
    //            amount, _enemyWaves.EnemyWaves[0].MeteoritesSpawnRate,
    //            _bigMeteoritePrefab,
    //            new Vector2((Random.Range(0, 2) * 2 - 1) * _camOrtSize * _camAspect - 1, Random.Range(-_camOrtSize, _camOrtSize)),
    //            Quaternion.Euler(0, 0, Random.Range(0, 180))));
    //    }
    //}

    //public void StartSpawnMediumMeteorites()
    //{
    //    int amount = _enemyWaves.EnemyWaves[0].MediumMeteoriteAmount;
    //    if (amount > 0)
    //    {
    //        StartCoroutine(SpawnEnemy(
    //            amount, _enemyWaves.EnemyWaves[0].MeteoritesSpawnRate,
    //            _mediumMeteoritePrefab,
    //            new Vector2((Random.Range(0, 2) * 2 - 1) * _camOrtSize * _camAspect - 1, Random.Range(-_camOrtSize, _camOrtSize)),
    //            Quaternion.Euler(0, 0, Random.Range(0, 180))));
    //    }
    //}

    //public void StartSpawnLittleMeteorites()
    //{
    //    int amount = _enemyWaves.EnemyWaves[0].LittleMeteoriteAmount;
    //    if (amount > 0)
    //    {
    //        StartCoroutine(SpawnEnemy(
    //            amount, _enemyWaves.EnemyWaves[0].MeteoritesSpawnRate,
    //            _littleMeteoritePrefab,
    //            new Vector2((Random.Range(0, 2) * 2 - 1) * _camOrtSize * _camAspect - 1, Random.Range(-_camOrtSize, _camOrtSize)),
    //            Quaternion.Euler(0, 0, Random.Range(0, 180))));
    //    }
    //}

    //public void StartSpawnUFO()
    //{
    //    int amount = _enemyWaves.EnemyWaves[0].UfoAmount;
    //    if(amount > 0)
    //    {
    //        StartCoroutine(SpawnEnemy(
    //            amount, _enemyWaves.EnemyWaves[0].UfoSpawnRate,
    //            _ufoPrefab,
    //            new Vector2((Random.Range(0, 2) * 2 - 1) * _camOrtSize * _camAspect, _camOrtSize - 1),
    //             Quaternion.identity));
    //    }
    //}   
}
