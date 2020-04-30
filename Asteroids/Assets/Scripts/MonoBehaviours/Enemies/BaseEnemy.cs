using System;
using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour, IDamager
{
    public static Action EnemyKilled;

    public byte Health
    {
        get
        {
            return currentHealth;
        }
        set
        {
            if (value > currentHealth)
                _healthBar.ResizeMaxValue(value);
            else if (gameObject.activeSelf)
                StartCoroutine(_healthBar.SetCurrentValue(value));

            currentHealth = value;
        }
    }
    protected byte currentHealth;

    [Header("Enemy")]
    public float Speed;
    public byte HitScorePoints;
    public byte DeathScorePoints;

    [SerializeField] protected AudioClip _dieSound;
    [SerializeField] protected EnemyHealthBar _healthBar;

    private EnemyWarning _warning;
    private CheckCameraVisability _checkVisability;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        IDamageable player = collision.gameObject.GetComponent<IDamageable>();
        if (player != null)
            DoDamage(player);
    }

    protected void DecreaseHealth(byte amount)
    {
        Health = (byte) (Health - amount < 0 ? 0 : Health - amount);

        if (Health == 0)
        {
            Death();
            EnemyKilled();
            AudioManager.Instance.PlayOneSound(_dieSound);
        }
    }

    protected virtual void Death() { }

    public void DoDamage(IDamageable damageable) => damageable.TakeDamage();
}
