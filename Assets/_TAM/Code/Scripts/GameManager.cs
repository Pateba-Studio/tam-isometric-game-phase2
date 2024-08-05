using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using System.Collections;
using Unity.VisualScripting;
using System.Reflection;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject inGamePanel;
    public GameObject outGamePanel;

    [Header("Interact Attribute")]
    public Animator interactAnim;
    public Button currentInteractButton;
    public Button mobileInteractButton;
    public Button desktopInteractButton;
    public bool disableSprite;

    [Header("Character Selection")]
    public Canvas nameCanvas;
    public TextMeshProUGUI nameText;
    public Movement movement;
    public Simple2DMovement simple2DMovement;
    public PlatformDetector platformDetector;
    public CameraFollow2D cameraFollow2D;
    public GameObject charSelectionPanel;
    public List<GameObject> characterObj;

    [Header("Panels Attribute")]
    public GameObject loadingAssetsPanel;
    public bool justForGettingBooth;

    [Header("Add On Logout & Quest Panel Attribute")]
    public GameObject logoutPanelObj;
    public GameObject questPanelObj;

    [Header("Add On Interact Teleport")]
    public GameObject interactTeleportPanel;
    public Button buttonSubmitInteractTeleport;
    public GameObject unlockedPanel, lockedPanel;

    [Header("Add On Direct Teleport")]
    public int targetTeleportID;
    public GameObject teleportSelectionPanel;

    [Header("Add On Hall B")]
    public TeleportState teleportStateToB;

    [Header("Add On Game Attributes")]
    public string gameScene;
    public GameObject gameClearPanel;

    [Header("Add On Game Over")]
    public GameObject gameOverParent;
    public GameObject gameOverPanel;
    public GameObject announcementPanel;
    public TextMeshProUGUI typeText;
    public TextMeshProUGUI divisionText;
    public TextMeshProUGUI contentText;

    int charIndex;

    private void Start()
    {
        instance = this;
        PlatformChecker();
        SetupFirstState();
        CharacterChecker();
        StartCoroutine(SetupGameOverPanel());
        currentInteractButton.gameObject.SetActive(true);
    }

    void SetupFirstState()
    {
        GameState.instance.SetupHallBoothIds();
        SoundManager.instance.SetFirstAudioState();
        if (DataHolder.instance.playerData.language == "id") LanguageManager.instance.SetIdLanguage();
        else LanguageManager.instance.SetEnLanguage();
    }

    void CharacterChecker()
    {
        if (DataHolder.instance.GetCurrentCharacter() == CharID.Unknown)
        {
            charSelectionPanel.gameObject.SetActive(true);
            charSelectionPanel.transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            CharacterSelection((int)DataHolder.instance.GetCurrentCharacter());
            CharacterSelection();
        }
    }

    void PlatformChecker()
    {
        if (platformDetector.isMobilePlatform)
        {
            platformDetector.WhenMobileUsed.Invoke();
            currentInteractButton = mobileInteractButton;
            DataHolder.instance.SetCurrentPlatformType(PlatformType.Mobile);
        }
        else
        {
            platformDetector.WhenDesktopUsed.Invoke();
            currentInteractButton = desktopInteractButton;
            DataHolder.instance.SetCurrentPlatformType(PlatformType.Desktop);
        }
    }

    public void SetBoothState(BoothData data, bool cond)
    {
        foreach (BoothState boothState in GameState.instance.boothStates)
        {
            if (data == boothState.boothData)
            {
                boothState.boothData.isDone = cond;
            }
        }
    }

    public void CharacterSelection(int index)
    {
        charIndex = index;
        simple2DMovement.enabled = true;
        simple2DMovement.character = characterObj[index];
        simple2DMovement.animator = characterObj[index].GetComponent<Animator>();
        FindObjectOfType<ChatBoxHandler>().playerSprite = FindObjectOfType<ChatBoxHandler>().playerSprites[index];

        nameText.text = DataHolder.instance.playerData.name;
        nameText.transform.parent.gameObject.SetActive(true);
    }

    public void CharacterSelection()
    {
        foreach (GameObject obj in characterObj)
        {
            obj.SetActive(false);
        }

        cameraFollow2D.enabled = true;
        simple2DMovement.isWork = true;
        charSelectionPanel.SetActive(false);
        simple2DMovement.character.SetActive(true);
        DataHolder.instance.SetCurrentCharacter((CharID)charIndex);

        if (!DataHolder.instance.videoMasterPosted)
        {
            APIManager.instance.PostWatchVideoMaster(GameState.instance.videoMaster.data.id, res =>
            {
                DataHolder.instance.videoMasterPosted = true;
                if (DataHolder.instance.playerData.audioState == 1)
                {
                    SoundManager.instance.UnMuteAll();
                }
            });
        }

        RefreshContentFitter((RectTransform)nameCanvas.transform);
    }

    void RefreshContentFitter(RectTransform transform)
    {
        if (transform == null || !transform.gameObject.activeSelf)
        {
            return;
        }

        foreach (RectTransform child in transform)
        {
            RefreshContentFitter(child);
        }

        var layoutGroup = transform.GetComponent<LayoutGroup>();
        var contentSizeFitter = transform.GetComponent<ContentSizeFitter>();
        if (layoutGroup != null)
        {
            layoutGroup.SetLayoutHorizontal();
            layoutGroup.SetLayoutVertical();
        }

        if (contentSizeFitter != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform);
        }
    }

    public void TeleportSelection(int index)
    {
        targetTeleportID = DataHolder.instance.currGameSessionDetail.hallDetails[index].hallId;
    }

    public void TeleportChecker(int targetTeleport)
    {
        DataHolder.instance.playerData.masterValueId = DataHolder.instance.currGameSessionDetail.hallDetails[2].hallId.ToString();
        DataHolder.instance.playerData.subMasterValueId = DataHolder.instance.currGameSessionDetail.hallDetails[2].boothDetails[0].boothId.ToString();
        DataHolder.instance.SetCurrentHall(Convert.ToInt32(DataHolder.instance.playerData.masterValueId));
        APIManager.instance.PostLastVisitedBooth();

        foreach (ProgressHallDetail progress in GameState.instance.progressHall.data)
        {
            if (progress.master_value.id == targetTeleport &&
                !progress.is_finish)
            {
                teleportStateToB.EnterGame();
                return;
            }
        }

        MultipleChoiceHandler.instance.inGamePanel.SetActive(false);
        gameClearPanel.SetActive(true);
        simple2DMovement.isWork = false;
    }

    public void TeleportSelection()
    {
        if (targetTeleportID != DataHolder.instance.currGameSessionDetail.hallDetails[2].hallId)
        {
            GameState.instance.SetupHall(targetTeleportID);
        }
        else
        {
            TeleportChecker(targetTeleportID);
        }

        teleportSelectionPanel.SetActive(false);
        targetTeleportID = 0;
    }

    public void SetQuestPanelState()
    {
        bool temp = !questPanelObj.activeSelf;
        simple2DMovement.isWork = !temp;
        questPanelObj.SetActive(temp);
    }

    public void SetLogoutPanelState()
    {
        bool temp = !logoutPanelObj.activeSelf;
        simple2DMovement.isWork = !temp;
        logoutPanelObj.SetActive(temp);
    }

    public void OpenCompliancePolicies()
    {
        string url = "https://s.id/tamSpeech";
        Application.ExternalEval("window.open('" + url + "', '_blank')");
    }

    public void AccountLogout()
    {
        string url = "https://www.tamconnect.com/logout";
        Application.ExternalEval("window.open('" + url + "', '_self')");
    }

    public void SetLoadingPanelState(string notif = null, bool isCheckpoint = false)
    {
        if (justForGettingBooth && !isCheckpoint) return;
        loadingAssetsPanel.SetActive(!loadingAssetsPanel.activeSelf);
        if (string.IsNullOrEmpty(notif)) loadingAssetsPanel.GetComponentInChildren<Text>().text = "LOADING...";
        else loadingAssetsPanel.GetComponentInChildren<Text>().text = $"LOADING...\r\n<size=30%>{notif}</size>";
    }

    public IEnumerator SetupGameOverPanel()
    {
        yield return new WaitUntil(() => GameState.instance.questCheckerForGameOver.GetCheckIconState() &&
                                         DataHolder.instance.temporaryData.character != CharID.Unknown);

        //APIManager.instance.GetVideoClosing(res =>
        //{
        //    GameState.instance.videoClosing = JsonUtility.FromJson<VideoClosing>(res);
        //    if (!GameState.instance.videoClosing.is_watched)
        //    {
        //        HTMLVideoTrigger.instance.PlayVideoExternal(GameState.instance.videoClosing.data.video);
        //    }

        //    simple2DMovement.SetIsWork(gameOverPanel.activeSelf);
        //    inGamePanel.SetActive(gameOverPanel.activeSelf);
        //    gameOverPanel.SetActive(!gameOverPanel.activeSelf);
        //});

        simple2DMovement.SetIsWork(false);
        inGamePanel.SetActive(false);

        gameOverParent.SetActive(true);
        gameOverPanel.SetActive(true);
        announcementPanel.SetActive(false);
    }

    public void SetupAfterGameOver()
    {
        //APIManager.instance.PostWatchVideoClosing(GameState.instance.videoClosing.data.id, res =>
        //{
        //    APIManager.instance.SendGameOverEmail(res => 
        //    {
        //        APIManager.instance.GetAnnouncement(res =>
        //        {
        //            GameState.instance.announce = JsonUtility.FromJson<Announce>(res);
        //            typeText.text = GameState.instance.announce.data.proper_division_label;
        //            divisionText.text = GameState.instance.announce.data.user_label_result;
        //            contentText.text = GameState.instance.announce.data.proper_division_label_description;

        //            simple2DMovement.SetIsWork(announcementPanel.activeSelf);
        //            inGamePanel.SetActive(announcementPanel.activeSelf);
        //            announcementPanel.SetActive(!announcementPanel.activeSelf);
        //        });
        //    });
        //});

        APIManager.instance.SendGameOverEmail(res =>
        {
            if (DataHolder.instance.playerData.gameSession.id != "3")
            {
                APIManager.instance.GetAnnouncement(res =>
                {
                    GameState.instance.announce = JsonUtility.FromJson<Announce>(res);
                    typeText.text = GameState.instance.announce.data.proper_division_label;
                    divisionText.text = GameState.instance.announce.data.user_label_result;
                    contentText.text = GameState.instance.announce.data.proper_division_label_description;

                    simple2DMovement.SetIsWork(false);
                    inGamePanel.SetActive(false);

                    gameOverParent.SetActive(true);
                    gameOverPanel.SetActive(false);
                    announcementPanel.SetActive(true);
                });
            }
            else
            {
                simple2DMovement.SetIsWork(true);
                inGamePanel.SetActive(true);

                gameOverParent.SetActive(false);
                gameOverPanel.SetActive(false);
                announcementPanel.SetActive(false);
            }
        });
    }
}
