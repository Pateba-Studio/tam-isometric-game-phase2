using Assets.SimpleLocalization.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PreloadManager : MonoBehaviour
{
    public static PreloadManager instance;

    [SerializeField] string defaultTicket;
    [SerializeField] string defaultLanguage;
    [SerializeField] string defaultSubMasterValue;

    private void Start()
    {
#if UNITY_EDITOR
        OnLoadPlayerData($"{{\"email\":\"youremail@gmail.com\"," +
            $"\"ticket\":\"{defaultTicket}\"," +
            $"\"sub_master_value_id\":\"{defaultSubMasterValue}\"," +
            $"\"language\":\"{defaultLanguage}\"}}");
#else
        DataHandler.instance.playerData.ticket = string.Empty;
        LocalStorageHandler.Load("playerData", gameObject.name, "OnLoadPlayerData");
#endif
    }

    void OnLoadPlayerData(string jsonData)
    {
        GameManager.instance.SetLoadingText("Getting Local Player Data");
        DataHandler.instance.playerData = JsonUtility.FromJson<PlayerData>(jsonData);
        SetInitLanguage();

        string json = $"{{\"ticket_number\":\"{DataHandler.instance.GetUserTicket()}\"}}";
        StartCoroutine(APIManager.instance.PostDataCoroutine(
            APIManager.instance.SetupMasterValue(),
            json, res =>
            {
                DataHandler.instance.masterValue = JsonUtility.FromJson<MasterValue>(res);

                foreach (var handler in GameManager.instance.masterValueHandlers)
                {
                    StartCoroutine(handler.masterValueHandler.InitAllBoothValue(
                        DataHandler.instance.masterValue.master_values
                        ));
                }
            }));
    }

    public void SetInitLanguage()
    {
        LocalizationManager.Read();

        switch (DataHandler.instance.playerData.language)
        {
            case "en":
                LocalizationManager.Language = "en-US";
                break;
            case "id":
                LocalizationManager.Language = "id-ID";
                break;
        }
    }
}
