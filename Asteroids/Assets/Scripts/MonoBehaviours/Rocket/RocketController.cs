using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketController : MonoBehaviour, IDamageable
{
    // Инициализируем чтобы не получить NullRef при вызове в UFOController без подписчиков
    // Тк Player включается в первый раз раньше, чем UFO и подписывается
    public static Action<GameObject> OnPlayerEnabled = delegate (GameObject g) { };

    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 250;
    [SerializeField] private float _rotateSpeed = 350;

    [Header("Sounds")]
    [SerializeField] private AudioClip _shotSound;
    [SerializeField] private AudioClip _rocketExplosionSound;

    
    private float _horizontal;
    private float _vertical;
    private Vector2 _moveVertical;
    private new Rigidbody2D rigidbody;
    private LaserPool _laserPool;

    
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        _laserPool = GetComponent<LaserPool>();
    }

    private void OnEnable()
    {
        _vertical = 0;

        OnPlayerEnabled(gameObject);
    }

    void Update()
    {
        _vertical = Input.GetAxis("Vertical");
        if (_vertical >= 0)
            _moveVertical = Vector2.up * Input.GetAxis("Vertical");
        _horizontal = Input.GetAxis("Horizontal");
        if (Input.GetKeyDown(KeyCode.L))
            LaserShot();            
    }

    private void LateUpdate()
    {
        rigidbody.AddRelativeForce(_moveVertical * _moveSpeed * Time.deltaTime);
        rigidbody.rotation -= _horizontal * _rotateSpeed * Time.deltaTime;
    }

    void LaserShot()
    {
        _laserPool.LaunchLaser(
            transform.up / 1.7f + transform.position,
            0 + transform.rotation.eulerAngles.z);

        AudioManager.Instance.PlayOneSound(_shotSound);
    }

    public void TakeDamage()
    {
        AudioManager.Instance.PlayOneSound(_rocketExplosionSound);
        SceneManager.Instance.PlayerDead();
    }
}
