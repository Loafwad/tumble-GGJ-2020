using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ConnectorFixed : ConnectorBase
{
	public float maxDragDistance = 1f;

	protected Vector2 pressWorldPosition;
	private Rigidbody2D _rigidbody2DCached;
	public Rigidbody2D rigidbody2DCached
	{
		get
		{
			return _rigidbody2DCached;
		}
	}

	// Start is called before the first frame update
	public override void Start()
    {
		base.Start();

		_rigidbody2DCached = GetComponent<Rigidbody2D>();
		rigidbody2DCached.gravityScale = 1f;
	}

    // Update is called once per frame
    public override void Update()
    {
		base.Update();

		if (connectorState == ConnectorState.Dragging)
		{
			Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			newPosition.z = 0f;
			gameObject.transform.position = newPosition;
		}
		else if (connectorState == ConnectorState.FixedPartially)
		{
			Vector2 currentWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			if (Vector2.SqrMagnitude(pressWorldPosition - currentWorldPosition) > maxDragDistance * maxDragDistance)
			{
				connectorState = ConnectorState.Rotating;
			}
		}

		if (connectorState == ConnectorState.Rotating)
		{
			Vector2 currentWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector3 forward = currentWorldPosition - pressWorldPosition;
			Vector3 up = Vector3.Cross(-Vector3.forward, forward);
			Quaternion rotation = Quaternion.FromToRotation(Vector3.right, forward);
			gameObject.transform.rotation = rotation;
		}
		
    }

	public override void RunSimulation(bool run)
	{
		if(run)
		{
			rigidbody2DCached.gravityScale = 1f;
			rigidbody2DCached.simulated = true;
		}
		else
		{
			if(runtimeCreated)
			{
				LevelController.instance.RegisterConnector(this, false);
				Destroy(this.gameObject);
				return;
			}

			transform.position = originalPosition;
			transform.rotation = originalRotation;
			rigidbody2DCached.gravityScale = 0f;
			rigidbody2DCached.velocity = Vector2.zero;
			rigidbody2DCached.angularVelocity = 0f;
		}
	}

	public override ConnectorState FixToPoint(Vector2 position, bool Pressed, bool mouseCoordinates)
	{
		if(Pressed)
		{
			pressWorldPosition = Camera.main.ScreenToWorldPoint(position);
			return ConnectorState.FixedPartially;
		}
		else
		{
			if (connectorState == ConnectorState.Rotating)
			{
				return ConnectorState.Dragging;
			}

			List<RaycastHit2D> raycastHits = new List<RaycastHit2D>();

			for (int i = 0; i < connectorsPositions.Count; ++i)
			{
				Vector2 connectorPosition = transform.TransformPoint(connectorsPositions[i]);
				Vector3 hitPosition = new Vector3(connectorPosition.x, connectorPosition.y, 0f);
				hitPosition.z = Camera.main.transform.position.z;

				Ray ray = new Ray(hitPosition, Vector3.forward);
				RaycastHit2D rayHit = Physics2D.GetRayIntersection(ray, 10f);
				if (rayHit.collider != null)
				{
					raycastHits.Add(rayHit);
				}
			}

			for (int i = 0; i < raycastHits.Count; ++i)
			{
				Collider2D nextCollider = raycastHits[i].collider;
				FixedJoint2D joint = gameObject.AddComponent<FixedJoint2D>();
				joint.connectedBody = nextCollider.attachedRigidbody;
			}

			if (LevelController.instance.simulationRunning)
			{
				runtimeCreated = true;
				RunSimulation(true);
			}

			if (raycastHits.Count > 0)
			{
				Rigidbody2D rigidBody = GetComponent<Rigidbody2D>();
				return ConnectorState.Fixed;
			}

			return ConnectorState.Dragging;
		}
	}
}
