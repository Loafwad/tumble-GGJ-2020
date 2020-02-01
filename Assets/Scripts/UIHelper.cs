using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIHelper : MonoBehaviour
{
    public LevelController levelController;
    public TextMeshProUGUI buttonText;

    //play & stop button
    public void PlayButton()
    {
        levelController.RunSimulation(!levelController._simulationRunning);
        if (!levelController._simulationRunning)
        {
            buttonText.text = "Play";
        }
        else
        {
            buttonText.text = "Paused";
        }

    }
}
