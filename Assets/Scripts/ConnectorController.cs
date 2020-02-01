using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectorController : MonoBehaviour
{
	public List<GameObject> connectors;
	protected ConnectorBase activeConnector = null;

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		for (int i = 0; i < connectors.Count; ++i)
		{
			if (Input.GetKeyDown(KeyCode.Alpha1+i))
			{
				Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				newPosition.z = 0f;
				activeConnector = Instantiate(connectors[i], newPosition, Quaternion.identity).GetComponent<ConnectorBase>();
				activeConnector.StartDragging();
			}
		}
    }
}
