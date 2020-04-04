using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyWaves", menuName = "New EnemyWaves", order = 51)]
public class EnemyWaves_SO : ScriptableObject
{
    public Wave[] WavesArray;

    [Serializable]
     public class EnemySpawnInfo
     {
        public GameObject Prefab;
        public int Amount;

        [Tooltip("Frequency = 10 / SpawnRate")]
        public float SpawnRate;
     }

    [Serializable]
    public class Wave
    {
        [SerializeField] private string _name;

        [Header("Meteorites")]
        public EnemySpawnInfo[] meteoriteTypes;

        [Header("Ufo")]
        public EnemySpawnInfo ufoInfo = new EnemySpawnInfo();
    }
}