using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameBoothId
{
    public GameType gameType;
    public List<int> gameBoothIds;
}

[CreateAssetMenu(
    fileName = "Hall and Booth Data", 
    menuName = "ScriptableObjects/Hall and Booth Data", 
    order = 1
    )]
public class HallBoothData : ScriptableObject
{
    [Header("Booth-Dialogue")]
    public string NPCCharKey;
    public string titleId;
    public string titleEn;
    public string finalContentId;
    public string finalContentEn;
    [TextArea] public List<string> contentId;
    [TextArea] public List<string> contentEn;

    [Header("Content-Type")]
    public int masterValueId;
    public List<GameBoothId> gameBooths;

    public GameBoothId GetGameBooth()
    {
        switch (DataHandler.instance.playerData.type_elearning)
        {
            case "default":
                return gameBooths.Find(res => res.gameType == GameType.Default);
            case "simple":
                return gameBooths.Find(res => res.gameType == GameType.Simple);
            default:
                return null;
        }
    }
}