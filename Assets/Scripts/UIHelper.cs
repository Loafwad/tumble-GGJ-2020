using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class UIHelper : Singleton<UIHelper>
{
    public LevelController levelController;
    public TextMeshProUGUI buttonText;

	public Canvas canvas;
	public GameObject UiContainer;
	protected int curAmount;
	private List<GameObject> containerList;
	public GameObject winLevelUIObject;
	public GameObject failLevelUIObject;

	//play & stop button
	public void PlayButton()
    {
        levelController.RunSimulation(!levelController.simulationRunning);
        if (!levelController.simulationRunning)
        {
            buttonText.text = "Play";
        }
        else
        {
            buttonText.text = "Paused";
        }
    }

	private void Start()
	{
		containerList = new List<GameObject>();
		winLevelUIObject.SetActive(false);
		failLevelUIObject.SetActive(false);
		GetAmount();
	}

	public void UpdateAmount()
	{
		UiContainer.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = curAmount.ToString();
	}

	public void SpawnUIContainers(int id)
	{
		GameObject newElement = Instantiate(UiContainer, canvas.transform.Find("Background-Left"));
		containerList.Add(newElement);
		newElement.GetComponent<ButtonConnector>().id = id;
	}

	public void RemoveContainer(int id)
	{
		Destroy(containerList[id]);
	}

	public void GetAmount()
	{
		for (int i = 0; i < ConnectorController.instance.connectorInfoListCurrent.Count; i++)
		{
			curAmount = ConnectorController.instance.connectorInfoListCurrent[i].amount;
			UiContainer.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = curAmount.ToString();
			SpawnUIContainers(i);
		}
	}

	//couldn't think of a better solution
	public void DisplayWin()
	{
		winLevelUIObject.SetActive(true);
		failLevelUIObject.SetActive(false);
	}

	public void DisplayFail()
	{
		winLevelUIObject.SetActive(false);
		failLevelUIObject.SetActive(true);
	}
	
	public void ResetWinOrFailUI()
	{
		winLevelUIObject.SetActive(false);
		failLevelUIObject.SetActive(false);
	}
}
