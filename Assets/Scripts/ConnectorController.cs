using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ConnectorInfo
{
	public GameObject item;
	public int amount;
}

public class ConnectorController : Singleton<ConnectorController>
{
	public List<ConnectorInfo> connectorInfoList;
	[HideInInspector]
	public List<ConnectorInfo> connectorInfoListCurrent;
	[HideInInspector]
	public ConnectorBase activeConnector = null;

	// Start is called before the first frame update
	void Start()
    {
		connectorInfoListCurrent = connectorInfoList;
	}
	public bool InstantiateConnector(int id)
	{
		if (activeConnector == null)
		{
			Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			newPosition.z = 0f;
			activeConnector = Instantiate(connectorInfoListCurrent[id].item.gameObject, newPosition, Quaternion.identity).GetComponent<ConnectorBase>();
			connectorInfoListCurrent[id].amount--;
			activeConnector.StartDragging();
			UIHelper.instance.UpdateAmount();

			return true;
		}
		return false;
	}

	void Update()
    {
		if (activeConnector == null)
		{
			for (int i = 0; i < connectorInfoListCurrent.Count; ++i)
			{
				if (Input.GetKeyDown(KeyCode.Alpha1 + i))
				{
					InstantiateConnector(i);
				}
			}
		}
	}
}
