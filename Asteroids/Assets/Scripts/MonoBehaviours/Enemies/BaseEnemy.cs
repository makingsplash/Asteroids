﻿using System;
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
            else
            {
                StartCoroutine(_healthBar.DecreaseCurrentValue());
            }

            currentHealth = value;
        }
    }
    protected byte currentHealth;

    public float Speed;
    public byte ScorePoints;

    [SerializeField] protected AudioClip _dieSound;
    [SerializeField] protected EnemyHealthBar _healthBar;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        IDamageable player = collision.gameObject.GetComponent<IDamageable>();
        if (player != null)
            DoDamage(player);
    }

    protected bool DecreaseHealth()
    {
        Health--;
        if (Health == 0)
        {
            EnemyKilled();
            AudioManager.Instance.PlayOneSound(_dieSound);
            Debug.Log("Враг " + gameObject.name + " убит");
            return true;
        }
        else
        {
            Debug.Log("У врага " + gameObject.name + " осталось " + Health + " хп");
            return false;
        }
    }

    public void DoDamage(IDamageable damageable)
    {
        damageable.TakeDamage();
    }
}
