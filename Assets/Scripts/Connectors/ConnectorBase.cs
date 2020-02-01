using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConnectorState
{
	Inactivated = 0,
	Dragged = 1,
	Fixed = 2,
}

public class ConnectorBase : MonoBehaviour
{
	public List<Vector2> connectorsPositions;
	protected ConnectorState connectorState = ConnectorState.Inactivated;
	protected Vector2 originalPosition;
	protected Quaternion originalRotation;
	protected bool runtimeCreated = false;

	public virtual void Start()
	{
		LevelController.instance.RegisterConnector(this);
	}

	public virtual void StartDragging()
	{
		connectorState = ConnectorState.Dragged;
	}

	public virtual void RunSimulation(bool run)
	{
		
	}

	public void TryToFix(Vector2 position, bool mouseCoordinates = true)
	{
		if (connectorState == ConnectorState.Dragged)
		{
			connectorState = FixToPoint(position, mouseCoordinates);
			if(connectorState == ConnectorState.Fixed)
			{
				originalPosition = transform.position;
				originalRotation = transform.rotation;
			}
		}
	}

	// Definition of behavior of actual fixing the connecter.
	// Return Dragged if can't be solved in that position
	public virtual ConnectorState FixToPoint(Vector2 position, bool mouseCoordinates)
	{
		// Fail to connect
		return ConnectorState.Dragged;
	}

	// Update is called once per frame
	public virtual void Update()
    {
        if(Input.GetMouseButtonDown(0))
		{
			TryToFix(Input.mousePosition, true);

			if ( connectorState == ConnectorState.Fixed)
			{
				enabled = false;
			}
		}
    }
}
