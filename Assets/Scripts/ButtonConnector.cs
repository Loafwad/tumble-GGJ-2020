using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonConnector : MonoBehaviour
{
	public GameObject textElement;
	public int id;

    public void ConnectorMouseClick()
	{
		ConnectorController.instance.InstantiateConnector(id);
		int x;
		x = ConnectorController.instance.connectorInfoListCurrent[id].amount;
		if (x <= 0)
		{
			UIHelper.instance.RemoveContainer(id);
		}
		else
		{
			textElement.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = x.ToString();
		}
	}
}
