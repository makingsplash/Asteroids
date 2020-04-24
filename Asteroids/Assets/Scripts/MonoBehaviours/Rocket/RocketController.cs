using System;
using System.Collections;
using UnityEngine;

public class RocketController : MonoBehaviour, IDamageable
{
    
    [Header("Testing")] public bool TESTNONHITTABLE;

    public static Action<GameObject> OnPlayerEnabled;

    [SerializeField] private float _fireRate;

    [SerializeField] private ObjectPool _laserPool;

    [Header("Movement")]
    [SerializeField] private ushort _moveSpeed = 250;
    [SerializeField] private ushort _rotateSpeed = 350;

    [Header("Sounds")]
    [SerializeField] private AudioClip _shotSound;
    [SerializeField] private AudioClip _rocketExplosionSound;

    [Header("Animations")]
    [SerializeField] private GameObject _movingFireAnim;
    [SerializeField] private GameObject _invulnerabilityAnim;

    private bool _canShot;

    private RocketInput _input;
    
    private readonly float invulnerabilityTimerMax = 5f;
    private float invulnerabilityTimerCurrent;
    private bool isInvulnerability = false;

    private Rigidbody2D _rigidbody;
    private PolygonCollider2D _polygonCollider;

    
    private void OnEnable()
    {
        _polygonCollider = GetComponent<PolygonCollider2D>();
        _input = GetComponent<RocketInput>();

        MakeInvulnerability();

        _canShot = true;

        _input.Horizontal = 0;
        _input.Vertical = 0;
        _input.IsShotTapDown = false;

        OnPlayerEnabled?.Invoke(gameObject);
    }

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!_movingFireAnim.activeSelf && _input.Vertical > 0.1f)
            _movingFireAnim.SetActive(true);
        else if(_input.Vertical < 0.1f)
            _movingFireAnim.SetActive(false);

        if (_input.IsShotTapDown)
            LaserShot();

        if (isInvulnerability)
            InvulnerabilityCouner();
    }
    private void LateUpdate()
    {
        _rigidbody.AddRelativeForce(
            Vector2.up * _input.Vertical * _moveSpeed * Time.deltaTime);

        transform.eulerAngles -= Vector3.forward * _input.Horizontal * _rotateSpeed * Time.deltaTime;
    }

    private void MakeInvulnerability()
    {
        _polygonCollider.enabled = false;

        invulnerabilityTimerCurrent = invulnerabilityTimerMax;
        isInvulnerability = true;
        _invulnerabilityAnim.SetActive(true);
    }

    private void InvulnerabilityCouner()
    {
        invulnerabilityTimerCurrent -= Time.deltaTime;
        if(invulnerabilityTimerCurrent < 0 && !TESTNONHITTABLE)
        {
            _polygonCollider.enabled = true;

            _invulnerabilityAnim.SetActive(false);
            isInvulnerability = false;
        }
    }


    private IEnumerator LaserReload()
    {
        _canShot = false;
        yield return new WaitForSeconds(_fireRate);
        _canShot = true;
    }

    private void LaserShot()
    {
        if (_canShot)
        {
            _laserPool.SpawnObject(
                transform.up / 1.7f + transform.position,
                transform.rotation.eulerAngles.z);

            AudioManager.Instance.PlayOneSound(_shotSound);

            StartCoroutine(LaserReload());
        }
    }

    public void TakeDamage()
    {
        AudioManager.Instance.PlayOneSound(_rocketExplosionSound);
        SceneManager.Instance.PlayerDead();
    }
}
