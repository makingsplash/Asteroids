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
    [SerializeField] private GameObject _shieldAnim;

    private bool _canShot;

    private readonly float _shieldEnabledTimer = 5f;
    private readonly float _shieldReloadTimer = 5f;
    private bool _isShieldReady;

    private RocketInput _input;
    private Rigidbody2D _rigidbody;
    private PolygonCollider2D _polygonCollider;

    
    private void OnEnable()
    {
        if(_polygonCollider == null)
            _polygonCollider = GetComponent<PolygonCollider2D>();
        if(_input == null)
            _input = GetComponent<RocketInput>();

        _isShieldReady = true;
        StartCoroutine(UseShield());

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
    }

    private void LateUpdate()
    {
        _rigidbody.AddRelativeForce(
            Vector2.up * _input.Vertical * _moveSpeed * Time.deltaTime);

        transform.eulerAngles -= Vector3.forward * _input.Horizontal * _rotateSpeed * Time.deltaTime;
    }
	public void TakeDamage(byte damage)
    {
        AudioManager.Instance.PlayOneSound(_rocketExplosionSound);
        SceneManager.Instance.PlayerDead();
    }

	#region Shield usage
	public IEnumerator UseShield()
    {
        if (_isShieldReady)
        {
            _isShieldReady = false;

            yield return ActivateShield();
            yield return DeactivateShield();
            UIManager.Instance.EnableShieldButton();

            _isShieldReady = true;
        }
    }

    private Coroutine ActivateShield()
    {
        _polygonCollider.enabled = false;
        _shieldAnim.SetActive(true);
        return StartCoroutine(UIManager.Instance.DisableShieldButton(_shieldEnabledTimer));
    }

    private Coroutine DeactivateShield()
    {
        ///// For tests
        if (!TESTNONHITTABLE)
        {
            _polygonCollider.enabled = true;
            _shieldAnim.SetActive(false);
        }

        return StartCoroutine(UIManager.Instance.PrepareShieldButton(_shieldReloadTimer));
    }
    #endregion

    #region Laser usage
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
    #endregion

}
