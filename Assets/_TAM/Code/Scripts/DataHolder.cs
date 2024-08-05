using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[Serializable]
public class LastVisitedBooth
{
    public int data;
    public bool success;
    public string message;
}

[Serializable]
public class GameSessionDetail
{
    public GameSession gameSession;
    public List<HallIdDetail> hallDetails;
}

[Serializable]
public class HallIdDetail
{
    public int hallId;
    public string hallName;
    public bool is_watched_intro;
    public List<HallVideoDetail> hallIntros;
    public List<BoothIdDetail> boothDetails;
}

[Serializable]
public class BoothIdDetail
{
    public int boothId;
    public string boothName;
}

[Serializable]
public class GameSession
{
    public string id;
    public string type_elearning;
}

[Serializable]
public class PlayerData
{
    public string name;
    public string email;
    public string ticket;
    public string language;
    public int audioState;
    public string masterValueId;
    public string subMasterValueId;
    public GameSession gameSession;
}

[Serializable]
public class TemporaryData
{
    public CharID character;
    public PlatformType platform;
    public int hall;
    public string lastVisitedBooth;
}

public enum LangID
{
    EN, ID
}

public enum CharID
{
    Green, Black, Pink, Yellow, Unknown
}

public enum PlatformType
{
    Desktop, Mobile, Unknown
}

public class DataHolder : MonoBehaviour
{
    public static DataHolder instance;
    public bool isFirstOpen;
    public bool videoMasterPosted;

    [Header("Current Player Data")]
    public PlayerData playerData;
    public TemporaryData temporaryData;

    [Header("Another Datas")]
    public GameSessionDetail currGameSessionDetail;
    public LastVisitedBooth lastVisitedBooth;
    public HallId hallDetails;

    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void SetGameSession(List<MasterValueDetail> hallIds, List<BoothIdData> boothIds)
    {
        currGameSessionDetail.gameSession = playerData.gameSession;
        foreach (MasterValueDetail hall in hallIds)
        {
            List<BoothIdDetail> boothDetails = new();
            foreach (BoothIdData booth in boothIds.FindAll(booth => booth.master_value == hall.name))
            {
                BoothIdDetail boothDetail = new()
                {
                    boothId = booth.id,
                    boothName = booth.name
                };

                boothDetails.Add(boothDetail);
            }

            HallIdDetail hallDetail = new()
            {
                hallId = hall.id,
                hallName = hall.name,
                is_watched_intro = hall.is_watched_intro,
                hallIntros = hall.intros,
                boothDetails = boothDetails
            };

            currGameSessionDetail.hallDetails.Add(hallDetail);
        }
    }

    public HallIdDetail GetHallDetail(int hallId)
    {
        return currGameSessionDetail.hallDetails.Find(hall => hall.hallId == hallId);
    }

    public void SetCurrentCharacter(CharID id)
    {
        temporaryData.character = id;
    }

    public CharID GetCurrentCharacter()
    {
        return temporaryData.character;
    }

    public void SetCurrentHall(int id)
    {
        temporaryData.hall = id;
    }

    public int GetCurrentHall()
    {
        return temporaryData.hall;
    }

    public void SetCurrentPlatformType(PlatformType platform)
    {
        temporaryData.platform = platform;
    }

    public PlatformType GetCurrentPlatformType()
    {
        return temporaryData.platform;
    }

    public LangID GetCurrentLanguage()
    {
        if (playerData.language == "en") return LangID.EN;
        else return LangID.ID;
    }
}
