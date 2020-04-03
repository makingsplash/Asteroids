using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private bool _gameOver = false;

    private float _camOrtSize;  // Camera.main.orthographicSize
    private float _camAspect;   // Camera.main.aspect

    [Header("Meteorites")]
    [SerializeField] private GameObject _bigMeteoritePrefab;
    [Tooltip("Количество больших метеоритов, которое может быть заспавнено за игру")]
    [SerializeField] [Range(0, 10)] private int _bigMeteoritesAmount = 4;

    [Tooltip("Таймер, по истечению которого может заспавниться большой метеорит")]
    [SerializeField] [Range(0, 10)] private float _bigMeteoritesSpawnRate;

    [Header("UFO")]
    [SerializeField] private GameObject _ufoPrefab;

    [SerializeField] private GameObject _UFOLaserPoolPrefab;

    [Tooltip("Количество НЛО, которое может быть заспавнено за игру")]
    [SerializeField] [Range(0, 10)] private int _ufoAmount;

    [Tooltip("Вероятность того, что НЛО будет заспавнен по истечению таймера")]
    [SerializeField] [Range(0, 100)] private int _ufoSpawnChance;

    [Tooltip("Таймер, по истечению которого может заспавниться НЛО")]
    [SerializeField] [Range(0, 10)] private float _ufoSpawnRate;



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

    public IEnumerator SpawnBigMeteorites()
    {
        while (!_gameOver && _bigMeteoritesAmount > 0)
        {
            _bigMeteoritesAmount--;

            // Спавн метеоритов с правой стороны
            if (_bigMeteoritesAmount % 2 == 0)
            {
                Instantiate(_bigMeteoritePrefab,
                    new Vector2(_camOrtSize * _camAspect - 1, Random.Range(-_camOrtSize, _camOrtSize)),
                    Quaternion.Euler(0, 0, Random.Range(0, 180)));
            }
            // Спавн метеоритов с левой стороны
            else
            {
                Instantiate(_bigMeteoritePrefab,
                    new Vector2(-_camOrtSize * _camAspect + 1, Random.Range(-_camOrtSize, _camOrtSize)),
                    Quaternion.Euler(0, 0, Random.Range(0, 180)));
            }

            yield return new WaitForSeconds(1 / _bigMeteoritesSpawnRate);
        }
    }

    public IEnumerator SpawnUFO()
    {
        while (!_gameOver && _ufoAmount > 0)
        {
            if (Random.Range(0, 101) <= _ufoSpawnChance)
            {
                _ufoAmount--;

                GameObject ufoObj = Instantiate(_ufoPrefab,
                    // Нло может появиться и справа, и слева с одинаковым шансом
                    new Vector2((Random.Range(0, 2) * 2 - 1) * _camOrtSize * _camAspect, _camOrtSize - 1),
                    Quaternion.identity);
            }
            yield return new WaitForSeconds(1 /_ufoSpawnRate);
        }
    }   
}
