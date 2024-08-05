using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

[Serializable]
public class BoothData
{
    public bool isDone;
    public int hallID;
    public int boothID;
    public QuestChecker questChecker;
    public Transform playerInitPos;

    [Header("Booth Identity")]
    public GameObject boothInteractIcon;
    public GameObject boothLabel;
    public GameObject boothCheckIndicator;
    public TextMeshProUGUI boothNameNCText;
    public TextMeshProUGUI boothNameCText;

    [Header("Clear & Not Clear")]
    public GameObject notClearIcon;
    public GameObject notClearLabel;
    public GameObject clearIcon;
    public GameObject clearLabel;
}

[Serializable]
public class TutorialDetail
{
    public string spriteUrl;
    [TextArea(3, 10)] public string title;
    [TextArea(3, 10)] public string body;
}

public class BoothState : MonoBehaviour
{
    public BoothData boothData;
    public List<string> videoDetails;
    public List<TutorialDetail> tutorialDetails;
    public List<Game> gameDetails;

    [Header("Conditions")]
    public bool boothIsActive;
    public bool tutorialIsDone;
    public bool videoIntroIsDone;

    public bool CheckAllBoothStateBefore()
    {
        List<BoothState> boothStates = GameState.instance.hallStates.Find(hall => hall.hallData.hallID == boothData.hallID).hallData.booths;
        int currBoothId = boothStates.FindIndex(booth => booth.boothData.boothID == boothData.boothID);

        for (int i = 0; i < currBoothId; i++)
        {
            if (!boothStates[i].boothData.isDone)
            {
                return false;   
            }
        }

        return true;
    }

    public void SetSubMasterValueId()
    {
        DataHolder.instance.playerData.masterValueId = boothData.hallID.ToString();
        DataHolder.instance.playerData.subMasterValueId = boothData.boothID.ToString();
        APIManager.instance.PostLastVisitedBooth();
    }

    public void SetConditionIsDone()
    {
        boothData.isDone = true;

        boothData.notClearIcon.SetActive(false);
        boothData.notClearLabel.SetActive(false);
        boothData.clearIcon.SetActive(true);
        boothData.clearLabel.SetActive(true);
        boothData.boothCheckIndicator.transform.GetChild(0).gameObject.SetActive(true);
        
        if (boothData.questChecker != null)
            boothData.questChecker.SetCheckIconState(true);

        GameManager.instance.SetBoothState(boothData, true);
    }

    public void SetBoothState()
    {
        if (!boothIsActive) SetConditionIsDone();
        if (boothData.questChecker != null) boothData.questChecker.gameObject.SetActive(boothIsActive);

        GetComponent<InteractHandler>().isInteractable = boothIsActive;
        boothData.boothCheckIndicator.SetActive(boothIsActive);
        boothData.boothInteractIcon.SetActive(boothIsActive);
        boothData.boothLabel.SetActive(boothIsActive);
    }
}
