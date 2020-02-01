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
    protected float _secondsPassed = 0;

	public float secondspassed
	{
		get
		{
			return _secondsPassed;
		}
	}

	private LevelManager levelManager;
	protected GameplayStatus gameplayStatus = GameplayStatus.Stopped;

	protected List<ActiveElement> activeElements = new List<ActiveElement>();
	protected List<ConnectorBase> connectors = new List<ConnectorBase>();
	private bool _simulationRunning = false;
	public bool simulationRunning
	{
		get
		{
			return _simulationRunning;
		}
	}

	private void Awake()
    {
        levelManager = GetComponent<LevelManager>();
    }


	public void RegisterActiveElement(ActiveElement activeElement)
	{
		activeElements.Add(activeElement);
	}

	public void RegisterConnector(ConnectorBase connector, bool register = true)
	{
		if (register)
		{
			connectors.Add(connector);
		}
		else
		{
			connectors.Remove(connector);
		}
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
			_secondsPassed = 0f;
		}

		// Reset level state to default
		for (int i = 0; i < activeElements.Count; ++i)
		{
			ActiveElement nextElement = activeElements[i];
			nextElement.RunSimulation(true);
			if(!run)
			{
				nextElement.Reset();
			}
		}

		for (int i = 0; i < connectors.Count; ++i )
		{
			connectors[i].RunSimulation(run);
		}
	}
	
	void Update()
	{
		// Increase the time first, so it doesn't always start increased when starting
		if (_simulationRunning && gameplayStatus == GameplayStatus.Running)
		{
			_secondsPassed += Time.deltaTime;
			if(_secondsPassed >= secondsToPass)
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
