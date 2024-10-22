using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using System.Text.RegularExpressions;
using Assets.SimpleLocalization.Scripts;
using TMPro;
using System.Security.Cryptography;

[Serializable]
public class CharacterModelData
{
    public Button selectionButton;
    public GameObject showcaseOption;
    public GameObject playerCharacter;
}

[Serializable]
public class MasterValueHandlerData
{
    public MasterValueHandler masterValueHandler;
    public bool isDone;
}

[Serializable]
public class AnswerButtonData
{
    public GameObject unselectedBG;
    public GameObject selectedBG;
    [Space]
    public TextMeshProUGUI unselectedText;
    public TextMeshProUGUI selectedText;
}

[Serializable]
public class HallData
{
    public string hallKey;
    public GameObject missionList;
    public GameObject boothCheckpoint;
    public GameObject hallObject;
    [Space]
    public Transform teleportTransform;
}

[Serializable]
public class BoothCheckData
{
    public Sprite boothCheckDone;
    public Sprite boothCheckUndone;
}

public enum GameType
{
    Default, Simple
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public PlayerController playerController;

    [Header("General UI")]
    public Button interactButton;
    public Button changeLangButton;
    public GameObject gameCanvas;
    public GameObject charSelectionPanel;
    public GameObject loadingPanel;
    public List<TextMeshProUGUI> loadingText;

    [Header("Tutorial UI")]
    public int tutorialIndex;
    public GameObject tutorialPanel;
    public List<GameObject> tutorialPanels;

    [Header("Dialogue Box")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueTitleText;
    public TextMeshProUGUI dialogueContentText;
    public List<Button> dialogueButtons;
    public List<Image> charImages;
    public List<Sprite> charSprites;

    [Header("Question Panel")]
    public GameObject questionPanel;
    public GameObject questionOnlyPanel;
    public GameObject answerOnlyPanel;
    public List<Image> questionBackgrounds;
    public List<TextMeshProUGUI> questionText;
    public List<AnswerButtonData> answerButtons;
    [Space]
    public GameObject resultAnswerPanel;
    public Button resultAnswerNextButton;
    public TextMeshProUGUI resultAnswerTitleText;
    public TextMeshProUGUI resultAnswerText;
    public TextMeshProUGUI adviceContentText;

    [Header("Hall & Booth Attributes")]
    public GameObject boothCheckpointParent;
    public List<BoothCheckData> boothCheckDatas;
    public List<HallData> hallDatas;

    [Header("Data List")]
    public RoleplayQuestion currentRoleplayQuestion;
    public List<RoleplayQuestion> currentRoleplayQuestions;
    public List<MasterValueHandlerData> masterValueHandlers;
    public List<CharacterModelData> charModels;
    [HideInInspector] public int currentCharIndex;

    int currentGameIndex;
    int currentAnswerIndex;
    int currentDialogueIndex;
    bool initLoading;
    string teleportTarget;
    HallBoothData currentHallBoothData;

    private void Awake()
    {
        instance = this;
    }

    #region General
    public void ChangeLanguage()
    {
        changeLangButton.interactable = false;
        switch (DataHandler.instance.playerData.language)
        {
            case "en":
                DataHandler.instance.playerData.language = "id";
                break;
            case "id":
                DataHandler.instance.playerData.language = "en";
                break;
        }

        string json = $"\"{{\"ticket_number\":\"{DataHandler.instance.GetUserTicket()}\"," +
                      $"\"language\":\"{DataHandler.instance.playerData.language}\"}}\"";
        StartCoroutine(APIManager.instance.PostDataCoroutine(
            APIManager.instance.SetupChangeLanguage(),
            json, res =>
            {
                changeLangButton.interactable = true;
                switch (DataHandler.instance.playerData.language)
                {
                    case "en":
                        LocalizationManager.Language = "en-US";
                        break;
                    case "id":
                        LocalizationManager.Language = "id-ID";
                        break;
                }
            }));
    }

    public void SetLoadingText(string text)
    {
        foreach (var tmp in loadingText)
        {
            var match = Regex.Match(tmp.text, @"\.{1,3}$");
            tmp.text = match.Success ? text + match.Value : text;
        }
    }

    public void SetTutorialPanel(int index)
    {
        tutorialPanel.SetActive(true);
        if (tutorialIndex + index < 0) return;
        if (tutorialIndex + index == tutorialPanels.Count)
        {
            string json = $"{{\"ticket_number\":\"{DataHandler.instance.GetUserTicket()}\"}}";
            StartCoroutine(APIManager.instance.PostDataCoroutine(
                APIManager.instance.SetupStoreTutorial(),
                json, res =>
                {
                    DataHandler.instance.playerData.have_seen_tutorial = true;
                    foreach (var item in tutorialPanels) item.SetActive(false);
                    tutorialPanel.SetActive(false);
                    tutorialIndex = 0;
                }));
        }
        else
        {
            tutorialIndex += index;
            foreach (var item in tutorialPanels) item.SetActive(false);
            tutorialPanels[tutorialIndex].SetActive(true);
        }
    }

    public void SetupInteractButton(bool open, UnityEvent whenInteract)
    {
        string param = open ? "On" : "Off";
        interactButton.GetComponent<Animator>().Play(param);

        interactButton.onClick.RemoveAllListeners();
        interactButton.onClick.AddListener(() => whenInteract.Invoke());
    }

    public void SetMasterValueState(MasterValueHandler handler)
    {
        //SetLoadingText("Getting All Booth Data");
        masterValueHandlers.Find(res => res.masterValueHandler == handler).isDone = true;
        if (masterValueHandlers.Any(res => !res.isDone)) return;

        if (!initLoading)
        {
            initLoading = true;
            loadingPanel.SetActive(false);
            TeleportHall(PreloadManager.instance.defaultHall);
            OpenCharSelection();
        }
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

    public static string ConvertHTMLToTMPRO(string htmlText)
    {
        // Mengonversi <b> dan </b> menjadi <b> dan </b> yang didukung oleh TMP
        htmlText = Regex.Replace(htmlText, @"<b>", "<b>");
        htmlText = Regex.Replace(htmlText, @"</b>", "</b>");

        // Mengonversi <i> dan </i> menjadi <i> dan </i> yang didukung oleh TMP
        htmlText = Regex.Replace(htmlText, @"<i>", "<i>");
        htmlText = Regex.Replace(htmlText, @"</i>", "</i>");

        // Mengonversi <u> dan </u> menjadi <u> dan </u> yang didukung oleh TMP
        htmlText = Regex.Replace(htmlText, @"<u>", "<u>");
        htmlText = Regex.Replace(htmlText, @"</u>", "</u>");

        // Mengonversi tag <color> dan </color>
        htmlText = Regex.Replace(htmlText, @"<color=(.*?)>", "<color=$1>");
        htmlText = Regex.Replace(htmlText, @"</color>", "</color>");

        // Mengonversi <br> menjadi baris baru \n yang didukung oleh TMP
        htmlText = Regex.Replace(htmlText, @"<br\s*/?>", "\n");

        // Mengonversi <p> dan </p> menjadi \n yang didukung oleh TMP
        htmlText = Regex.Replace(htmlText, @"<p>", "");
        htmlText = Regex.Replace(htmlText, @"</p>", "\n");

        // Mengonversi <ol> menjadi baris kosong di awal (untuk keteraturan list)
        htmlText = Regex.Replace(htmlText, @"<ol>", "");
        htmlText = Regex.Replace(htmlText, @"</ol>", "\n");

        // Mengonversi <ul> menjadi baris kosong di awal (untuk keteraturan list)
        htmlText = Regex.Replace(htmlText, @"<ul>", "");
        htmlText = Regex.Replace(htmlText, @"</ul>", "\n");

        // Mengonversi <li> menjadi bullet point atau nomor manual
        htmlText = Regex.Replace(htmlText, @"<li>", "• ");
        htmlText = Regex.Replace(htmlText, @"</li>", "\n");

        // Mengonversi <span> menjadi tag <color> atau format lain yang diperlukan
        htmlText = Regex.Replace(htmlText, @"<span.*?>", ""); // Menghilangkan tag <span> tanpa properti
        htmlText = Regex.Replace(htmlText, @"</span>", "");

        // Mengonversi <h1> sampai <h6> menjadi size yang lebih besar
        htmlText = Regex.Replace(htmlText, @"<h1>", "<size=150%>");
        htmlText = Regex.Replace(htmlText, @"</h1>", "</size>\n");
        htmlText = Regex.Replace(htmlText, @"<h2>", "<size=140%>");
        htmlText = Regex.Replace(htmlText, @"</h2>", "</size>\n");
        htmlText = Regex.Replace(htmlText, @"<h3>", "<size=130%>");
        htmlText = Regex.Replace(htmlText, @"</h3>", "</size>\n");
        htmlText = Regex.Replace(htmlText, @"<h4>", "<size=120%>");
        htmlText = Regex.Replace(htmlText, @"</h4>", "</size>\n");
        htmlText = Regex.Replace(htmlText, @"<h5>", "<size=110%>");
        htmlText = Regex.Replace(htmlText, @"</h5>", "</size>\n");
        htmlText = Regex.Replace(htmlText, @"<h6>", "<size=100%>");
        htmlText = Regex.Replace(htmlText, @"</h6>", "</size>\n");

        // Mengonversi <a href> menjadi teks biasa, karena TMP tidak mendukung link HTML langsung
        htmlText = Regex.Replace(htmlText, @"<a\s+href=[""'](.*?)[""'].*?>", "");
        htmlText = Regex.Replace(htmlText, @"</a>", "");

        // Menghapus semua tag HTML lainnya yang tidak didukung oleh TMP
        htmlText = Regex.Replace(htmlText, @"<[^>]+>", "");

        return htmlText;
    }
    #endregion

    #region Character Selection
    public void OpenCharSelection()
    {
        charSelectionPanel.SetActive(true);
        charModels[currentCharIndex].selectionButton.onClick.Invoke();
        DataHandler.instance.isPlaying = false;
    }

    public void SetCharacterModel(int index)
    {
        currentCharIndex = index;
        foreach (var item in charModels)
        {
            item.selectionButton.transform.GetChild(0).gameObject.SetActive(false);
            item.showcaseOption.SetActive(false);
            item.playerCharacter.SetActive(false);
        }

        charModels[index].selectionButton.transform.GetChild(0).gameObject.SetActive(true);
        charModels[index].showcaseOption.SetActive(true);
        charModels[index].playerCharacter.SetActive(true);
    }

    public void SetCharacterModelLeftRight(int factor)
    {
        if (currentCharIndex + factor < 0) return;
        if (currentCharIndex + factor > charModels.Count - 1) return;
        SetCharacterModel(currentCharIndex + factor);
    }

    public void SubmitCharacter()
    {
        if (!DataHandler.instance.playerData.have_seen_tutorial) SetTutorialPanel(0);
        DataHandler.instance.isPlaying = true;
        charSelectionPanel.SetActive(false);
        gameCanvas.SetActive(true);
    }
    #endregion

    #region
    public void SetTeleportTarget(string target)
    {
        teleportTarget = target;
    }

    public void SubmitTeleportTarget()
    {
        TeleportHall(teleportTarget);
    }
    #endregion

    #region Hall & Booth System
    public void TeleportHall(string key)
    {
        foreach (var item in hallDatas)
        {
            item.hallObject.SetActive(false);
            if (item.boothCheckpoint != null) 
                item.boothCheckpoint.SetActive(false);
        }

        foreach (var target in hallDatas)
        {
            if (target.hallKey != key) continue;
            target.hallObject.SetActive(true);

            if (key == "Main") 
                boothCheckpointParent.SetActive(false);
            else
            {
                target.boothCheckpoint.SetActive(true);
                boothCheckpointParent.SetActive(true); 
            }

            playerController.transform.SetParent(target.teleportTransform.parent);
            playerController.transform.localPosition = target.teleportTransform.localPosition;
        }
    }

    public void SetDialogue(bool finalDialogue)
    {
        charImages[0].sprite = charSprites.Find(sprite => sprite.name.Contains(currentHallBoothData.NPCCharKey));
        charImages[1].sprite = charSprites[currentCharIndex];

        switch (LocalizationManager.Language)
        {
            case "en-US":
                dialogueTitleText.text = currentHallBoothData.titleEn;
                dialogueContentText.text = finalDialogue ? currentHallBoothData.finalContentEn : 
                    currentHallBoothData.contentEn[currentDialogueIndex];
                break;
            case "id-ID":
                dialogueTitleText.text = currentHallBoothData.titleId;
                dialogueContentText.text = finalDialogue ? currentHallBoothData.finalContentId : 
                    currentHallBoothData.contentId[currentDialogueIndex];
                break;
        }
    }

    public void NextDialogue()
    {
        if (currentDialogueIndex + 1 >= currentHallBoothData.contentId.Count + 1) return;

        currentDialogueIndex++;
        SetupBooth(null, false);
    }
    
    public void CloseDialogue()
    {
        dialoguePanel.SetActive(false);
        gameCanvas.SetActive(true);
        currentDialogueIndex = 0;
    }

    public void SetupBooth(HallBoothData data, bool reset)
    {
        if (reset) currentDialogueIndex = 0;
        if (data != null) currentHallBoothData = data;

        dialogueButtons.ForEach(button =>
        {
            button.onClick.RemoveAllListeners();
            //if (currentDialogueIndex == currentHallBoothData.contentId.Count)
            //    button.gameObject.SetActive(true);
            //else
            //    button.gameObject.SetActive(false);
        });

        if (currentDialogueIndex == currentHallBoothData.contentId.Count)
        {
            SetDialogue(true);
            dialogueButtons[0].onClick.AddListener(() =>
            {
                string json = $"{{\"ticket_number\":\"{DataHandler.instance.GetUserTicket()}\"," +
                              $"\"master_value_id\":{currentHallBoothData.masterValueId}}}";

                //SetLoadingText("Getting Video");
                //loadingPanel.SetActive(true);
                StartCoroutine(APIManager.instance.PostDataCoroutine(
                    APIManager.instance.SetupMasterValueIntro(),
                    json, res =>
                    {
                        DataHandler.instance.masterValueIntro = JsonUtility.FromJson<MasterValueIntro>(res);
                        VideoController.instance.PlayVideo(0, DataHandler.instance.masterValueIntro.intro.video);
                        loadingPanel.SetActive(false);
                    }));
            });
        }
        else if (currentDialogueIndex < currentHallBoothData.contentId.Count)
        {
            SetDialogue(false);
        }

        dialoguePanel.SetActive(true);
        gameCanvas.SetActive(false);
    }
    #endregion

    #region Game
    public void SetupRoleplay(HallBoothData data)
    {
        currentGameIndex = 0;
        currentHallBoothData = data;
        currentRoleplayQuestions = new List<RoleplayQuestion>();
        MasterValueHandler handler = null;

        foreach (var item in instance.masterValueHandlers)
        {
            foreach (var booth in item.masterValueHandler.booth.booths)
            {
                if (data.GetGameBooth().gameBoothIds.Contains(booth.id))
                {
                    handler = item.masterValueHandler;
                    break;
                }
            }

            if (handler != null)
                break;
        }

        if (handler != null)
        {
            foreach (var booth in handler.booth.booths)
            {
                foreach (var item in booth.question.roleplay_questions)
                {
                    item.curr_booth_name = booth.name;
                    currentRoleplayQuestions.Add(item);
                }
            }
        }

        SetupGame();
    }

    public void SetupGame()
    {
        resultAnswerPanel.transform.GetChild(1).gameObject.SetActive(false);
        resultAnswerPanel.transform.GetChild(2).gameObject.SetActive(false);
        resultAnswerPanel.SetActive(false);

        if (currentGameIndex == currentRoleplayQuestions.Count)
        {
            questionPanel.SetActive(false);
            return;
        }

        switch(currentRoleplayQuestions[currentGameIndex].type)
        {
            case "video":
                VideoController.instance.PlayVideo(
                    currentRoleplayQuestions[currentGameIndex].id,
                    currentRoleplayQuestions[currentGameIndex].video_path,
                    SetupGame
                    );
                break;
            case "game_question":
                currentRoleplayQuestion = currentRoleplayQuestions[currentGameIndex];
                SetupQuestion();
                break;
        }

        currentGameIndex++;
    }

    public void SetupQuestion()
    {
        questionPanel.SetActive(true);
        loadingPanel.SetActive(true);
        SetLoadingText("Please Wait");

        if (!string.IsNullOrEmpty(currentRoleplayQuestion.question.background))
        {
            StartCoroutine(
                APIManager.instance.DownloadImageCoroutine(
                    currentRoleplayQuestion.question.background, res =>
                    {
                        AfterDownloadQuestionBackground(res);
                    }));
        }
        else
        {
            AfterDownloadQuestionBackground(null);
        }
    }

    public void AfterDownloadQuestionBackground(Sprite sprite)
    {
        if (sprite != null) questionBackgrounds.ForEach(a => a.sprite = sprite);
        questionText.ForEach(text => text.text = currentRoleplayQuestion.question.question);

        loadingPanel.SetActive(false);
        questionOnlyPanel.SetActive(true);
        answerOnlyPanel.SetActive(false);

        for (int i = 0; i < answerButtons.Count; i++)
        {
            answerButtons[i].selectedBG.SetActive(false);
            answerButtons[i].unselectedBG.SetActive(true);
            answerButtons[i].unselectedText.text = answerButtons[i].selectedText.text =
                currentRoleplayQuestion.question.answers[i].answer;
        }

        ChooseAnswer(0);
    }

    public void ChooseAnswer(int index)
    {
        currentAnswerIndex = index;
        for (int i = 0; i < answerButtons.Count; i++)
        {
            answerButtons[i].selectedBG.SetActive(false);
            answerButtons[i].unselectedBG.SetActive(true);

            if (i == index) 
            { 
                answerButtons[i].selectedBG.SetActive(true); 
                answerButtons[i].unselectedBG.SetActive(false); 
            }
        }
    }

    public void SetupAdvice()
    {
        if (string.IsNullOrEmpty(currentRoleplayQuestion.question.advice))
        {
            Debug.Log("Setup Game");
            SetupGame();
        }
        else
        {
            Debug.Log("Setup Advice");
            resultAnswerPanel.transform.GetChild(1).gameObject.SetActive(false);
            resultAnswerPanel.transform.GetChild(2).gameObject.SetActive(true);
            adviceContentText.text = ConvertHTMLToTMPRO(currentRoleplayQuestion.question.advice);
        }
    }

    public void SubmitAnswer()
    {
        PreloadManager.instance.SetupInitData();
        string json = $"{{\"ticket_number\":\"{DataHandler.instance.GetUserTicket()}\"," +
              $"\"roleplay_question_id\":{currentRoleplayQuestion.id}," +
              $"\"answer_id\":{currentRoleplayQuestion.question.answers[currentAnswerIndex].id}}}";
        StartCoroutine(APIManager.instance.PostDataCoroutine(
            APIManager.instance.SetupSubmitAnswer(),
            json, res =>
            {
                questionPanel.SetActive(false);
                resultAnswerPanel.SetActive(true);

                resultAnswerPanel.transform.GetChild(1).gameObject.SetActive(true);
                resultAnswerPanel.transform.GetChild(2).gameObject.SetActive(false);

                resultAnswerNextButton.interactable = false;
                resultAnswerTitleText.text = currentRoleplayQuestion.curr_booth_name;
                resultAnswerText.text = ConvertHTMLToTMPRO(currentRoleplayQuestion.
                    question.answers[currentAnswerIndex].response_dialogue);

                UnityEvent events = new();
                events.AddListener(() => resultAnswerNextButton.interactable = true);
                StartCoroutine(AudioManager.instance.PlayAudioFromURL(
                    currentRoleplayQuestion.question.answers[currentAnswerIndex].audio_path,
                    events
                    ));
            }));
    }
    #endregion
}