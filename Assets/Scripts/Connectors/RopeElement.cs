using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RopeElement : MonoBehaviour
{
	public GameObject startAchor;
	public GameObject endAnchor;

	public Rigidbody2D cachedRigidbody2D;

    // Start is called before the first frame update
    void Awake()
    {
		cachedRigidbody2D = GetComponent<Rigidbody2D>();
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
