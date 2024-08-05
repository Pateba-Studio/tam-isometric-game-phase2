using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[Serializable]
public class ProgressHallDetail
{
    public MasterValue master_value;
    public bool is_finish;
}

[Serializable]
public class MasterValue
{
    public int id;
    public string name;
}

[Serializable]
public class ProgressHall
{
    public bool success;
    public List<ProgressHallDetail> data;
    public string message;
}

[Serializable]
public class VideoMasterData
{
    public int id;
    public string video;
    public bool is_watched;
}

[Serializable]
public class VideoMaster
{
    public bool success;
    public VideoMasterData data;
    public string message;
}

[Serializable]
public class VideoClosingData
{
    public int id;
    public string game_session;
    public string video;
}

[Serializable]
public class VideoClosing
{
    public bool success;
    public VideoClosingData data;
    public bool is_watched;
}

[Serializable]
public class AnnounceData
{
    public string user_label_result;
    public string proper_division_label;
    public string announce;
    public string proper_division_label_description;
}

[Serializable]
public class Announce
{
    public bool success;
    public AnnounceData data;
    public string message;
}

public class GameState : MonoBehaviour
{
    public static GameState instance;

    public bool isDoneSetup;
    public HallState currentHallState;
    public List<HallState> hallStates;
    public List<BoothState> boothStates;
    public List<TeleportState> teleportStates;

    [Header("Video Attributes")]
    public int videoIndex;
    public GameObject afterVideoFinishedPanel;

    [Header("Add On Quest Checker For Progress Hall")]
    public ProgressHall progressHall;
    public List<QuestChecker> questCheckerForHall;
    public QuestChecker questCheckerForGameOver;

    [Header("Add On Video Master Panel")]
    public VideoMaster videoMaster;
    public VideoClosing videoClosing;
    public Announce announce;

    [Header("Notif For Teleport Button")]
    public GameObject teleportNotif;
    public GameObject teleportPanel;

    private void Awake()
    {
        instance = this;
        foreach (HallState datas in hallStates)
        {
            datas.hallData.HallObj.SetActive(false);
            datas.hallData.HallIndicatorPanel.SetActive(false);
        }
    }

    private void Start()
    {
        APIManager.instance.GetVideoMaster(res =>
        {
            videoMaster = JsonUtility.FromJson<VideoMaster>(res);

            if (videoMaster.success && 
                !videoMaster.data.is_watched && 
                !string.IsNullOrEmpty(videoMaster.data.video))
            {
                HTMLVideoTrigger.instance.PlayVideoExternal(videoMaster.data.video);
                SoundManager.instance.MuteAll();
            }
        });

        StartCoroutine(BoothDataHolder.instance.GetBoothsState(lastBoothId =>
        {
            ProgressChecker();
            StartCoroutine(LastVisitedBoothChecker(lastBoothId));
        }));
    }

    IEnumerator LastVisitedBoothChecker(int boothId)
    {
        yield return new WaitUntil(() => TutorialMasterHandler.instance.tutorialMaster.is_watch && 
                                         DataHolder.instance.GetCurrentCharacter() != CharID.Unknown &&
                                         teleportNotif.activeSelf);

        int hallId = boothStates.Find(booth => booth.boothData.boothID == boothId).boothData.hallID;
        HallState hallState = hallStates.Find(hall => hall.hallData.hallID == hallId);
        List<BoothState> boothTemps = hallState.hallData.booths;

        for (int i = 0; i < boothTemps.Count; i++)
        {
            if (boothTemps[i].boothData.boothID == boothId)
            {
                if (i == boothTemps.Count - 1)
                {
                    teleportPanel.SetActive(true);
                }
                else 
                {
                    if (!boothTemps[i + 1].boothIsActive)
                    {
                        teleportPanel.SetActive(true);
                    }
                }
            }
        }
    }

    void ProgressChecker()
    {
        APIManager.instance.GetProgressHall(res =>
        {
            progressHall = JsonUtility.FromJson<ProgressHall>(res);
            foreach (ProgressHallDetail progressHall in progressHall.data)
            {
                List<QuestChecker> questCheckers = questCheckerForHall.FindAll(quest => quest.hallState.hallData.hallID == progressHall.master_value.id);
                if (questCheckers == null) continue;

                if (progressHall.is_finish)
                {
                    questCheckers.ForEach(quest => quest.SetCheckIconState(true));
                }
                else
                {
                    bool isDone = true;
                    List<BoothState> boothStates = new();
                    questCheckers.ForEach(quest => boothStates.AddRange(quest.hallState.hallData.booths));

                    foreach (BoothState boothState in boothStates)
                    {
                        if (!boothState.boothData.isDone)
                        {
                            isDone = false;
                            break;
                        }
                    }

                    if (isDone)
                    {
                        questCheckers.ForEach(quest => quest.SetCheckIconState(true));
                    }
                }
            }

            bool isOver = true;
            foreach (QuestChecker questChecker in questCheckerForHall)
            {
                if (!questChecker.GetCheckIconState())
                {
                    isOver = false;
                    break;
                }
            }

            questCheckerForGameOver.SetCheckIconState(isOver);
        });
    }

    public void SetupHall(int hallID)
    {
        foreach (HallState hall in hallStates)
        {
            hall.hallData.HallObj.SetActive(false);
            hall.hallData.HallIndicatorPanel.SetActive(false);
            if (hall.hallData.scrollView != null)
                hall.hallData.scrollView.SetActive(false);
        }

        currentHallState = hallStates.Find(res => res.hallData.hallID == hallID);
        currentHallState.hallData.HallObj.SetActive(true);
        currentHallState.hallData.HallIndicatorPanel.SetActive(true);
        currentHallState.hallData.scrollView.SetActive(true);

        DataHolder.instance.SetCurrentHall(currentHallState.hallData.hallID);
        GameManager.instance.simple2DMovement.transform.position = currentHallState.hallData.spawnPoint.position;
        if (!GetHallDetail(currentHallState.hallData.hallID).is_watched_intro) StartCoroutine(SetupVideoHall());
    }

    IEnumerator SetupVideoHall()
    {   
        yield return new WaitUntil(() => TutorialMasterHandler.instance.tutorialMaster.is_watch &&
                                         DataHolder.instance.temporaryData.character != CharID.Unknown &&
                                         DataHolder.instance.videoMasterPosted);

        StartCoroutine(SetupVideoPanel());
    }

    public void SetupBooth(int boothID)
    {
        for (int i = 0; i < hallStates.Count; i++)
        {
            for (int j = 0; j < hallStates[i].hallData.booths.Count; j++)
            {
                BoothData boothData = hallStates[i].hallData.booths[j].boothData;
                if (boothData.boothID == boothID && !DataHolder.instance.isFirstOpen)
                {
                    GameManager.instance.simple2DMovement.transform.position = boothData.playerInitPos.position;
                }
            }
        }
    }

    public void SetupHallBoothIds()
    {
        GameSessionDetail session = DataHolder.instance.currGameSessionDetail;
        for (int i = 0; i < session.hallDetails.Count; i++)
        {
            hallStates[i].hallIsActive = true;
            hallStates[i].hallData.hallID = session.hallDetails[i].hallId;
            for (int j = 0; j < session.hallDetails[i].boothDetails.Count; j++)
            {
                hallStates[i].hallData.booths[j].boothIsActive = true;
                hallStates[i].hallData.booths[j].boothData.hallID = hallStates[i].hallData.hallID;
                hallStates[i].hallData.booths[j].boothData.boothID = session.hallDetails[i].boothDetails[j].boothId;
            }
        }

        isDoneSetup = true;
    }

    public void SetupVideoPlayer()
    {
        StartCoroutine(SetupVideoPanel());
    }

    MasterValueDetail GetHallDetail(int hallId)
    {
        return DataHolder.instance.hallDetails.data.master_values.Find(hall => hall.id == hallId);
    }

    public IEnumerator SetupVideoPanel()
    {
        GameManager.instance.simple2DMovement.isWork = false;
        HTMLVideoTrigger.instance.PlayVideoExternal(GetHallDetail(currentHallState.hallData.hallID).intros[videoIndex].media);
        SoundManager.instance.MuteAll();

        yield return new WaitForSeconds(.5f);
        afterVideoFinishedPanel.SetActive(true);
    }

    public void NextVideoPlayer()
    {
        if (CanNextVideo())
        {
            videoIndex++;
            SetupVideoPlayer();
        }
        else
        {
            CloseVideoPanel();
        }
    }

    public bool CanNextVideo()
    {
        if (videoIndex < GetHallDetail(currentHallState.hallData.hallID).intros.Count - 1) return true;
        else return false;
    }

    public void CloseVideoPanel()
    {
        videoIndex = 0;
        afterVideoFinishedPanel.SetActive(false);
        GameManager.instance.inGamePanel.SetActive(true);
        GameManager.instance.simple2DMovement.isWork = true;

        if (DataHolder.instance.playerData.audioState == 1)
            SoundManager.instance.UnMuteAll();

        APIManager.instance.PostWatchVideoHall(res => {
            GetHallDetail(currentHallState.hallData.hallID).is_watched_intro = true;
        });
    }

    public void SetNotifNextHall()
    {
        StartCoroutine(NotifNextHallChecker());
    }

    IEnumerator NotifNextHallChecker()
    {
        yield return new WaitForSeconds(1.5f);
        for (int i = 0; i < teleportStates.Count; i++)
        {
            if (teleportStates[i].teleportData.canTeleport)
            {
                bool isDone = true;
                HallState hallState = teleportStates[i].teleportData.targetHallState;
                foreach (BoothState boothState in hallState.hallData.booths)
                {
                    foreach (Game boothGame in boothState.gameDetails)
                    {
                        if (!boothGame.is_finish)
                        {
                            isDone = false;
                            break;
                        }
                    }
                }

                teleportStates[i].teleportData.notifOnTeleportBtn.ForEach(obj =>
                {
                    obj.SetActive(!isDone);
                });
            }
            
            //if (!teleportStates[i].teleportData.canTeleport &&
            //    teleportStates[i - 1].teleportData.canTeleport &&
            //    i > 0)
            //{
            //    teleportStates[i - 1].teleportData.notifOnTeleportBtn.ForEach(obj =>
            //    {
            //        obj.SetActive(true);
            //    });
            //}
            //else if (i == teleportStates.Count - 1)
            //{
            //    bool isDone = true;
            //    HallState hallState = teleportStates[i].teleportData.targetHallState;
            //    foreach (BoothState boothState in hallState.hallData.booths)
            //    {
            //        if (!boothState.boothData.isDone)
            //        {
            //            isDone = false;
            //            break;
            //        }
            //    }

            //    teleportStates[i].teleportData.notifOnTeleportBtn.ForEach(obj =>
            //    {
            //        obj.SetActive(!isDone);
            //    });
            //}
        }

        List<GameObject> notifs = new();
        teleportStates.ForEach(state =>
        {
            state.teleportData.notifOnTeleportBtn.ForEach(notif =>
            {
                notifs.Add(notif);
            });
        });

        List<GameObject> notifs2 = notifs.FindAll(res => res.activeSelf);
        if (notifs2.Count > 0)
        {
            teleportNotif.SetActive(true);
        }
    }
}
