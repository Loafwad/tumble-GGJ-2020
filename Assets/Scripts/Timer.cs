using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Slider slider;
    public float maxValue;
    private LevelController levelController;

    private void Start()
    {
        levelController = LevelController.instance;
        maxValue = slider.value;
    }

    public void Update()
    {   
        var secondsPassedPercent = levelController.getsecondspassed() / levelController.secondsToPass * maxValue;
        slider.value = secondsPassedPercent;    
    }

}
