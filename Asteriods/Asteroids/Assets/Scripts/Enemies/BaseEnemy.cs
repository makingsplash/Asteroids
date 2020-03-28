using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour, IDamageable, IDamager
{
    public static Action OnNoOtherEnemies;

    private static int _enemyAmount = 0;


    private void Awake()
    {
        _enemyAmount++;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        IDamageable player = collision.gameObject.GetComponent<IDamageable>();
        if (player != null)
            DoDamage(player);
    }

    public virtual void GotDamage()
    {
        _enemyAmount--;
        if (_enemyAmount == 0)
            OnNoOtherEnemies();
    }

    public void DoDamage(IDamageable damageable)
    {
        damageable.GotDamage();
    }
}
