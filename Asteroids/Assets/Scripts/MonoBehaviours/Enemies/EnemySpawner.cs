﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private EnemyWaves_SO _enemyWaves;

    [Header("Prefabs")]
    [SerializeField] private GameObject _UFOLaserPoolPrefab;

    private bool _gameOver = false;

    private float _camOrtSize;
    private float _camAspect;

    private int _currentWave = 0;
    private int _enemiesToSpawn;
    private int _enemiesKilled;


    private void OnEnable()
    {
        BaseEnemy.EnemyKilled += CheckForNextWave;

        _camOrtSize = Camera.main.orthographicSize;
        _camAspect = Camera.main.aspect;

        Instantiate(_UFOLaserPoolPrefab);

    }
    private void OnDisable()
    {
        BaseEnemy.EnemyKilled -= CheckForNextWave;
    }

    //\\ 
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            NextWave();
    }

    private IEnumerator SpawnWave()
    {
        if (_currentWave < _enemyWaves.WavesArray.Length)
        {
            EnemyWaves_SO.Wave wave = _enemyWaves.WavesArray[_currentWave];

            // Spawn meteorites
            foreach (EnemyWaves_SO.EnemySpawnInfo meteoriteInfo in wave.meteoriteTypes)
            {
                StartCoroutine(SpawnEnemy(
                    meteoriteInfo.Amount, meteoriteInfo.SpawnRate, meteoriteInfo.Prefab,
                    new Vector2((Random.Range(0, 2) * 2 - 1) * _camOrtSize * _camAspect - 1, Random.Range(-_camOrtSize, _camOrtSize)),
                    Quaternion.Euler(0, 0, Random.Range(0, 180))));

                _enemiesToSpawn += meteoriteInfo.Amount;
            }
        
            // Spawn Ufos
            if (wave.ufoInfo.Amount > 0)
            {
                //\\
                //\\ Не сразу создавать пул лазеров?

                StartCoroutine(SpawnEnemy(
                    wave.ufoInfo.Amount, wave.ufoInfo.SpawnRate, wave.ufoInfo.Prefab,
                    new Vector2((Random.Range(0, 2) * 2 - 1) * _camOrtSize * _camAspect, _camOrtSize - 1),
                    Quaternion.identity));

                _enemiesToSpawn += wave.ufoInfo.Amount;
            }

            _currentWave++;
        }
        else
            SceneManager.Instance.GameWin();

        yield return null;
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

    public void NextWave() => StartCoroutine(SpawnWave());

    private void CheckForNextWave()
    {
        _enemiesKilled++;
        if (_enemiesKilled == _enemiesToSpawn)
        {
            _enemiesKilled = 0;
            _enemiesToSpawn = 0;

            NextWave();
        }
    }
}
