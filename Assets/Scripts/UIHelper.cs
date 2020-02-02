using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class UIHelper : Singleton<UIHelper>
{
    public TextMeshProUGUI buttonText;

	public Canvas canvas;
	public GameObject UiContainer;
	protected int curAmount;
	private List<GameObject> containerList;
	public GameObject winLevelScreen;
	public Animator FailTextAnimation;
	public Animator FailButtonAnimation;

	public Animator WinTextAnimation;
	public Animator WinButtonAnimation;

	public GameObject failLevelScreen;

	//play & stop button
	public void PlayButton()
    {
		LevelController.instance.RunSimulation(!LevelController.instance.simulationRunning);
        if (!LevelController.instance.simulationRunning)
        {
            buttonText.text = "Play";
        }
        else
        {
            buttonText.text = "Paused";
        }
    }

	public void LoadNextLevel()
	{
		LevelController.instance.GetComponent<LevelManager>().NextScene();
	}

	public void Restart()
	{
		LevelController.instance.RunSimulation(false);
	}

	private void Start()
	{
		containerList = new List<GameObject>();
		winLevelScreen.SetActive(false);
		failLevelScreen.SetActive(false);
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
		winLevelScreen.SetActive(true);
		failLevelScreen.SetActive(false);
		WinButtonAnimation.GetComponent<Animator>().SetTrigger("PlayAnimation");
		WinTextAnimation.GetComponent<Animator>().SetTrigger("PlayAnimation");
	}

	public void DisplayFail()
	{
		winLevelScreen.SetActive(false);
		failLevelScreen.SetActive(true);
		FailButtonAnimation.GetComponent<Animator>().SetTrigger("PlayAnimation");
		FailTextAnimation.GetComponent<Animator>().SetTrigger("PlayAnimation");
	}

	public void ResetWinOrFailUI()
	{
		winLevelScreen.SetActive(false);
		failLevelScreen.SetActive(false);
	}
}
