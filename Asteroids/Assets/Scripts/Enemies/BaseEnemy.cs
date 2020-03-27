using System;
using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour
{
    public static Action OnNoOtherEnemies;

    private static int _enemyAmount = 0;


    private void Awake()
    {
        _enemyAmount++;
    }

    protected void OnDisable()
    {
        _enemyAmount--;
        if (_enemyAmount == 0)
            OnNoOtherEnemies();
    }
}
