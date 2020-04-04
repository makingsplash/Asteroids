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
    [SerializeField] private GameObject _UFOLaserPoolPrefab;


    private void OnEnable()
    {
        //BaseEnemy.OnNoOtherEnemies += OnGameOver;
        BaseEnemy.OnNoOtherEnemies += NextWave;

        _camOrtSize = Camera.main.orthographicSize;
        _camAspect = Camera.main.aspect;

        Instantiate(_UFOLaserPoolPrefab);
    }
    private void OnDisable()
    {
        //BaseEnemy.OnNoOtherEnemies -= OnGameOver;
        BaseEnemy.OnNoOtherEnemies -= NextWave;
    }

    //void OnGameOver() => _gameOver = true; /// ВРОДЕ ТЕПЕРЬ ЛИШНЕЕ, КОНТРОЛИМ В SCENEMANAGER


    public IEnumerator SpawnWave()
    {
        foreach(EnemyWaves_SO.Wave wave in _enemyWaves.WavesArray)
        {
            Debug.Log("Спавним метеориты");

            // Spawn meteorites
            foreach (EnemyWaves_SO.EnemySpawnInfo meteoriteInfo in wave.meteoriteTypes)
            {
                StartCoroutine(SpawnEnemy(
                    meteoriteInfo.Amount, meteoriteInfo.SpawnRate, meteoriteInfo.Prefab,
                    new Vector2((Random.Range(0, 2) * 2 - 1) * _camOrtSize * _camAspect - 1, Random.Range(-_camOrtSize, _camOrtSize)),
                    Quaternion.Euler(0, 0, Random.Range(0, 180))));
            }

            Debug.Log("Спавним нло");
            // Spawn Ufos
            if (wave.ufoInfo.Amount > 0)
            {
                //\\
                //\\ Не сразу создавать пул лазеров?

                StartCoroutine(SpawnEnemy(
                    wave.ufoInfo.Amount, wave.ufoInfo.SpawnRate, wave.ufoInfo.Prefab,
                    new Vector2((Random.Range(0, 2) * 2 - 1) * _camOrtSize * _camAspect, _camOrtSize - 1),
                    Quaternion.identity));
            }

            Debug.Log("Волна заспавнена, жду следующего вызова");

            yield return null;
        }

        SceneManager.Instance.GameWin();
    }

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

    private void NextWave()
    {
        Debug.Log("Следующая волна?");
        SpawnWave();
    }
}
