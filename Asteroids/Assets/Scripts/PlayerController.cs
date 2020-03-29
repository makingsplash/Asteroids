using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamageable
{
    // Инициализируем чтобы не получить NullRef при вызове в UFOController без подписчиков
    // Тк Player включается в первый раз раньше, чем UFO и подписывается
    public static Action<GameObject> OnPlayerEnabled = delegate (GameObject g) { };

    [Header("Movement")]
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _rotateSpeed;

    [Header("Sounds")]
    [SerializeField] private AudioClip _shotSound;
    [SerializeField] private AudioClip _rocketExplosionSound;

    
    private float _horizontal;
    private float _vertical;
    private Vector2 _moveVertical;
    private Rigidbody2D rigidbody;
    private LaserPool _laserPool;

    
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        _laserPool = GetComponent<LaserPool>();
    }

    private void OnEnable()
    {
        _vertical = 0;
        transform.position = new Vector2(0, 0);

        OnPlayerEnabled(gameObject);
    }

    private void OnDisable()
    {
        AudioController.Instance.PlayOneSound(_rocketExplosionSound);
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
            transform.rotation);

        AudioController.Instance.PlayOneSound(_shotSound);
    }

    public void GotDamage()
    {
        SceneManager.Instance.PlayerDead();
    }
}
