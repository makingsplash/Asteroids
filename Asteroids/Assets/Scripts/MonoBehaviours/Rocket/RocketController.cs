using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketController : MonoBehaviour, IDamageable
{
    public static Action<GameObject> OnPlayerEnabled;

    [Header("Movement")]
    [SerializeField] private ushort _moveSpeed = 250;
    [SerializeField] private ushort _rotateSpeed = 350;

    [Header("Sounds")]
    [SerializeField] private AudioClip _shotSound;
    [SerializeField] private AudioClip _rocketExplosionSound;
    
    [Header("Invulnerability")]
    [SerializeField] private Animator _animator;
    [SerializeField] private PolygonCollider2D _polygonCollider;

    [SerializeField] private float _fireRate;
    private bool _canShot;

    private float _horizontal;
    private float _vertical;
    private Vector2 _moveVertical;

    
    private readonly float invulnerabilityTimerMax = 5f;
    private float invulnerabilityTimerCurrent;
    private bool isInvulnerability = false;

    private Rigidbody2D _rigidbody;
    private LaserPool _laserPool;

    
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _laserPool = GetComponent<LaserPool>();
    }

    private void OnEnable()
    {
        MakeInvulnerability();

        _vertical = 0;

        _canShot = true;

        OnPlayerEnabled?.Invoke(gameObject);
    }

    void Update()
    {
        _vertical = Input.GetAxis("Vertical");
        if (_vertical >= 0)
        {
            _moveVertical = Vector2.up * Input.GetAxis("Vertical");
        }
        _horizontal = Input.GetAxis("Horizontal");

        if (Input.GetKey(KeyCode.L))
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

    private IEnumerator LaserReload()
    {
        yield return new WaitForSeconds(_fireRate);
        _canShot = true;
    }

    void LaserShot()
    {
        if (_canShot)
        {
            _laserPool.LaunchLaser(
                transform.up / 1.7f + transform.position,
                0 + transform.rotation.eulerAngles.z);

            AudioManager.Instance.PlayOneSound(_shotSound);

            _canShot = false;
            StartCoroutine(LaserReload());
        }
    }

    public void TakeDamage()
    {
        AudioManager.Instance.PlayOneSound(_rocketExplosionSound);
        SceneManager.Instance.PlayerDead();
    }
}
