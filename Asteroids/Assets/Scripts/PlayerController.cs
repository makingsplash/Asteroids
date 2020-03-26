using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamageable
{
    // Инициализируем чтобы не получить NullRef при вызове в UFOController без подписчиков
    // Тк Player включается в первый раз раньше, чем UFO и подписывается
    public static Action<GameObject> OnPlayerEnabled = delegate (GameObject g) { };

    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _rotateSpeed;

    [SerializeField] private GameObject _lazerPrefab;
    [SerializeField] private AudioClip _shotSound;
    [SerializeField] private AudioClip _rocketExplosionSound;
    private const int _lazerPoolSize = 40;

    private Queue<GameObject> _lazersQueue; // Пул подготовленных к выстрелу лазеров
    private float _horizontal;
    private float _vertical;
    private Vector2 _moveVertical;
    private Rigidbody2D rigidbody;

    
    private void OnEnable()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        _vertical = 0;
        transform.position = new Vector2(0, 0);
        FillLaserPool();

        OnPlayerEnabled(gameObject);

        // Не отписываемся в OnDisable тк при выключении объекта при смерти лазеры не смогут
        // Вернуться обратно в пул (получим NullRef)
        LazerController.ReturnToPool += ReturnLazerToPool;
    }

    private void OnDisable()
    {
        AudioController.Instance.PlayOneSound(_rocketExplosionSound);
    }

    void Update()
    {
        // Получение ввода от игрока
        _vertical = Input.GetAxis("Vertical");
        if (_vertical >= 0)                                           // Только положительное, тк у ракеты нет "заднего хода"
            _moveVertical = Vector2.up * Input.GetAxis("Vertical");   // Значение для изменения скорости передвижения
        _horizontal = Input.GetAxis("Horizontal");                    // Значение для изменения направления вращения
        if (Input.GetKeyDown(KeyCode.L))                              // Стрельба лазерами по кнопке L
            LaserShot();            
    }

    private void LateUpdate()
    {
        rigidbody.AddRelativeForce(_moveVertical * _moveSpeed * Time.deltaTime);   // Изменение скорости передвижения
        rigidbody.rotation -= _horizontal * _rotateSpeed * Time.deltaTime;         // Изменение направления вращения
    }

    void FillLaserPool()
    {
        if (_lazersQueue == null)
        {
            // Заполняем ObjectPool лазеров
            _lazersQueue = new Queue<GameObject>();
            for (int i = 0; i < _lazerPoolSize; i++)
            {
                GameObject lazerObj = GameObject.Instantiate(_lazerPrefab);
                lazerObj.transform.SetParent(transform);
                lazerObj.SetActive(false);
                _lazersQueue.Enqueue(lazerObj);
            }
        }
    }

    void LaserShot()
    {
        if (_lazersQueue.Count > 0)
        {
            GameObject lazer = _lazersQueue.Dequeue();
            lazer.SetActive(true);
            lazer.transform.position = transform.up / 1.7f + transform.position;
            lazer.transform.rotation = transform.rotation;

            AudioController.Instance.PlayOneSound(_shotSound);
        }
        else
            Debug.LogError("Не хватает лазеров в пуле для выстрела");
    }

    void ReturnLazerToPool(GameObject lazer)
    {
        _lazersQueue.Enqueue(lazer);
    }

    public void GotDamage()
    {
        throw new NotImplementedException();
    }
}
