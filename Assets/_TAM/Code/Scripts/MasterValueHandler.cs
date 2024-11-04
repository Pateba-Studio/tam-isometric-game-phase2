using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

#region Booth
[Serializable]
public class BoothData
{
    public int id;
    public string name;
    public QuestionData question;
}

[Serializable]
public class Booth
{
    public bool success;
    public string message;
    public List<BoothData> booths;
}
#endregion

#region Question
[Serializable]
public class Answer
{
    public int id;
    public string answer;
    public string audio_path;
    public string response_dialogue;
    public int point;
}

[Serializable]
public class Question
{
    public int id;
    public string question;
    public string advice;
    public string background;
    public List<Answer> answers;
}

[Serializable]
public class RoleplayQuestion
{
    public int id;
    public string type;
    public int order;
    public string video_path;
    public string curr_booth_name;
    public Question question;
}

[Serializable]
public class QuestionData
{
    public bool success;
    public string message;
    public List<RoleplayQuestion> roleplay_questions;
}
#endregion

public class MasterValueHandler : MonoBehaviour
{
    public List<InteractableHandler> boothsSites;

    [Header("Data Handler")]
    public MasterValueData masterValueData;
    public Booth booth;

    public IEnumerator InitAllBoothValue(List<MasterValueData> data)
    {
        booth = new Booth();
        //GameManager.instance.SetLoadingText("Getting All Hall Data");
        var curr = data.Find(res => res.name.Contains(masterValueData.name));
        if (curr == null) yield break;
        masterValueData = curr;

        string json = $"{{\"ticket_number\":\"{DataHandler.instance.GetUserTicket()}\"," +
            $"\"master_value_id\":{masterValueData.id}}}";
        StartCoroutine(APIManager.instance.PostDataCoroutine(
            APIManager.instance.SetupBooth(), json,
            res =>
            {
                booth = JsonUtility.FromJson<Booth>(res);
            }));

        yield return new WaitUntil(() => booth.success);

        foreach (var item in booth.booths)
        {
            string boothData = $"{{\"ticket_number\":\"{DataHandler.instance.GetUserTicket()}\"," +
                $"\"booth_id\":{item.id}}}";
            StartCoroutine(APIManager.instance.PostDataCoroutine(
                APIManager.instance.SetupQuestionByBooth(),
                boothData, res => item.question = JsonUtility.FromJson<QuestionData>(res)));
        }

        yield return new WaitUntil(() => booth.booths.Find(res => !res.question.success) == null);

        GameManager.instance.SetMasterValueState(this);
        foreach (var item in booth.booths)
        {
            foreach (var boothTemp in boothsSites)
            {
                int id = boothTemp.hallBoothData.GetGameBooth().gameBoothId;
                if (item.id == id)
                {
                    if (item.question.roleplay_questions.Count > 0)
                    {
                        boothTemp.SetupBoothClear(false);
                    }
                    else
                    {
                        boothTemp.SetupBoothClear(true);
                    }
                }
            }
        }
    }

    public IEnumerator InitBoothValue(UnityEvent events)
    {
        string boothName = GameManager.instance.currentRoleplayQuestion.curr_booth_name;
        var item = booth.booths.Find(booth => booth.name == boothName);
        if (item == null) yield break;

        foreach (var boothTemp in boothsSites)
        {
            int id = boothTemp.hallBoothData.GetGameBooth().gameBoothId;
            if (item.id == id)
            {
                item.question.roleplay_questions.RemoveAt(0);
                if (item.question.roleplay_questions.Count > 0)
                {
                    boothTemp.SetupBoothClear(false);
                }
                else
                {
                    boothTemp.SetupBoothClear(true);
                }

                break;
            }
        }

        events?.Invoke();
    }
}
