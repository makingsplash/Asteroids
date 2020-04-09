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
    
    private float invulnerabilityTimerMax = 5f;
    private float invulnerabilityTimerCurrent;
    private bool isInvulnerability = false;

    private Rigidbody2D _rigidbody;
    private PolygonCollider2D _polygonCollider;
    private Animator _animator;
    private LaserPool _laserPool;

    
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _laserPool = GetComponent<LaserPool>();
    }

    private void OnEnable()
    {
        if(_polygonCollider == null)
            _polygonCollider = GetComponent<PolygonCollider2D>();
        if(_animator == null)
            _animator = GetComponent<Animator>();
        MakeInvulnerability();

        _vertical = 0;

        OnPlayerEnabled(gameObject);
    }

    void Update()
    {
        _vertical = Input.GetAxis("Vertical");
        if (_vertical >= 0)
        {
            _moveVertical = Vector2.up * Input.GetAxis("Vertical");
        }
        _horizontal = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.L))
            LaserShot();

        if (Input.GetKey(KeyCode.W))
            _animator.SetBool("isMoving", true);
        else
            _animator.SetBool("isMoving", false);
        

        if (isInvulnerability)
            InvulnerabilityCouner();
    }

    private void MakeInvulnerability()
    {
        invulnerabilityTimerCurrent = invulnerabilityTimerMax;
        isInvulnerability = true;
        _polygonCollider.enabled = false;
        _animator.SetBool("invulnerability", true);
    }

    private void InvulnerabilityCouner()
    {
        invulnerabilityTimerCurrent -= Time.deltaTime;
        if(invulnerabilityTimerCurrent < 0)
        {
            _animator.SetBool("invulnerability", false);
            _polygonCollider.enabled = true;
        }
    }

    private void LateUpdate()
    {
        _rigidbody.AddRelativeForce(_moveVertical * _moveSpeed * Time.deltaTime);
        _rigidbody.rotation -= _horizontal * _rotateSpeed * Time.deltaTime;
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
