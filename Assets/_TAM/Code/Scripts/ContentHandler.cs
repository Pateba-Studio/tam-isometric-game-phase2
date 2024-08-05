using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;
using UnityEngine.SceneManagement;

public class ContentHandler : MonoBehaviour
{
    public static ContentHandler instance;

    public BoothState currBoothState;

    [Header("Tutorial Attributes")]
    public int tutorialIndex;
    public GameObject tutorialPanel;
    public GameObject prevTutorialBtn;
    public GameObject nextTutorialBtn;
    public Image tutorialImage;

    [Header("Video Attributes")]
    public int videoIndex;
    public GameObject afterVideoFinishedPanel;

    private void Awake()
    {
        instance = this;
    }

    public void SetupContent()
    {
        GameManager.instance.inGamePanel.SetActive(false);

        foreach (BoothState booth in GameState.instance.boothStates)
        {
            if (booth.boothData.boothID == (Convert.ToInt32(DataHolder.instance.playerData.subMasterValueId)))
            {
                currBoothState = booth;
            }
        }

        if (currBoothState.tutorialDetails.Count > 0) SetupTutorial();
        else if (currBoothState.videoDetails.Count > 0) StartCoroutine(SetupVideoPanel());
        else OpenGame();
    }

    public void SetupTutorial()
    {
        tutorialPanel.SetActive(true);
        prevTutorialBtn.SetActive(true);
        nextTutorialBtn.SetActive(true);

        if (tutorialIndex < currBoothState.tutorialDetails.Count)
        {
            if (tutorialIndex == 0)
            {
                prevTutorialBtn.SetActive(false);
            }
            if (tutorialIndex == currBoothState.tutorialDetails.Count - 1)
            {
                APIManager.instance.PostWatchTutorial(res =>
                {
                    currBoothState.tutorialIsDone = true;
                    StartCoroutine(BoothDataHolder.instance.GetBoothState());
                });
            }
        }
        else
        {
            CloseTutorial();
            if (currBoothState.videoDetails.Count > 0)
            {
                StartCoroutine(SetupVideoPanel());
            }
        }

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

    public void CloseTutorial()
    {
        tutorialIndex = 0;
        tutorialPanel.SetActive(false);
        currBoothState.videoIntroIsDone = true;
        GameManager.instance.inGamePanel.SetActive(true);
        GameManager.instance.simple2DMovement.isWork = true;
    }

    public IEnumerator SetupVideoPanel()
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

    public bool CanNextVideo()
    {
        if (videoIndex < currBoothState.videoDetails.Count - 1) return true;
        else return false;
    }

    public void CloseVideoPanel()
    {
        videoIndex = 0;
        currBoothState.videoIntroIsDone = true;
        afterVideoFinishedPanel.SetActive(false);
        GameManager.instance.inGamePanel.SetActive(true);

        if (DataHolder.instance.playerData.audioState == 1)
            SoundManager.instance.UnMuteAll();

        GameManager.instance.simple2DMovement.isWork = true;
        APIManager.instance.PostWatchVideoIntro(res =>
        {
            currBoothState.videoIntroIsDone = true;
            StartCoroutine(BoothDataHolder.instance.GetBoothState());
            OpenGame();
        });
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
            SceneManager.LoadScene(GameManager.instance.gameScene);
        }
        else
        {
            GameManager.instance.simple2DMovement.SetIsWork(false);
            GameManager.instance.inGamePanel.SetActive(false);
            GameManager.instance.gameClearPanel.SetActive(true);
        }
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
