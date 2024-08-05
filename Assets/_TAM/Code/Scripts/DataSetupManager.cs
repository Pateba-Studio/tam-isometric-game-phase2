using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#region Booth
[Serializable]
public class BoothIdData
{
    public int id;
    public string master_value;
    public string name;
}

[Serializable]
public class BoothId
{
    public bool success;
    public List<BoothIdData> data;
}
#endregion
#region Hall
[Serializable]
public class HallVideoDetail
{
    public string media;
    public string audio;
}

[Serializable]
public class MasterValueDetail
{
    public int id;
    public string name;
    public List<HallVideoDetail> intros;
    public bool is_watched_intro;
}

[Serializable]
public class HallIdData
{
    public List<MasterValueDetail> master_values;
    public string master_video;
}

[Serializable]
public class HallId
{
    public bool success;
    public HallIdData data;
}
#endregion

public class DataSetupManager : MonoBehaviour
{
    public string gameScene;

    [Header("Hall Booth Ids")]
    public HallId hallIds;
    public BoothId boothIds;

    [Header("Default Attribute For Editor Only")]
    public float setupDelay;
    public PlayerData playerData;

    void Start()
    {
#if UNITY_EDITOR
        StartCoroutine(PlayerDataDefault());
#endif
        StartCoroutine(PlayerDataChecker());
    }

    IEnumerator PlayerDataDefault()
    {
        yield return new WaitForSeconds(setupDelay);
        DataHolder.instance.playerData = playerData;
    }

    IEnumerator PlayerDataChecker()
    {
        yield return new WaitUntil(() => !string.IsNullOrEmpty(DataHolder.instance.playerData.gameSession.id));
        APIManager.instance.GetHallID(res => { DataHolder.instance.hallDetails = hallIds = JsonUtility.FromJson<HallId>(res); });
        APIManager.instance.GetBoothID(res => { boothIds = JsonUtility.FromJson<BoothId>(res); });
        Debug.Log("Getting Hall & Booth Data");

        yield return new WaitUntil(() => hallIds.success && boothIds.success);
        DataHolder.instance.SetGameSession(hallIds.data.master_values, boothIds.data);
        Debug.Log("Setting Game Session");

        yield return new WaitUntil(() => !string.IsNullOrEmpty(DataHolder.instance.currGameSessionDetail.gameSession.id));
        SceneManager.LoadScene(gameScene);
    }
}
