using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteorite : BaseEnemy
{
    [Header("When destroyed")]
    [SerializeField] private int _scorePoints;
    [SerializeField] private List<GameObject> _smallerMeteoritesPrefabs = new List<GameObject>();

    private new Rigidbody2D rigidbody;


    private void OnEnable()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void LateUpdate()
    {
        rigidbody.velocity = transform.up * _speed * Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        IDamageable player = collision.gameObject.GetComponent<IDamageable>();
        if (player != null)
            DoDamage(player);
    }

    public override void TakeDamage()
    {
        UIManager.Instance.ChangeScore(_scorePoints);

        if (_smallerMeteoritesPrefabs.Count > 0)
            SpawnSmallerMeteorites();

        base.TakeDamage();
        Destroy(gameObject);
    }

    void SpawnSmallerMeteorites()
    {
        for (int i = 0; i < UnityEngine.Random.Range(1, 3); i++)
        {
            // pick random prefab, apply random offset and rotation
            Instantiate(
                _smallerMeteoritesPrefabs[UnityEngine.Random.Range(0, _smallerMeteoritesPrefabs.Count)],
                transform.position + Vector3.right * UnityEngine.Random.Range(-0.5f, 0.5f) + Vector3.up * UnityEngine.Random.Range(-0.5f, 0.5f),
                Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + UnityEngine.Random.Range(-30, 30)));
        }
    }
}