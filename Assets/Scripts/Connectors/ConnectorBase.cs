using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum ConnectorState
{
	Inactivated = 0,
	Dragging = 1,
	Fixed = 2,
	FixedPartially = 3,
	Rotating = 4
}

public class PosRot
{
	public PosRot(Vector3 pos, Quaternion rot)
	{
		this.pos = pos;
		this.rot = rot;
	}
	public PosRot(Transform transform)
	{
		this.pos = transform.position;
		this.rot = transform.rotation;
	}

	public PosRot() { }

	public Vector3 pos;
	public Quaternion rot;
}

public class ConnectorBase : MonoBehaviour
{
	public List<Vector2> connectorsPositions;
	public Sprite icon;
	protected ConnectorState connectorState = ConnectorState.Inactivated;
	protected PosRot originalPosRot;
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
		if(!run && runtimeCreated)
		{
			DestroySelf();
		}
	}

	public virtual void DestroySelf()
	{
		LevelController.instance.RegisterConnector(this, false);
		Destroy(this.gameObject);
	}

	protected RaycastHit2D RaycastAgainstTheScreen(Vector2 worldPosition)
	{
		Vector3 hitPosition = new Vector3(worldPosition.x, worldPosition.y, 0f);
		hitPosition.z = Camera.main.transform.position.z;

		Ray ray = new Ray(hitPosition, Vector3.forward);
		return Physics2D.GetRayIntersection(ray, 10f, LayerMask.GetMask("ActiveElement"));
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
				originalPosRot = new PosRot(transform);
				ConnectorController.instance.activeConnector = null;
				if (LevelController.instance.simulationRunning)
				{
					runtimeCreated = true;
					RunSimulation(true);
				}
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

		if(Input.GetMouseButtonDown(1))
		{
			ConnectorController.instance.PushBackLastConnector();
			DestroySelf();
		}

		if (triedToFix && connectorState == ConnectorState.Fixed)
		{
			enabled = false;
		}
	}
}
