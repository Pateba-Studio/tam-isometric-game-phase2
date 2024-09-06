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

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("General UI")]
    public Button interactButton;
    public GameObject loadingPanel;
    public GameObject gameCanvas;
    public GameObject charSelectionPanel;

    [Header("Dialogue Box")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueTitleText;
    public TextMeshProUGUI dialogueContentText;
    public List<Button> dialogueButtons;
    public List<Image> charImages;
    public List<Sprite> charSprites;

    [Header("Data List")]
    public List<MasterValueHandlerData> masterValueHandlers;
    public List<CharacterModelData> charModels;
    int currentCharIndex;

    private void Awake()
    {
        instance = this;
    }

    public void SetLoadingText(string text)
    {
        foreach (var tmp in loadingPanel.GetComponentsInChildren<TextMeshProUGUI>())
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

    #region Dialogue System
    public void SetDialoguePanel(DialogueData data)
    {
        dialogueButtons.ForEach(button => {
            if (data.isReceptionist) button.gameObject.SetActive(true);
            else button.gameObject.SetActive(false);
        });

        charImages[0].sprite = charSprites.Find(sprite => sprite.name.Contains(data.NPCCharKey));
        charImages[1].sprite = charSprites[currentCharIndex];

        switch (LocalizationManager.Language)
        {
            case "en-US":
                dialogueTitleText.text = data.titleEn;
                dialogueContentText.text = data.contentEn[0];
                break;
            case "id-ID":
                dialogueTitleText.text = data.titleId;
                dialogueContentText.text = data.contentId[0];
                break;
        }

        dialoguePanel.SetActive(true);
    }
    #endregion
}
