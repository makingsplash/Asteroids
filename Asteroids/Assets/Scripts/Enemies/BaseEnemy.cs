using System;
using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour, IDamageable, IDamager
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        IDamageable player = collision.gameObject.GetComponent<IDamageable>();
        if (player != null)
            DoDamage(player);
    }

    public abstract void GotDamage();

    public void DoDamage(IDamageable damageable)
    {
        damageable.GotDamage();
    }
}
