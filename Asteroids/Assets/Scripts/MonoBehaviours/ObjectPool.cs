using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    /*
     * Общий пул для всех метеоритов
     * В который мы из скриптабл обджектов будем подставлять значения
     * в игровые объекты этого пула ? было бы хорошо
     * 
     */

    public Queue<GameObject> Pool;

    [SerializeField] private byte _poolSize = 30;
    [SerializeField] private GameObject _objectPrefab;


    private void Awake()
    {
        StartCoroutine(FillPool());
    }

    public void SpawnObject(Vector2 position, float rotationEulerAngle)
    {
        if (Pool.Count > 0)
        {
            GameObject poolObject = Pool.Dequeue();
            poolObject.SetActive(true);
            poolObject.transform.position = position;

            poolObject.transform.eulerAngles = Vector3.forward * rotationEulerAngle;
        }
        else
            Debug.LogError("В пуле нет свободных объектов для спавна");
    }

    private IEnumerator FillPool()
    {
        if (Pool == null)
        {
            Pool = new Queue<GameObject>();
            for (int i = 0; i < _poolSize; i++)
            {
                GameObject Obj = Instantiate(_objectPrefab);

                //Obj.GetComponent<Laser>().ParentPool = this;
                Obj.GetComponent<IPoolObject>().ParentPool = this;
                Obj.transform.SetParent(transform);
                Obj.SetActive(false);

                Pool.Enqueue(Obj);
            }
        }
        yield return null;
    }
}
