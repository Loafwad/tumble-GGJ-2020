using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Slider slider;
    public float maxValue;

    private void Start()
    {
        maxValue = slider.value;
    }

    public void Update()
    {   
        float secondsPassedPercent = LevelController.instance.secondspassed / LevelController.instance.secondsToPass * maxValue;
        slider.value = secondsPassedPercent;    
    }

}
