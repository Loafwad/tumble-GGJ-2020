using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameplayStatus
{
	Running = 0,
	Stopped = 1,
	Failed = 2,
	Success = 3
}

public class LevelController : Singleton<LevelController>
{
	public float secondsToPass = 5f;
    protected float secondsPassed = 0;

    protected GameplayStatus gameplayStatus = GameplayStatus.Stopped;

	protected List<ActiveElement> activeElements = new List<ActiveElement>();
	public bool _simulationRunning = false;

    private LevelManager levelManager;

    private void Awake()
    {
        levelManager = GetComponent<LevelManager>();
    }

    public float getsecondspassed()
    {
        return secondsPassed;
    }

    public bool simulationRunning
	{
		get
		{
			return _simulationRunning;
		}
	}

	public void RegisterActiveElement(ActiveElement activeElement)
	{
		activeElements.Add(activeElement);
	}

	public void FailLevel()
	{
		if (gameplayStatus == GameplayStatus.Running)
		{
			gameplayStatus = GameplayStatus.Failed;
			Debug.Log("Fail");
		}
	}

	public void FinishLevel()
	{
		if (gameplayStatus == GameplayStatus.Running)
		{
			gameplayStatus = GameplayStatus.Success;
			Debug.Log("Success");
		}

        levelManager.NextScene();
	}

	public void RunSimulation(bool run)
	{
		_simulationRunning = run;
		gameplayStatus = run ? GameplayStatus.Running : GameplayStatus.Stopped;

		if(!run)
		{
			secondsPassed = 0f;
		}

		// Reset level state to default
		for (int i = 0; i < activeElements.Count; ++i)
		{
			ActiveElement nextElement = activeElements[i];
			nextElement.rigidbody2DCached.simulated = run;
			if(!run)
			{
				nextElement.Reset();
			}
		}
	}
	
	void Update()
	{
		// Increase the time first, so it doesn't always start increased when starting
		if (_simulationRunning && gameplayStatus == GameplayStatus.Running)
		{
			secondsPassed += Time.deltaTime;
			if(secondsPassed >= secondsToPass)
			{
				FinishLevel();
			}
		}

		// Run / Stop simulation
		if (Input.GetKeyDown(KeyCode.Space))
		{
			RunSimulation(!_simulationRunning);
		}
	}
}
