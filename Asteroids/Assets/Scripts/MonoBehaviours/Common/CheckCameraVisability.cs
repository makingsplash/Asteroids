using System.Collections;
using UnityEngine;

public class CheckCameraVisability : MonoBehaviour
{
    public bool IsVisible;

    private float _camHeigth;
    private float _camWidth;


    private void OnEnable()
    {
        _camHeigth = CameraInfo.Instance.CamOrtSize;
        _camWidth = CameraInfo.Instance.CamAspect * _camHeigth;

        StartCoroutine(CheckVisability());
    }

    public IEnumerator CheckVisability()
    {
        WaitForSeconds wait = new WaitForSeconds(0.1f);

        while (true)
        {
            yield return wait;
            float posX = Mathf.Abs(transform.position.x);
            float posY = Mathf.Abs(transform.position.y);

            if (posX > _camWidth || posY > _camHeigth)
            {
                IsVisible = false;
            }
            else
                IsVisible = true;
        }
    }
}
