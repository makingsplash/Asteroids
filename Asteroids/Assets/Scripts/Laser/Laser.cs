using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour, IDamager
{
    public static Action<GameObject> ReturnToPool;  

    [SerializeField] private float _speed = 250;
    [SerializeField] private float _lifeTime;

    private new Rigidbody2D rigidbody;
    private float _currentLifeTime;


    private void OnEnable()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }
    private void OnDisable()
    {
        _currentLifeTime = _lifeTime;
    }

    private void Update()
    {
        _currentLifeTime -= Time.deltaTime;
        if (_currentLifeTime < 0)
        {
            SendToPool();
        }
    }

    private void LateUpdate()
    {
        rigidbody.velocity = transform.up * _speed * Time.deltaTime;
    }

    void SendToPool()
    {
        gameObject.SetActive(false);
        ReturnToPool(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        IDamageable enemy = collision.gameObject.GetComponent<IDamageable>();

        DoDamage(enemy);
    }

    public void DoDamage(IDamageable damageable)
    {
        damageable.GotDamage();
        SendToPool();
    }
}