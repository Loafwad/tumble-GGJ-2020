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
	[HideInInspector]
	public GameObject lastConnectorPrefabInstanciated = null;

	// Start is called before the first frame update
	void Start()
    {
		connectorInfoListCurrent = connectorInfoList;
	}
	public bool InstantiateConnector(int id)
	{
		if (activeConnector == null)
		{
			lastConnectorPrefabInstanciated = connectorInfoListCurrent[id].item;
			Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			newPosition.z = 0f;
			activeConnector = Instantiate(lastConnectorPrefabInstanciated, newPosition, Quaternion.identity).GetComponent<ConnectorBase>();
			activeConnector.sourcePrefab = lastConnectorPrefabInstanciated;
			connectorInfoListCurrent[id].amount--;
			activeConnector.StartDragging();
			UIHelper.instance.UpdateAmount();

			if (connectorInfoListCurrent[id].amount == 0)
			{
				connectorInfoListCurrent.RemoveAt(id);
				return true;
			}
		}
		return false;
	}

	public void PushBackConnector(GameObject connector)
	{
		bool foundItem = false;
		for (int i = 0; i < connectorInfoListCurrent.Count; ++i)
		{
			ConnectorInfo nextInfo = connectorInfoListCurrent[i];
			if (nextInfo.item == connector)
			{
				++nextInfo.amount;
				foundItem = true;
				break;
			}
		}
		if (!foundItem)
		{
			ConnectorInfo newInfo = new ConnectorInfo();
			newInfo.amount = 1;
			newInfo.item = connector;
			connectorInfoListCurrent.Add(newInfo);
		}

		UIHelper.instance.RefreshContainers();
	}

	public void PushBackLastConnector()
	{
		if(lastConnectorPrefabInstanciated != null)
		{
			PushBackConnector(lastConnectorPrefabInstanciated);
		}
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
