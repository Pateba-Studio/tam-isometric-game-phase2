using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class TutorialMasterDetail
{
    public GameObject tutorialPanel;
    public UnityEvent tutorialEvent;
}

[Serializable]
public class TutorialMaster
{
    public bool success;
    public bool is_watch;
    public string body;
}

public class TutorialMasterHandler : MonoBehaviour
{
    public static TutorialMasterHandler instance;

    public int tutorialIndex;
    public GameObject tutorialParentPanel;
    public GameObject tutorialDesktopIntroPanel;
    public GameObject tutorialMobileIntroPanel;
    public List<TutorialMasterDetail> tutorialPanels;
    public TutorialMaster tutorialMaster;

    private void Awake()
    {
        instance = this;
        StartCoroutine(SetupTutorial());
    }

    public void CloseTutorial()
    {
        APIManager.instance.PostWatchTutorialMaster(res =>
        {
            tutorialMaster.is_watch = true;
            Simple2DMovement.instance.SetIsWork(true);
            //GameManager.instance.inGamePanel.SetActive(true);
            tutorialParentPanel.SetActive(false);
        });
    }

    public void BackToIntro()
    {
        tutorialPanels[tutorialIndex].tutorialPanel.SetActive(false);
        tutorialMobileIntroPanel.SetActive(PlatformDetector.Instance.isMobilePlatform);
        tutorialDesktopIntroPanel.SetActive(!PlatformDetector.Instance.isMobilePlatform);
    }

    public void SpawnTutorial(int factor = 0)
    {
        tutorialIndex += factor;
        tutorialMobileIntroPanel.SetActive(false);
        tutorialDesktopIntroPanel.SetActive(false);

        if (tutorialIndex > 0) tutorialPanels[tutorialIndex - 1].tutorialPanel.SetActive(false);
        if (tutorialIndex < tutorialPanels.Count - 1) tutorialPanels[tutorialIndex + 1].tutorialPanel.SetActive(false);
        
        tutorialPanels[tutorialIndex].tutorialPanel.SetActive(true);
        tutorialPanels[tutorialIndex].tutorialEvent.Invoke();
        //GameManager.instance.inGamePanel.SetActive(false);
    }

    IEnumerator SetupTutorial()
    {
        APIManager.instance.GetTutorialMaster(res =>
        {
            tutorialMaster = JsonUtility.FromJson<TutorialMaster>(res);
        });

        yield return new WaitUntil(() => tutorialMaster.success);
        if (tutorialMaster.is_watch) yield break;

        yield return new WaitUntil(() => DataHolder.instance.temporaryData.character != CharID.Unknown && PlatformDetector.Instance.isDone);
        Simple2DMovement.instance.SetIsWork(false);
        tutorialParentPanel.SetActive(true);
        BackToIntro(); 
    }
}
