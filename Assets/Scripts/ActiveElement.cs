using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ActiveElement : MonoBehaviour
{
	protected Vector2 originalPosition;
	protected Quaternion originalRotation;

	private Rigidbody2D _rigidbody2DCached;

	public Rigidbody2D rigidbody2DCached
	{
		get
		{
			return _rigidbody2DCached;
		}
	}
	// Start is called before the first frame update
	void Start()
	{
		originalPosition = gameObject.transform.position;
		originalRotation = gameObject.transform.rotation;
		_rigidbody2DCached = GetComponent<Rigidbody2D>();
		LevelController.instance.RegisterActiveElement(this);
	}	public void Reset()
	{
		gameObject.transform.position = originalPosition;
		gameObject.transform.rotation = originalRotation;
		_rigidbody2DCached.velocity = Vector2.zero;
		rigidbody2DCached.angularVelocity = 0f;
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		if(col.gameObject.GetComponent<GroundElement>())
		{
			LevelController.instance.FailLevel();
		}
	}
}
