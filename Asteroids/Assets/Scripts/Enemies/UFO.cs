using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFO : BaseEnemy
{
    [SerializeField] private AudioClip _shotSound;
    [SerializeField] private float _speed;
    [SerializeField] private float _fireRate = 0.5f;
    private GameObject _player;
    private float _nonAngryFlyTimer = 2;
    private Vector2 _nonAngryFlyDirection;
    private bool _isAngry = false;
    private Rigidbody2D _rigidbody;
    private LaserPool _laserPool;
    

    private void OnEnable()
    {
        if (_player == null)
            _player = GameObject.FindGameObjectWithTag("Player");
        RocketController.OnPlayerEnabled += SetPlayerGameObject;

        _rigidbody = GetComponent<Rigidbody2D>();
        _laserPool = FindObjectOfType<LaserPool>();

        if (transform.position.x > 0)
            _nonAngryFlyDirection = -transform.right;
        else
            _nonAngryFlyDirection = transform.right;
    }

    private void OnDisable()
    {
        RocketController.OnPlayerEnabled -= SetPlayerGameObject;
    }

    private void Update()
    {
        if (!_isAngry)
        {
            _nonAngryFlyTimer -= Time.deltaTime;
            if (_nonAngryFlyTimer < 0)
            {
                _isAngry = true;
                StartCoroutine(LaserShots());
            }
        }
    }

    private void LateUpdate()
    {
        if (!_isAngry)
        {
            _rigidbody.velocity = _nonAngryFlyDirection * _speed * Time.deltaTime;
        }
        else
        {
            if (_player != null && _player.activeSelf)
                _rigidbody.velocity = (_player.transform.position - transform.position).normalized * _speed * Time.deltaTime;
            else
                _rigidbody.velocity = transform.up * _speed * Time.deltaTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        IDamageable player = collision.gameObject.GetComponent<IDamageable>();
        if (player != null)
            DoDamage(player);
    }

    void SetPlayerGameObject(GameObject player)
    {
        this._player = player;
    }

    public override void TakeDamage()
    {
        base.TakeDamage();

        UIManager.Instance.ChangeScore(40);
        Destroy(gameObject);
    }

    IEnumerator LaserShots()
    {
        while (true)
        {
            if (_player != null && _player.activeSelf)
            {
                Vector3 position = -transform.up / 2.5f + transform.position;

                Vector3 direction = _player.transform.position - position;
                float eulerAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;

                _laserPool.LaunchLaser(position, eulerAngle);

                AudioController.Instance.PlayOneSound(_shotSound);
            }
            yield return new WaitForSeconds(1 / _fireRate);
        }
    }
}
