using UnityEngine;

public class RocketInput : MonoBehaviour
{
    [HideInInspector] public float Vertical;
    [HideInInspector] public float Horizontal;
    [HideInInspector] public bool IsShotTapDown;

    [Header("For Tests")]
    public bool keyboardInput = false;


    private void Update()
    {
        if(keyboardInput)
        {
            Vertical = Input.GetAxis("Vertical");
            Horizontal = Input.GetAxis("Horizontal");

            IsShotTapDown = Input.GetKey(KeyCode.L);
        }
    }

    public void TapShotDown() => IsShotTapDown = true;
    public void TapShotUp() => IsShotTapDown = false;

    public void TapRightDown() => Horizontal = 1;
    public void TapRightUp() => Horizontal = 0;
    public void TapLeftDown() => Horizontal = -1;
    public void TapLeftUp() => Horizontal = 0;

    public void TapForwardDown() => Vertical = 1;
    public void TapForwardUp() => Vertical = 0;
}
