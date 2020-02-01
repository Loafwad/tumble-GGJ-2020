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
	protected ConnectorBase activeConnector = null;
	public List<ConnectorInfo> connectorInfoList;
	public List<ConnectorInfo> connectorInfoListCurrent;

	// Start is called before the first frame update
	void Start()
    {
		connectorInfoListCurrent = connectorInfoList;
	}
	public void InstantiateConnector(int id)
	{
		Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		newPosition.z = 0f;
		activeConnector = Instantiate(connectorInfoListCurrent[id].item.gameObject, newPosition, Quaternion.identity).GetComponent<ConnectorBase>();
		connectorInfoListCurrent[id].amount--;
		activeConnector.StartDragging();
		UIHelper.instance.UpdateAmount();
	}

	void Update()
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
