using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonConnector : MonoBehaviour
{
	public GameObject textElement;
	public int id;

	public void RefreshText(bool decrement = false)
	{
		int upcommingAmmout = ConnectorController.instance.connectorInfoListCurrent[id].amount;
		if(decrement)
		{
			--upcommingAmmout;
		}
		// Check if we are about to use the last one
		//if (upcommingAmmout == 0)
		//{
		//	UIHelper.instance.RemoveContainer(id);
		//}
		if(upcommingAmmout > 0)
		{
			textElement.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = upcommingAmmout.ToString();
		}

	}

    public void ConnectorMouseClick()
	{
		bool wasRemovedFromList = ConnectorController.instance.InstantiateConnector(id);
		if(!wasRemovedFromList)
		{
			RefreshText();
		}
		else
		{
			UIHelper.instance.RemoveContainer(id);
		}
	}
}
