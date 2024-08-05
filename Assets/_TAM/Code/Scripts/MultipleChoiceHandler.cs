using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;
using UnityEngine.SceneManagement;

public class MultipleChoiceHandler : MonoBehaviour
{
    public static MultipleChoiceHandler instance;

    public Text prologueText;
    public GameObject multipleChoicePanel;
    public GameObject inGamePanel;
    public BoothState currBoothState;

    [Header("Button Attributes")]
    public Button videoBtn;
    public Button tutorialBtn;
    public Button gameBtn;

    [Header("Sprite Attributes")]
    public Image playerImagePlaceHolder;
    public Image npcImagePlaceHolder;

    [Header("Video Attributes")]
    public int videoIndex;
    public GameObject afterVideoFinishedPanel;

    [Header("Tutorial Attributes")]
    public int tutorialIndex;
    public GameObject tutorialPanel;
    public GameObject prevTutorialBtn;
    public GameObject nextTutorialBtn;
    public Image tutorialImage;
    public Text tutorialTitle;
    public Text tutorialContent;

    [Header("Game Attributes")]
    public string gameScene;
    public GameObject gameClearPanel;

    private void Awake()
    {
        instance = this;
    }

    public void SetupMultipleChoice(ChatBoxHandler chatBox, ChatBoxScriptableObject chatBoxObj)
    {
        foreach (BoothState booth in GameState.instance.boothStates)
        {
            if (booth.boothData.boothID == (Convert.ToInt32(DataHolder.instance.playerData.subMasterValueId)))
            {
                currBoothState = booth;
            }
        }
        
        npcImagePlaceHolder.sprite = chatBox.npcSprite;
        playerImagePlaceHolder.sprite = chatBox.playerSprite;

        if (chatBox.npcFlip) npcImagePlaceHolder.rectTransform.localRotation = Quaternion.Euler(0, 180, 0);
        //if (DataHolder.instance.GetCurrentLanguage() == LangID.EN) prologueText.text = chatBoxObj.prologueEnChatBox;
        //else prologueText.text = chatBoxObj.prologueIdChatBox;

        videoBtn.gameObject.SetActive(true);
        tutorialBtn.gameObject.SetActive(true);
        gameBtn.gameObject.SetActive(currBoothState.gameDetails.Count != 0);

        if (currBoothState.gameDetails.Count != 0 ||
            currBoothState.videoDetails.Count != 0 ||
            currBoothState.tutorialDetails.Count != 0)
        {
            gameBtn.interactable = currBoothState.tutorialIsDone && currBoothState.videoIntroIsDone;
            multipleChoicePanel.SetActive(true);
            inGamePanel.SetActive(false);
        }
        else
        {
            FinishMultipleChoice();
        }
    }

    IEnumerator SetupVideoPanel()
    {
        GameManager.instance.simple2DMovement.isWork = false;
        HTMLVideoTrigger.instance.PlayVideoExternal(currBoothState.videoDetails[videoIndex]);
        SoundManager.instance.MuteAll();

        yield return new WaitForSeconds(.5f);
        afterVideoFinishedPanel.SetActive(true);
    }

    public void NextVideoPlayer()
    {
        if (CanNextVideo())
        {
            videoIndex++;
            StartCoroutine(SetupVideoPanel());
        }
        else
        {
            CloseVideoPanel();
        }
    }

    public void CloseVideoPanel()
    {
        videoIndex = 0;
        multipleChoicePanel.SetActive(true);
        afterVideoFinishedPanel.SetActive(false);
        currBoothState.videoIntroIsDone = true;

        if (DataHolder.instance.playerData.audioState == 1)
            SoundManager.instance.UnMuteAll();
        
        GameManager.instance.simple2DMovement.isWork = true;
        APIManager.instance.PostWatchVideoIntro(res =>
        {
            currBoothState.videoIntroIsDone = true;
            gameBtn.interactable = currBoothState.tutorialIsDone && currBoothState.videoIntroIsDone;
        });
    }

    public bool CanNextVideo()
    {
        if (videoIndex < currBoothState.videoDetails.Count - 1) return true;
        else return false;
    }

    public void SetupTutorial()
    {
        tutorialPanel.SetActive(true);
        multipleChoicePanel.SetActive(false);
        prevTutorialBtn.SetActive(true);
        nextTutorialBtn.SetActive(true);

        if (tutorialIndex == 0) prevTutorialBtn.SetActive(false);
        if (tutorialIndex == currBoothState.tutorialDetails.Count - 1)
        {
            nextTutorialBtn.SetActive(false);
            APIManager.instance.PostWatchTutorial(res =>
            {
                currBoothState.tutorialIsDone = true;
                gameBtn.interactable = currBoothState.tutorialIsDone && currBoothState.videoIntroIsDone;
            });
        }

        tutorialTitle.text = currBoothState.tutorialDetails[tutorialIndex].title;
        tutorialContent.text = currBoothState.tutorialDetails[tutorialIndex].body;
        StartCoroutine(ImageDownloader(currBoothState.tutorialDetails[tutorialIndex].spriteUrl, res =>
        {
            tutorialImage.sprite = res;
        }));
    }

    public void PrevTutorial()
    {
        tutorialIndex--;
        SetupTutorial();
    }

    public void NextTutorial()
    {
        tutorialIndex++;
        SetupTutorial();
    }

    public void OpenGame()
    {
        bool gameIsDone = true;
        foreach (Game games in currBoothState.gameDetails)
        {
            if (!games.is_finish)
            {
                gameIsDone = false;
            }
        }

        if (!gameIsDone)
        {
            SceneManager.LoadScene(gameScene);
            // DataHolder.instance.isFirstOpen = false;
        }
        else
        {
            gameClearPanel.SetActive(true);
            multipleChoicePanel.SetActive(false);
        }
    }

    public void FinishMultipleChoice()
    {
        multipleChoicePanel.SetActive(false);
        GameManager.instance.simple2DMovement.isWork = true;
        StartCoroutine(BoothDataHolder.instance.GetBoothState());
        currBoothState = null;
    }

    public IEnumerator ImageDownloader(string imageURL, Action<Sprite> result)
    {
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(imageURL))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error downloading image: " + www.error);
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                result(Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero));
            }
        }
    }
}
