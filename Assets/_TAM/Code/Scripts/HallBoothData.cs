using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "Hall and Booth Data", 
    menuName = "ScriptableObjects/Hall and Booth Data", 
    order = 1
    )]
public class HallBoothData : ScriptableObject
{
    public string NPCCharKey;
    public string titleId;
    public string titleEn;
    public string finalContentId;
    public string finalContentEn;
    [TextArea] public List<string> contentId;
    [TextArea] public List<string> contentEn;

    [Header("Content-Type")]
    public int masterValueId;
    public List<int> gameBoothIds;
}