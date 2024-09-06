using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue Data", menuName = "ScriptableObjects/DialogueData", order = 1)]
public class DialogueData : ScriptableObject
{
    public bool isReceptionist;
    public string titleId;
    public string titleEn;
    public string NPCCharKey;
    [TextArea] public List<string> contentId;
    [TextArea] public List<string> contentEn;
}