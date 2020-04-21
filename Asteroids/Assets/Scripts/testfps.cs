using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class testfps : MonoBehaviour
{
    private int frames;
    private float currentTimer = 1.0f;
    private float oneSecond = 1.0f;

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
            text.text = Screen.dpi.ToString() + " " + frames.ToString();
            frames = 0;
        }
    }
}
