using UnityEngine;
using TMPro;

public class testfps : MonoBehaviour
{
    private int frames;
    private float currentTimer = 1.0f;
    private const float oneSecond = 1.0f;

    private TextMeshProUGUI text;


    private void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        currentTimer -= Time.deltaTime;
        frames++;

        if (currentTimer < 0)
        {
            currentTimer = oneSecond;
            text.text = "fps: " + frames;
            frames = 0;
        }
    }
}
