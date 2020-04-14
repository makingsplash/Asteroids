using System.Collections;
using UnityEngine;

public class UFO : BaseEnemy
{
    [HideInInspector] public ObjectPool LaserPool;

    [SerializeField] private AudioClip _shotSound;

    [SerializeField] private float _fireRate = 0.5f;
    
    private GameObject _player;

    private float _nonAngryFlyTimer = 2;
    private Vector2 _nonAngryFlyDirection;
    private bool _isAngry = false;
    private float _deadTimer = 10f;

    private void OnEnable()
    {
        if (_player == null)
            _player = GameObject.FindGameObjectWithTag("Player");
        RocketController.OnPlayerEnabled += SetPlayerGameObject;

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
            transform.Translate(_nonAngryFlyDirection * Speed * Time.deltaTime);
        }
        else
        {
            if (_player != null && _player.activeSelf)
                transform.Translate((_player.transform.position - transform.position).normalized * Speed * Time.deltaTime);
            else
            {
                transform.Translate(transform.up * Speed * Time.deltaTime);
                _deadTimer -= Time.deltaTime;
                if (_deadTimer < 0)
                    Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
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

                LaserPool.SpawnObject(position, eulerAngle);

                AudioManager.Instance.PlayOneSound(_shotSound);
            }
            yield return new WaitForSeconds(1 / _fireRate);
        }
    }
}
