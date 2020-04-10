using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPool : MonoBehaviour
{
    public Queue<GameObject> Pool;

    [SerializeField] private byte _poolSize = 40;
    [SerializeField] private GameObject _laserPrefab;


    void Awake()
    {
        StartCoroutine(FillLaserPool());
    }

    public void LaunchLaser(Vector2 launchPosition, float rotationEulerAngle)
    {
        if (Pool.Count > 0)
        {
            GameObject laser = Pool.Dequeue();
            laser.SetActive(true);
            laser.transform.position = launchPosition;

            laser.transform.eulerAngles = Vector3.forward * rotationEulerAngle;
        }
        else
            Debug.LogError("В пуле нет свободных лазеров для выстрела");
    }

    private IEnumerator FillLaserPool()
    {
        if (Pool == null)
        {
            Pool = new Queue<GameObject>();
            for (int i = 0; i < _poolSize; i++)
            {
                GameObject laserObj = Instantiate(_laserPrefab);

                laserObj.GetComponent<Laser>().ParentPool = this;
                laserObj.transform.SetParent(transform);
                laserObj.SetActive(false);

                Pool.Enqueue(laserObj);
            }
        }
        yield return null;
    }
}
