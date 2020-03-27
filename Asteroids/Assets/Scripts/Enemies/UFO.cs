using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFO : BaseEnemy, IDamageable, IDamager
{

    [SerializeField] private float _speed;
    private GameObject _player;
    private float _nonAngryFlyTimer = 2;
    private Vector2 _nonAngryFlyDirection;
    private bool _isAngry = false;
    private Rigidbody2D rigidbody;


    private void OnEnable()
    {
        if (_player == null)
            _player = GameObject.FindWithTag("Player");

        PlayerController.OnPlayerEnabled += SetPlayerGameObject;

        rigidbody = GetComponent<Rigidbody2D>();

        // Если заспавнились справа - летим влево и наоборот
        if (transform.position.x > 0)
            _nonAngryFlyDirection = -transform.right;
        else
            _nonAngryFlyDirection = transform.right;
    }

    private new void OnDisable()
    {
        base.OnDisable();

        PlayerController.OnPlayerEnabled -= SetPlayerGameObject;
    }

    private void Update()
    {
        // Переход из свободного полёта в преследование
        if (!_isAngry)
        {
            _nonAngryFlyTimer -= Time.deltaTime;
            if (_nonAngryFlyTimer < 0)
                _isAngry = true;
        }
    }

    private void LateUpdate()
    {
        if(!_isAngry)
        {
            // Свободный полёт
            rigidbody.velocity = _nonAngryFlyDirection * _speed * Time.deltaTime;
        }
        else
        {
            // Если игрок жив, преследуем
            if (_player != null && _player.activeSelf)
                rigidbody.velocity = (_player.transform.position - transform.position).normalized * _speed * Time.deltaTime;
            // Если игрок в процессе возрождения, летим вверх
            else
                rigidbody.velocity = transform.up * _speed * Time.deltaTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        IDamageable player = collision.gameObject.GetComponent<IDamageable>();
        if (player != null)
            DoDamage(player);
    }

    public void DoDamage(IDamageable damageable)
    {
        damageable.GotDamage();
    }

    void SetPlayerGameObject(GameObject player)
    {
        this._player = player;
    }

    public void GotDamage()
    {
        UIManager.Instance.ChangeScore(40);
        Destroy(gameObject);
    }
}
