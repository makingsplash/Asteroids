using System;
using UnityEngine;

[CreateAssetMenu(fileName = "WavesOfEnemy", menuName = "New Waves", order = 51)]
public class WavesOfEmemies_SO : ScriptableObject
{
    public Wave[] WavesArray;

    [Serializable]
     public class BaseEnemyInfo
     {
        public byte Amount;
        public float Frequency;
     }

    [Serializable]
    public class UfosInfo : BaseEnemyInfo
    {
        public GameObject Prefab;
        public GameObject LaserPoolPrefab;
    }

    [Serializable]
    public class MeteoritesInfo : BaseEnemyInfo
    {
        public MeteoriteType_SO meteoriteInfo;
    }

    [Serializable]
    public class Wave
    {
        [SerializeField] private string _name;

        [Header("Meteorites")]
        public MeteoritesInfo[] meteoriteTypes;

        [Header("Ufo")]
        public UfosInfo ufoInfo = new UfosInfo();
    }
}