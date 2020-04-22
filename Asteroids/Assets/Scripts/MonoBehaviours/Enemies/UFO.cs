using System.Collections;
using UnityEngine;

public class UFO : BaseEnemy, IDamageable
{
    [HideInInspector] public ObjectPool LaserPool;

    [SerializeField] private AudioClip _shotSound;

    [SerializeField] private float _fireRate = 0.5f;
    
    private GameObject _player;

    private const string _playerTag = "Player";
    private float _horizontalFlyTimer = 2;
    private Vector2 _horizontalFlyDirection;
    private float _deadTimer = 12f;
    private Vector2 _nextPosition;


    private void OnEnable()
    {
        if (_player == null)
            _player = GameObject.FindGameObjectWithTag(_playerTag);
        RocketController.OnPlayerEnabled += SetPlayerGameObject;

        StartCoroutine(StatesSwitch());
    }

    private void OnDisable()
    {
        RocketController.OnPlayerEnabled -= SetPlayerGameObject;
    }

    private void Update()
    {
        transform.Translate(_nextPosition);
    }

	#region FSM
	private IEnumerator StatesSwitch()
    {
        yield return StartCoroutine(FreeFlyHorizontal());

        while (true)
        {
            yield return StartCoroutine(Attack());
            yield return StartCoroutine(FreeFlyUp());
        }
    }

    private IEnumerator FreeFlyHorizontal()
    {
        if (transform.position.x > 0)
            _horizontalFlyDirection = -transform.right;
        else
            _horizontalFlyDirection = transform.right;

        WaitForEndOfFrame wait = new WaitForEndOfFrame();

        while(_horizontalFlyTimer > 0)
        {
            _horizontalFlyTimer -= Time.deltaTime;
            _nextPosition = _horizontalFlyDirection * Speed * Time.deltaTime;
            yield return wait;
        }

        yield return null;
    }

    private IEnumerator Attack()
    {
        Coroutine shots = StartCoroutine(LaserShots());
        WaitForEndOfFrame wait = new WaitForEndOfFrame();

        while (_player != null && _player.activeSelf)
        {
            _nextPosition = (_player.transform.position - transform.position).normalized * Speed * Time.deltaTime;
            yield return wait;
        }

        _player = null;
        StopCoroutine(shots);
        
        yield return null;
    }

    private IEnumerator FreeFlyUp()
    {
        WaitForEndOfFrame wait = new WaitForEndOfFrame();

        while (_player == null)
        {
            _deadTimer -= Time.deltaTime;
            if (_deadTimer < 0)
                Destroy(gameObject);

            _nextPosition = transform.up * Speed * Time.deltaTime;

            yield return wait;
        }

        yield return null;
    }
    IEnumerator LaserShots()
    {
        while (true)
        {
            Vector3 position = -transform.up / 2.5f + transform.position;
            Vector3 direction = _player.transform.position - position;
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

    void SetPlayerGameObject(GameObject player) => this._player = player;

    public void TakeDamage()
    {
        if(DecreaseHealth())
            Destroy(gameObject);
    }
}
