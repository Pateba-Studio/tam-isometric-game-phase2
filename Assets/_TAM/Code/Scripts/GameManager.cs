using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using Assets.SimpleLocalization.Scripts;
using System.Reflection;

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

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("General UI")]
    public Button interactButton;
    public GameObject gameCanvas;
    public GameObject charSelectionPanel;
    public GameObject loadingPanel;
    public List<TextMeshProUGUI> loadingText;

    [Header("Dialogue Box")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueTitleText;
    public TextMeshProUGUI dialogueContentText;
    public List<Button> dialogueButtons;
    public List<Image> charImages;
    public List<Sprite> charSprites;

    [Header("Question Panel")]
    public GameObject questionPanel;
    public TextMeshProUGUI questionText;
    public List<AnswerButtonData> answerButtons;
    public List<RoleplayQuestion> currentRoleplayQuestions;

    [Header("Data List")]
    public List<MasterValueHandlerData> masterValueHandlers;
    public List<CharacterModelData> charModels;

    int currentGameIndex;
    int currentAnswerIndex;
    int currentCharIndex;
    int currentDialogueIndex;
    HallBoothData currentHallBoothData;
    Question currentQuestion;

    private void Awake()
    {
        instance = this;
    }

    public void SetLoadingText(string text)
    {
        foreach (var tmp in loadingText)
        {
            var match = Regex.Match(tmp.text, @"\.{1,3}$");
            tmp.text = match.Success ? text + match.Value : text;
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
        GameManager.instance.SetLoadingText("Getting All Booth Data");
        masterValueHandlers.Find(res => res.masterValueHandler == handler).isDone = true;
        if (masterValueHandlers.Any(res => !res.isDone)) return;

        loadingPanel.SetActive(false);
        OpenCharSelection();
    }

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
        DataHandler.instance.isPlaying = true;
        charSelectionPanel.SetActive(false);
        gameCanvas.SetActive(true);
    }
    #endregion

    #region Hall & Booth System
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
            if (currentDialogueIndex == currentHallBoothData.contentId.Count)
                button.gameObject.SetActive(true);
            else
                button.gameObject.SetActive(false);
        });

        if (currentDialogueIndex == currentHallBoothData.contentId.Count)
        {
            SetDialogue(true);
            dialogueButtons[0].onClick.AddListener(() =>
            {
                string json = $"{{\"ticket_number\":\"{DataHandler.instance.GetUserTicket()}\"," +
                              $"\"master_value_id\":{currentHallBoothData.masterValueId}}}";

                SetLoadingText("Getting Video");
                loadingPanel.SetActive(true);
                StartCoroutine(APIManager.instance.PostDataCoroutine(
                    APIManager.instance.SetupMasterValueIntro(),
                    json, res =>
                    {
                        DataHandler.instance.masterValueIntro = JsonUtility.FromJson<MasterValueIntro>(res);
                        VideoController.instance.PlayVideo(DataHandler.instance.masterValueIntro.intro.video);
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
        currentRoleplayQuestions = new List<RoleplayQuestion>();
        MasterValueHandler handler = new MasterValueHandler();
        
        foreach (var item in instance.masterValueHandlers)
        {
            if (item.masterValueHandler.masterValueData.id == data.masterValueId)
            {
                handler = item.masterValueHandler;
                break;
            }
        }

        foreach (var booth in handler.booth.booths)
        {
            foreach (var item in booth.question.roleplay_questions)
            {
                currentRoleplayQuestions.Add(item);
            }
        }

        SetupGame();
    }

    public void SetupGame()
    {
        if (currentGameIndex == currentRoleplayQuestions.Count)
        {
            questionPanel.SetActive(false);
            return;
        }

        switch(currentRoleplayQuestions[currentGameIndex].type)
        {
            case "video":
                VideoController.instance.PlayVideo(
                    currentRoleplayQuestions[currentGameIndex].video_path,
                    SetupGame
                    );
                break;
            case "game_question":
                currentQuestion = currentRoleplayQuestions[currentGameIndex].question;
                SetupQuestion();
                break;
        }

        currentGameIndex++;
    }

    public void SetupQuestion()
    {
        questionPanel.SetActive(true);
        questionText.text = currentQuestion.question;
        
        for (int i = 0; i < answerButtons.Count; i++)
        {
            answerButtons[i].selectedBG.SetActive(false);
            answerButtons[i].unselectedBG.SetActive(true);
            answerButtons[i].unselectedText.text = answerButtons[i].selectedText.text = 
                currentQuestion.answers[i].answer;
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

    public void SubmitAnswer()
    {
        questionPanel.SetActive(false);
        VideoController.instance.PlayVideo(
            currentQuestion.answers[currentAnswerIndex].video_path,
            SetupGame, !(currentGameIndex == currentRoleplayQuestions.Count)
            );
    }
    #endregion
}
