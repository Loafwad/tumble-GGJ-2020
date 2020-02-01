using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConnectorState
{
	Inactivated = 0,
	Dragging = 1,
	Fixed = 2,
	FixedPartially = 3,
	Rotating = 4
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
		connectorState = ConnectorState.Dragging;
	}

	public virtual void RunSimulation(bool run)
	{
		
	}

	// Hit position, if the input was pressed or released, if it in screen or world coordinates
	public void TryToFix(Vector2 position, bool Pressed = true, bool mouseCoordinates = true)
	{
		if (connectorState == ConnectorState.Dragging || connectorState == ConnectorState.FixedPartially || connectorState == ConnectorState.Rotating)
		{
			ConnectorState prevState = connectorState;
			connectorState = FixToPoint(position, Pressed, mouseCoordinates);

			// If it was just fixed to something, save it's status
			if(	prevState == ConnectorState.Dragging &&
				(connectorState == ConnectorState.Fixed || connectorState == ConnectorState.FixedPartially) )
			{
				originalPosition = transform.position;
				originalRotation = transform.rotation;
			}
		}
	}

	// Definition of behavior of actual fixing the connecter.
	// Return Dragged if can't be solved in that position
	public virtual ConnectorState FixToPoint(Vector2 position, bool Pressed, bool mouseCoordinates)
	{
		// Fail to connect
		return ConnectorState.Dragging;
	}

	// Update is called once per frame
	public virtual void Update()
    {
		bool triedToFix = false;
        if(Input.GetMouseButtonDown(0))
		{
			TryToFix(Input.mousePosition, true);
			triedToFix = true;
		}
		else if(Input.GetMouseButtonUp(0))
		{
			TryToFix(Input.mousePosition, false);
			triedToFix = true;
		}

		if (triedToFix && connectorState == ConnectorState.Fixed)
		{
			enabled = false;
		}
	}
}
