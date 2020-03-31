using System;
using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour, IDamageable, IDamager
{
    public static Action OnNoOtherEnemies;

    [SerializeField] private AudioClip _enemyExplotion;

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

    public virtual void TakeDamage()
    {
        _enemyAmount--;
        if (_enemyAmount == 0)
            OnNoOtherEnemies();

        AudioController.Instance.PlayOneSound(_enemyExplotion);
    }

    public void DoDamage(IDamageable damageable)
    {
        damageable.TakeDamage();
    }
}
