using System;
using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour, IDamageable, IDamager
{
    public static Action OnNoOtherEnemies;

    public int ScorePoints;

    [SerializeField] protected float _speed;
    [SerializeField] private AudioClip _dieSound;

    private static int _enemyCount = 0;


    private void Awake()
    {
        _enemyCount++;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        IDamageable player = collision.gameObject.GetComponent<IDamageable>();
        if (player != null)
            DoDamage(player);
    }

    public virtual void TakeDamage()
    {
        _enemyCount--;
        if (_enemyCount == 0)
            OnNoOtherEnemies();

        AudioManager.Instance.PlayOneSound(_dieSound);
    }

    public void DoDamage(IDamageable damageable)
    {
        damageable.TakeDamage();
    }
}
