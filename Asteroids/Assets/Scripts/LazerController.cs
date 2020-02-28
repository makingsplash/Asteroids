using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LazerController : MonoBehaviour
{
    public static Action<GameObject> ReturnToPool;
    public static Action OnNoMoreEnemies;          // Если не осталось врагов - игрок победил

    [SerializeField] private float _speed;
    [SerializeField] private float _lifeTime;
    [SerializeField] private AudioClip _enemyExplotion;

    private Rigidbody2D rigidbody;
    private float _currentLifeTime;


    private void OnEnable()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }
    private void OnDisable()
    {
        _currentLifeTime = _lifeTime;
    }

    private void Update()
    {
        _currentLifeTime -= Time.deltaTime;
        if (_currentLifeTime < 0)
        {
            SendToPool();
        }
    }
    private void LateUpdate()
    {
        rigidbody.velocity = transform.up * _speed * Time.deltaTime;
    }

    void SendToPool()
    {
        gameObject.SetActive(false);
        ReturnToPool(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            AudioController.Instance.PlayOneSound(_enemyExplotion);

            collision.gameObject.GetComponent<ICanGotShot>().WasShooted();

            if (GameObject.FindGameObjectsWithTag("Enemy").Length < 2)
                OnNoMoreEnemies();
        }
        SendToPool();
    }
}