using System;
using UnityEngine;

[CreateAssetMenu(fileName = "WavesOfEnemy", menuName = "New Waves", order = 51)]
public class WavesOfEmemies_SO : ScriptableObject
{
    public Wave[] WavesArray;

    [Serializable]
     public class BaseEnemyInfo
     {
        public GameObject Prefab;
        public byte Amount;
        public float Frequency;
     }

    [Serializable]
    public class UfoInfo : BaseEnemyInfo
    {
        public GameObject LaserPoolPrefab;
    }

    [Serializable]
    public class Wave
    {
        [SerializeField] private string _name;

        [Header("Meteorites")]
        public BaseEnemyInfo[] meteoriteTypes;

        [Header("Ufo")]
        public UfoInfo ufoInfo = new UfoInfo();
    }
}