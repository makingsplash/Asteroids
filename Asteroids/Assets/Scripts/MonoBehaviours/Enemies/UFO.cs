using System.Collections;
using UnityEngine;

public class Ufo : BaseEnemy, IDamageable
{
    [HideInInspector] public ObjectPool LaserPool;

    [SerializeField] private byte _ufoHealth;
    [SerializeField] private float _noShootTimer = 5;

    [Header("Shots")]
    [SerializeField] private AudioClip _shotSound;
    [SerializeField] private float _fireRate = 0.5f;
    
    private GameObject _rocket;

    private const string _playerTag = "Player";
    private float _selfDestroyTimer = 15f;
    private Vector2 _nextPosition;
    private Coroutine _laserShots;


    private void OnEnable()
    {
        Health = _ufoHealth;

        RocketController.OnPlayerEnabled += OnPlayerEnabled;
        if (_rocket == null)
            _rocket = GameObject.FindGameObjectWithTag(_playerTag);
        if (_rocket != null)
            StartCoroutine(Attack());
    }

    private void OnDisable()
    {
        RocketController.OnPlayerEnabled -= OnPlayerEnabled;
    }

    private void Update()
    {
        transform.Translate(_nextPosition);
    }

	#region State Machine

    private IEnumerator Attack()
    {
        StartCoroutine(NonShootingTimer());
        yield return StartCoroutine(ChaseRocket());

        StartCoroutine(FreeFlyUp());
    }

    private IEnumerator NonShootingTimer()
    {
        WaitForEndOfFrame wait = new WaitForEndOfFrame();
        while(_noShootTimer > 0)
        {
            _noShootTimer -= Time.deltaTime;
            yield return wait;
        }
        _laserShots = StartCoroutine(LaserShots());
    }

    private IEnumerator ChaseRocket()
    {
        WaitForEndOfFrame wait = new WaitForEndOfFrame();

        while (_rocket != null && _rocket.activeSelf)
        {
            _nextPosition = (_rocket.transform.position - transform.position).normalized * Speed * Time.deltaTime;
            yield return wait;
        }
        if (_laserShots != null)
            StopCoroutine(_laserShots);
    }

    private IEnumerator FreeFlyUp()
    {
        WaitForEndOfFrame wait = new WaitForEndOfFrame();

        while (!_rocket.activeSelf && _rocket != null)
        {
            _selfDestroyTimer -= Time.deltaTime;
            if (_selfDestroyTimer < 0)
                Destroy(gameObject);

            _nextPosition = transform.up * Speed * 1.3f * Time.deltaTime;

            yield return wait;
        }

        StartCoroutine(Attack());
    }

    IEnumerator LaserShots()
    {
        while (_rocket.activeSelf)
        {
            Vector3 position = -transform.up / 2.5f + transform.position;
            Vector3 direction = _rocket.transform.position - position;
            float eulerAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;

            LaserPool.SpawnObject(position, eulerAngle);

            AudioManager.Instance.PlayOneSound(_shotSound);

            yield return new WaitForSeconds(1 / _fireRate);
        }
    }
	#endregion

	private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable player = collision.gameObject.GetComponent<IDamageable>();
        if (player != null)
            DoDamage(player);
    }

    void OnPlayerEnabled(GameObject player)
    {
        _rocket = player;
        StartCoroutine(Attack());
    }

    public void TakeDamage(byte damage)
    {
        UIManager.Instance.ChangeScore(HitScorePoints);
        DecreaseHealth(damage);
    }

    protected override void Death()
    {
        UIManager.Instance.ChangeScore(DeathScorePoints);
        Destroy(gameObject);
    }
}
