﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPool : MonoBehaviour
{
    public Queue<GameObject> Pool;

    [SerializeField] private readonly int _laserPoolSize = 40;
    [SerializeField] private GameObject _laserPrefab;
    


    void Awake()
    {
        Laser.ReturnToPool += ReturnLaserToPool;

        FillLaserPool();
    }

    public void LaunchLaser(Vector2 launchPosition, Quaternion rotation)
    {
        if (Pool.Count > 0)
        {
            GameObject laser = Pool.Dequeue();
            laser.SetActive(true);
            laser.transform.position = launchPosition;

            laser.transform.rotation = rotation;
        }
        else
            Debug.LogError("В пуле нет свободных лазеров для выстрела");
    }

    void FillLaserPool()
    {
        if (Pool == null)
        {
            // Заполняем ObjectPool лазеров
            Pool = new Queue<GameObject>();
            for (int i = 0; i < _laserPoolSize; i++)
            {
                GameObject laserObj = GameObject.Instantiate(_laserPrefab);
                laserObj.transform.SetParent(transform);
                laserObj.SetActive(false);
                Pool.Enqueue(laserObj);
            }
        }
    }

    void ReturnLaserToPool(GameObject laser)
    {
        Pool.Enqueue(laser);
    }
}
