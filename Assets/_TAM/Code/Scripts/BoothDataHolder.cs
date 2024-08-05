using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class Game
{
    public int id;
    public string name;
    public bool is_finish;
}

[Serializable]
public class Tutorial
{
    public bool success;
    public List<TutorialData> data;
    public bool watch_tutorial;
}

[Serializable]
public class VideoIntro
{
    public bool success;
    public List<VideoIntroData> data;
    public bool watch_intro;
}

[Serializable]
public class TutorialData
{
    public int id;
    public string sub_master_value;
    public string image;
    public string title;
    public string body;
}

[Serializable]
public class VideoIntroData
{
    public int id;
    public string sub_master_value;
    public string media_type;
    public object english_media;
    public string media;
    public object audio;
    public object description;
}

[Serializable]
public class BoothDetail
{
    public SubMasterValue sub_master_value;
    public Tutorial tutorials;
    public VideoIntro intros;
    public List<Game> games;
}

[Serializable]
public class SubMasterValue
{
    public int id;
    public string name;
    public bool enabled;
}

[Serializable]
public class BoothStateDatas
{
    public bool success;
    public List<BoothDetail> data;
}

[Serializable]
public class BoothStateData
{
    public bool success;
    public BoothDetail data;
}

public class BoothDataHolder : MonoBehaviour
{
    public static BoothDataHolder instance;
    public BoothStateDatas boothStateDatas;
    public BoothStateData tempBoothStateData;

    private void Awake()
    {
        instance = this;
    }

    void SetClearBooth()
    {
        foreach (BoothState booth in GameState.instance.boothStates)
        {
            int id = tempBoothStateData.data.sub_master_value.id;
            if (id == booth.boothData.boothID)
            {
                if (GetBoothState(id))
                {
                    booth.SetConditionIsDone();
                }
            }
        }

        tempBoothStateData = null;
    }

    void SetClearBooths()
    {
        foreach (BoothDetail data in boothStateDatas.data)
        {
            foreach (BoothState booth in GameState.instance.boothStates)
            {
                int id = data.sub_master_value.id;
                if (id == booth.boothData.boothID)
                {
                    if (GetBoothsState(id))
                    {
                        booth.SetConditionIsDone();
                    }

                    if (booth.boothData.questChecker != null)
                    {
                        booth.boothData.questChecker.SetBoothName(data.sub_master_value.name);
                    }

                    foreach (TutorialData tutorial in data.tutorials.data)
                    {
                        TutorialDetail detail = new TutorialDetail();
                        detail.title = tutorial.title;
                        detail.body = tutorial.body;
                        detail.spriteUrl = tutorial.image;
                        booth.tutorialDetails.Add(detail);
                    }

                    foreach (VideoIntroData video in data.intros.data)
                    {
                        booth.videoDetails.Add(video.media);
                    }

                    foreach (Game game in data.games)
                    {
                        booth.gameDetails.Add(game);
                    }

                    booth.boothIsActive = data.sub_master_value.enabled;
                    booth.boothData.boothNameNCText.text = data.sub_master_value.name;
                    booth.boothData.boothNameCText.text = data.sub_master_value.name;
                    booth.tutorialIsDone = data.tutorials.watch_tutorial;
                    booth.videoIntroIsDone = data.intros.watch_intro;
                    booth.SetBoothState();
                }
            }
        }
    }

    bool GetBoothState(int boothID)
    {
        bool intro = true;
        if (tempBoothStateData.data.sub_master_value.id == boothID)
            intro = tempBoothStateData.data.intros.watch_intro;

        bool tutorial = true;
        if (tempBoothStateData.data.sub_master_value.id == boothID)
            tutorial = tempBoothStateData.data.tutorials.watch_tutorial;

        bool game = true;
        if (tempBoothStateData.data.sub_master_value.id == boothID)
            foreach (Game data in tempBoothStateData.data.games)
                if (!data.is_finish)
                {
                    game = false;
                    break;
                }

        return intro && tutorial && game;
    }

    bool GetBoothsState(int boothID)
    {
        bool intro = true;
        for (int i = 0; i < boothStateDatas.data.Count; i++)
            if (boothStateDatas.data[i].sub_master_value.id == boothID)
                intro = boothStateDatas.data[i].intros.watch_intro;

        bool tutorial = true;
        for (int i = 0; i < boothStateDatas.data.Count; i++)
            if (boothStateDatas.data[i].sub_master_value.id == boothID)
                tutorial = boothStateDatas.data[i].tutorials.watch_tutorial;

        bool game = true;
        for (int i = 0; i < boothStateDatas.data.Count; i++)
            if (boothStateDatas.data[i].sub_master_value.id == boothID)
                foreach (Game data in boothStateDatas.data[i].games)
                    if (!data.is_finish)
                    {
                        game = false;
                        break;
                    }

        return intro && tutorial && game;
    }

    public IEnumerator GetBoothsState(Action<int> lastBoothId)
    {
        APIManager.instance.GetAllBoothState(res =>
        {
            boothStateDatas = JsonUtility.FromJson<BoothStateDatas>(res);
            SetClearBooths();
        });

        int hallID = 0;
        int boothID = 0;
        bool isDone = false;

        yield return new WaitUntil(() => GameState.instance.isDoneSetup);

        APIManager.instance.GetLastVisitedBooth(hall =>
        {
            hallID = hall;
        }, booth =>
        {
            boothID = booth;
            isDone = true;
        });

        yield return new WaitUntil(() => isDone);

        GameState.instance.SetupHall(hallID);
        GameState.instance.SetupBooth(boothID);
        DataHolder.instance.playerData.masterValueId = hallID.ToString();
        DataHolder.instance.playerData.subMasterValueId = boothID.ToString();
        GameManager.instance.SetLoadingPanelState(null, true);

        GameState.instance.hallStates.ForEach(hall =>
        {
            hall.SetHallState();
        });

        GameState.instance.boothStates.ForEach(booth =>
        {
            booth.SetBoothState();
        });

        lastBoothId(boothID);
    }

    public IEnumerator GetBoothState()
    {
        string json = string.Empty;
        APIManager.instance.GetBoothState(res =>
        {
            json = res;
        });

        yield return new WaitUntil(() => !string.IsNullOrEmpty(json));
        tempBoothStateData = JsonUtility.FromJson<BoothStateData>(json);
        SetClearBooth();
    }
}