using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class APIManager : MonoBehaviour
{
    public static APIManager instance;

    [Header("URLs")]
    public string rootURL;
    public string postLastVisitedBooth;
    public string getLastVisitedBooth;
    public string postChangeLanguage;
    public string postChangeAudio;
    public string getBoothData;
    public string postWatchTutorial;
    public string postWatchVideoIntro;
    public string getVideoMaster;
    public string postWatchVideoMaster;
    public string postWatchVideoHall;
    public string getTutorialMaster;
    public string postWatchTutorialMaster;
    public string getVideoClosing;
    public string postWatchVideoClosing;
    public string getProgressHall;
    public string getHallIds;
    public string getBoothIds;
    public string postGameOverEmail;
    public string getAnnouncement;

    private void Awake()
    {
        instance = this;
    }

    public void PostLastVisitedBooth()
    {
        WWWForm form = new WWWForm();
        form.AddField("ticket", DataHolder.instance.playerData.ticket);
        form.AddField("sub_master_value_id", DataHolder.instance.playerData.subMasterValueId);

        string url = rootURL + postLastVisitedBooth;
        SetLoadingPanelState("Posting Last Booth", false);

        StartCoroutine(Post(form, url, res =>
        {
            // Debug.Log($"PostLastBooth || {url}\n{res}");
            SetLoadingPanelState();
        }));
    }

    public void GetLastVisitedBooth(Action<int> hallID, Action<int> boothID)
    {
        WWWForm form = new WWWForm();
        form.AddField("ticket", DataHolder.instance.playerData.ticket);

        string url = rootURL + getLastVisitedBooth;
        //SetLoadingPanelState("Getting Last Booth", true);
        
        StartCoroutine(Post(form, url, res =>
        {
            // Debug.Log($"GetLastBooth || {url}\n{res}");

            bool isExist = false;
            int hall = 0, booth = 0;
            DataHolder.instance.lastVisitedBooth = JsonUtility.FromJson<LastVisitedBooth>(res);

            foreach (BoothState boothState in GameState.instance.boothStates)
            {
                if (boothState.boothData.boothID == DataHolder.instance.lastVisitedBooth.data)
                {
                    hall = boothState.boothData.hallID;
                    booth = boothState.boothData.boothID;
                    isExist = true; 
                    break;
                }
            }

            if (hall == DataHolder.instance.currGameSessionDetail.hallDetails[2].hallId ||
                !isExist)
            {
                hall = DataHolder.instance.currGameSessionDetail.hallDetails[0].hallId;
                booth = DataHolder.instance.currGameSessionDetail.hallDetails[0].boothDetails[0].boothId;
            }

            Debug.Log($"[Game] Checkpoint: {hall}-{booth}");

            hallID(hall);
            boothID(booth);
        }));
    }

    public void PostChangeLanguage()
    {
        WWWForm form = new WWWForm();
        form.AddField("ticket", DataHolder.instance.playerData.ticket);
        form.AddField("language", DataHolder.instance.playerData.language);

        string url = rootURL + postChangeLanguage;
        SetLoadingPanelState("Posting Last Language", false);

        StartCoroutine(Post(form, url, res =>
        {
            // Debug.Log($"PostLanguage || {url}\n{res}");
            SetLoadingPanelState();
        }));
    }

    public void PostChangeAudio()
    {
        WWWForm form = new WWWForm();
        form.AddField("ticket", DataHolder.instance.playerData.ticket);
        form.AddField("audio", DataHolder.instance.playerData.audioState);

        string url = rootURL + postChangeAudio;
        SetLoadingPanelState("Posting Last Audio", false);

        StartCoroutine(Post(form, url, res =>
        {
            // Debug.Log($"PostAudio || {url}\n{res}");
            SetLoadingPanelState();
        }));
    }

    public void PostWatchTutorial(Action<string> datas)
    {
        WWWForm form = new WWWForm();
        form.AddField("ticket", DataHolder.instance.playerData.ticket);
        form.AddField("sub_master_value_id", DataHolder.instance.playerData.subMasterValueId);

        string url = rootURL + postWatchTutorial;
        SetLoadingPanelState("Posting Watch Tutorial State", false);

        StartCoroutine(Post(form, url, res =>
        {
            // Debug.Log($"PostWatchTutorial || {url}\n{res}");
            SetLoadingPanelState();
            datas(res);
        }));
    }

    public void PostWatchVideoIntro(Action<string> datas)
    {
        WWWForm form = new WWWForm();
        form.AddField("ticket", DataHolder.instance.playerData.ticket);
        form.AddField("sub_master_value_id", DataHolder.instance.playerData.subMasterValueId);

        string url = rootURL + postWatchVideoIntro;
        SetLoadingPanelState("Posting Watch Video State", false);

        StartCoroutine(Post(form, url, res =>
        {
            // Debug.Log($"PostWatchVideoIntro || {url}\n{res}");
            SetLoadingPanelState();
            datas(res);
        }));
    }

    public void GetVideoMaster(Action<string> datas)
    {
        WWWForm form = new WWWForm();
        form.AddField("ticket", DataHolder.instance.playerData.ticket);

        string url = rootURL + getVideoMaster;
        SetLoadingPanelState("Getting Video Master", false);

        StartCoroutine(Post(form, url, res =>
        {
            // Debug.Log($"GetVideoMaster || {url}\n{res}");
            SetLoadingPanelState();
            datas(res);
        }));
    }

    public void PostWatchVideoMaster(int video_id, Action<string> datas)
    {
        WWWForm form = new WWWForm();
        form.AddField("ticket", DataHolder.instance.playerData.ticket);
        form.AddField("video_id", video_id);

        string url = rootURL + postWatchVideoMaster;
        SetLoadingPanelState("Posting Watch Video State", false);

        StartCoroutine(Post(form, url, res =>
        {
            // Debug.Log($"PostWatchVideoIntro || {url}\n{res}");
            SetLoadingPanelState();
            datas(res);
        }));
    }

    public void GetTutorialMaster(Action<string> datas)
    {
        WWWForm form = new WWWForm();
        form.AddField("ticket", DataHolder.instance.playerData.ticket);
        form.AddField("game_session_id", DataHolder.instance.playerData.gameSession.id);

        string url = rootURL + getTutorialMaster;
        SetLoadingPanelState("Getting Tutorial Master", false);

        StartCoroutine(Post(form, url, res =>
        {
            // Debug.Log($"GetVideoMaster || {url}\n{res}");
            SetLoadingPanelState();
            datas(res);
        }));
    }

    public void PostWatchTutorialMaster(Action<string> datas)
    {
        WWWForm form = new WWWForm();
        form.AddField("ticket", DataHolder.instance.playerData.ticket);
        form.AddField("game_session_id", DataHolder.instance.playerData.gameSession.id);

        string url = rootURL + postWatchTutorialMaster;
        SetLoadingPanelState("Posting Watch Tutorial Master", false);

        StartCoroutine(Post(form, url, res =>
        {
            // Debug.Log($"PostWatchVideoIntro || {url}\n{res}");
            SetLoadingPanelState();
            datas(res);
        }));
    }

    public void PostWatchVideoHall(Action<string> datas)
    {
        WWWForm form = new WWWForm();
        form.AddField("ticket", DataHolder.instance.playerData.ticket);
        form.AddField("master_value_id", DataHolder.instance.playerData.masterValueId);

        string url = rootURL + postWatchVideoHall;
        SetLoadingPanelState("Posting Watch Video Hall State", false);

        StartCoroutine(Post(form, url, res =>
        {
            // Debug.Log($"PostWatchVideoIntro || {url}\n{res}");
            SetLoadingPanelState();
            datas(res);
        }));
    }

    public void GetVideoClosing(Action<string> datas)
    {
        WWWForm form = new WWWForm();
        form.AddField("ticket", DataHolder.instance.playerData.ticket);
        form.AddField("game_session_id", DataHolder.instance.playerData.gameSession.id);

        string url = rootURL + getVideoClosing;
        SetLoadingPanelState("Getting Video Closing", false);

        StartCoroutine(Post(form, url, res =>
        {
            // Debug.Log($"GetVideoMaster || {url}\n{res}");
            SetLoadingPanelState();
            datas(res);
        }));
    }

    public void PostWatchVideoClosing(int video_id, Action<string> datas)
    {
        WWWForm form = new WWWForm();
        form.AddField("ticket", DataHolder.instance.playerData.ticket);
        form.AddField("closing_id", video_id);

        string url = rootURL + postWatchVideoClosing;
        SetLoadingPanelState("Posting Watch Video Closing", false);

        StartCoroutine(Post(form, url, res =>
        {
            // Debug.Log($"PostWatchVideoIntro || {url}\n{res}");
            SetLoadingPanelState();
            datas(res);
        }));
    }

    public void GetAllBoothState(Action<string> datas)
    {
        WWWForm form = new WWWForm();
        form.AddField("ticket", DataHolder.instance.playerData.ticket);

        string url = rootURL + getBoothData;
        StartCoroutine(Post(form, url, res =>
        {
            //Debug.Log($"GetAllBoothState || {url}\n{res}");
            SetLoadingPanelState();
            datas(res);
        }));
    }

    public void GetBoothState(Action<string> datas)
    {
        WWWForm form = new WWWForm();
        form.AddField("ticket", DataHolder.instance.playerData.ticket);
        form.AddField("sub_master_value_id", DataHolder.instance.playerData.subMasterValueId);

        string url = rootURL + getBoothData;
        SetLoadingPanelState("Getting Booth State", false);

        StartCoroutine(Post(form, url, res =>
        {
            //Debug.Log($"GetBoothState || {url}\n{res}");
            SetLoadingPanelState();
            datas(res);
        }));
    }

    public void GetProgressHall(Action<string> datas)
    {
        WWWForm form = new WWWForm();
        form.AddField("ticket", DataHolder.instance.playerData.ticket);

        string url = rootURL + getProgressHall;
        SetLoadingPanelState("Getting Progress Hall", false);

        StartCoroutine(Post(form, url, res =>
        {
            // Debug.Log($"GetBoothState || {url}\n{res}");
            SetLoadingPanelState();
            datas(res);
        }));
    }

    public void GetHallID(Action<string> datas)
    {
        WWWForm form = new WWWForm();
        form.AddField("game_session_id", DataHolder.instance.playerData.gameSession.id);
        form.AddField("ticket", DataHolder.instance.playerData.ticket);

        string url = rootURL + getHallIds;
        SetLoadingPanelState("Getting Hall Id", false);

        StartCoroutine(Post(form, url, res =>
        {
            // Debug.Log($"GetBoothState || {url}\n{res}");
            SetLoadingPanelState();
            datas(res);
        }));
    }

    public void GetBoothID(Action<string> datas)
    {
        WWWForm form = new WWWForm();
        form.AddField("game_session_id", DataHolder.instance.playerData.gameSession.id);

        string url = rootURL + getBoothIds;
        SetLoadingPanelState("Getting Booth Id", false);

        StartCoroutine(Post(form, url, res =>
        {
            // Debug.Log($"GetBoothState || {url}\n{res}");
            SetLoadingPanelState();
            datas(res);
        }));
    }

    public void SendGameOverEmail(Action<string> datas)
    {
        WWWForm form = new WWWForm();
        form.AddField("ticket", DataHolder.instance.playerData.ticket);

        string url = rootURL + postGameOverEmail;
        SetLoadingPanelState("Posting GameOver Email", false);

        StartCoroutine(Post(form, url, res =>
        {
            // Debug.Log($"GetBoothState || {url}\n{res}");
            SetLoadingPanelState();
            datas(res);
        }));
    }

    public void GetAnnouncement(Action<string> datas)
    {
        WWWForm form = new WWWForm();
        form.AddField("ticket", DataHolder.instance.playerData.ticket);
        form.AddField("game_session_id", DataHolder.instance.playerData.gameSession.id);

        string url = rootURL + getAnnouncement;
        SetLoadingPanelState("Getting Announcement", false);

        StartCoroutine(Post(form, url, res =>
        {
            // Debug.Log($"GetBoothState || {url}\n{res}");
            SetLoadingPanelState();
            datas(res);
        }));
    }

    public void SetLoadingPanelState(string desc = null, bool cond = false)
    {
        if (GameManager.instance != null) 
            GameManager.instance.SetLoadingPanelState(desc, cond);
    }

    public IEnumerator Post(WWWForm form, string fullUri, Action<string> handler)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Post(fullUri, form))
        {
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    break;
                case UnityWebRequest.Result.Success:
                    handler(webRequest.downloadHandler.text);
                    break;
            }

            webRequest.Dispose();
        }
    }
}
