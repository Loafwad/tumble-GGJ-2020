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

	public void Awake()
	{
		_rigidbody2DCached = GetComponent<Rigidbody2D>();
	}

	// Start is called before the first frame update
	public override void Start()
    {
		base.Start();

		rigidbody2DCached.constraints = RigidbodyConstraints2D.FreezeAll;
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
		base.RunSimulation(run);

		if(run)
		{
			rigidbody2DCached.constraints = RigidbodyConstraints2D.None;
			rigidbody2DCached.WakeUp();
		}
		else
		{
			transform.position = originalPosRot.pos;
			transform.rotation = originalPosRot.rot;
			rigidbody2DCached.constraints = RigidbodyConstraints2D.FreezeAll;
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
				RaycastHit2D rayHit = RaycastAgainstTheScreen(connectorPosition);
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

			if (raycastHits.Count > 0)
			{
				Rigidbody2D rigidBody = GetComponent<Rigidbody2D>();
				return ConnectorState.Fixed;
			}

			return ConnectorState.Dragging;
		}
	}
}
