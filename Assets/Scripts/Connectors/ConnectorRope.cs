using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectorRope : ConnectorBase
{
	public GameObject ropeElementPrefab;
	public Sprite closingSprite;
	public float connectionMaxAngle = 30f;
	public int maxLength = 10;

	protected int connectionsToPlaceLeft = 2;
	protected GameObject fixToObject = null;
	protected float elementLength;
	protected int maxElementsNumber;

	protected List<PosRot> originalPosRotsList;

	protected GameObject startGameObject;
	protected GameObject endGameObject;

	protected Vector3 cachedBestEndPosition;

	public List<GameObject> ropeElements = null;

	// Start is called before the first frame update
	public override void Start()
	{
		base.Start();
		
		RopeElement ropeElement = ropeElementPrefab.GetComponent<RopeElement>();
		elementLength = (ropeElement.endAnchor.transform.position - ropeElement.startAchor.transform.position).magnitude;
		maxElementsNumber = Mathf.FloorToInt(maxLength / elementLength);

		ropeElements = new List<GameObject>(maxElementsNumber);
		originalPosRotsList = new List<PosRot>(maxElementsNumber);

		--connectionsToPlaceLeft;

		startGameObject = new GameObject("StartGameObject");
		Vector3 startPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		startPosition.z = 0f;
		startGameObject.transform.position = startPosition;
		AddClosingSprite(startGameObject);
	}

	public override void RunSimulation(bool run)
	{
		base.RunSimulation(run);

		if (!run)
		{
			// Reset all rope elements
			for (int i = 0; i < ropeElements.Count; ++i)
			{
				RopeElement nextElement = ropeElements[i].GetComponent<RopeElement>();
				PosRot nextPosRot = originalPosRotsList[i];
				nextElement.transform.position = nextPosRot.pos;
				nextElement.transform.rotation = nextPosRot.rot;
				nextElement.cachedRigidbody2D.velocity = Vector2.zero;
				nextElement.cachedRigidbody2D.angularVelocity = 0f;
			}
		}
	}

	void AddClosingSprite(GameObject gameObject)
	{
		SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
		spriteRenderer.sprite = closingSprite;
		spriteRenderer.sortingOrder = 1;
		spriteRenderer.sortingLayerName = "Connector";
		gameObject.transform.parent = transform;
	}

	// Update is called once per frame
	public override void Update()
	{
		base.Update();

		Vector3 worldMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		worldMousePosition.z = 0f;

		GameObject closingPinGameObject = null;

		if (connectionsToPlaceLeft == 1)
		{
			closingPinGameObject = startGameObject;
		}
		else if(connectionsToPlaceLeft == 0)
		{
			closingPinGameObject = endGameObject;

			Vector3 lastElementPos;
			GameObject prevObject;
			bool prevObjectIsRope;

			if(ropeElements.Count == 0)
			{
				prevObject = fixToObject;
				lastElementPos = startGameObject.transform.position;
				prevObjectIsRope = false;
			}
			else
			{
				prevObject = ropeElements[ropeElements.Count - 1];
				RopeElement prevRopeElement = prevObject.GetComponent<RopeElement>();
				lastElementPos = prevRopeElement.endAnchor.transform.position;
				prevObjectIsRope = true;
			}

			// Add new element in front, if the prev is far enough
			if (Vector3.SqrMagnitude(worldMousePosition - lastElementPos) >= elementLength*elementLength &&
				ropeElements.Count < maxElementsNumber)
			{
				GameObject newElement = Instantiate<GameObject>(ropeElementPrefab, transform);
				newElement.transform.right = worldMousePosition - lastElementPos;
				newElement.transform.position = lastElementPos + (worldMousePosition - lastElementPos).normalized * elementLength * 0.5f;

				RopeElement newElementRopeElement = newElement.GetComponent<RopeElement>();

				HingeJoint2D joint = prevObject.AddComponent<HingeJoint2D>();
				joint.connectedBody = newElementRopeElement.cachedRigidbody2D;
				joint.autoConfigureConnectedAnchor = false;

				Vector2 anchor;
				Vector2 connectedAnchor = newElementRopeElement.startAchor.transform.localPosition;

				if ( prevObjectIsRope )
				{
					RopeElement prevRopeElement = prevObject.GetComponent<RopeElement>();
					anchor = prevRopeElement.endAnchor.transform.localPosition;

					JointAngleLimits2D jal = new JointAngleLimits2D();
					jal.min = -connectionMaxAngle;
					jal.max = connectionMaxAngle;
					joint.limits = jal;
				}
				else
				{
					anchor = prevObject.transform.InverseTransformPoint(startGameObject.transform.position);
				}

				joint.anchor = anchor;
				joint.connectedAnchor = connectedAnchor;

				ropeElements.Add(newElement);

				// Fix the starting pin to the rope element so it follows it
				if(ropeElements.Count == 1)
				{
					startGameObject.transform.parent = ropeElements[0].transform;
				}
			}
		}

		cachedBestEndPosition = CaculateBestEndPosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));
		if (closingPinGameObject != null)
		{
			if (ropeElements.Count > 0)
			{
				closingPinGameObject.transform.position = ropeElements[ropeElements.Count - 1].GetComponent<RopeElement>().endAnchor.transform.position; ;
			}
			else
			{
				closingPinGameObject.transform.position = cachedBestEndPosition;
			}
		}

	}

	public Vector3 CaculateBestEndPosition(Vector3 worldPosition)
	{
		worldPosition.z = 0f;

		if (ropeElements.Count > 0)
		{
			GameObject lastRopeElementGO = ropeElements[ropeElements.Count - 1];
			RopeElement lastRopeElement = lastRopeElementGO.GetComponent<RopeElement>();
			Vector3 offset = lastRopeElement.endAnchor.transform.position - lastRopeElement.transform.position;
			Vector3 seekPosition = worldPosition - offset;

			RopeElement firstRopeElement = ropeElements[0].GetComponent<RopeElement>();
			float distanceSqr = (firstRopeElement.endAnchor.transform.position - seekPosition).sqrMagnitude;
			float maxDistance = elementLength * maxElementsNumber;

			Vector3 finalPosition = seekPosition;
			if (distanceSqr > maxDistance * maxDistance)
			{
				Vector3 direction = worldPosition - firstRopeElement.startAchor.transform.position;
				finalPosition = firstRopeElement.endAnchor.transform.position + direction.normalized * maxDistance;
			}

			finalPosition.z = 0f;
			return finalPosition;
		}

		return worldPosition;
	}

	public void FixedUpdate()
	{
		if (ropeElements.Count > 0)
		{
			Vector3 finalPosition = cachedBestEndPosition;
			ropeElements[ropeElements.Count - 1].GetComponent<RopeElement>().cachedRigidbody2D.MovePosition(finalPosition);
		}
	}

	public override ConnectorState FixToPoint(Vector2 position, bool Pressed, bool mouseCoordinates)
	{
		if (Pressed)
		{
			// Get the proper input
			Vector3 inPos;
			if (mouseCoordinates)
			{
				inPos = Camera.main.ScreenToWorldPoint(position);
				inPos.z = 0f;
			}
			else
			{
				inPos = new Vector3(position.x, position.y, 0f);
			}

			// Check if player hit something
			RaycastHit2D raycastHit2D = RaycastAgainstTheScreen(inPos);
			if (raycastHit2D.collider == null)
			{
				return ConnectorState.Dragging;
			}
			
			if (connectionsToPlaceLeft == 1)
			{
				// Initiate creating the rope, and create an end drag element
				fixToObject = raycastHit2D.collider.gameObject;
				endGameObject = new GameObject("EndGameObject");
				endGameObject.transform.position = inPos;
				AddClosingSprite(endGameObject);
			}
			else if(connectionsToPlaceLeft == 0)
			{
				GameObject lastRopeElement = ropeElements[ropeElements.Count - 1];
				HingeJoint2D joint = lastRopeElement.AddComponent<HingeJoint2D>();
				joint.connectedBody = raycastHit2D.collider.gameObject.GetComponent<Rigidbody2D>();
				joint.autoConfigureConnectedAnchor = false;
				joint.anchor = lastRopeElement.GetComponent<RopeElement>().endAnchor.transform.localPosition;
				joint.connectedAnchor = raycastHit2D.collider.gameObject.transform.InverseTransformPoint(inPos);

				enabled = false;

				for(int i = 0; i<ropeElements.Count; ++i)
				{
					PosRot newPosRot = new PosRot(ropeElements[i].GetComponent<RopeElement>().transform);
					if (originalPosRotsList.Count - 1 < i)
					{
						originalPosRotsList.Add(newPosRot);
					}
					else
					{
						originalPosRotsList[i] = newPosRot;
					}
				}

				endGameObject.transform.parent = ropeElements[ropeElements.Count-1].transform;
				return ConnectorState.Fixed;
			}

			--connectionsToPlaceLeft;
		}

		return ConnectorState.Dragging;
	}
}