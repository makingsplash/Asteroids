using System;
using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour, IDamageable, IDamager
{
    public static Action EnemyKilled;

    public byte ScorePoints;

    [SerializeField] protected float _speed;
    [SerializeField] protected AudioClip _dieSound;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        IDamageable player = collision.gameObject.GetComponent<IDamageable>();
        if (player != null)
            DoDamage(player);
    }

    public virtual void TakeDamage()
    {
        EnemyKilled();

        AudioManager.Instance.PlayOneSound(_dieSound);
    }

    public void DoDamage(IDamageable damageable)
    {
        damageable.TakeDamage();
    }
}
