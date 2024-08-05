using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class LanguageDetail
{
    public GameObject gameobjectEn;
    public GameObject gameobjectId;
}

public class LanguageManager : MonoBehaviour
{
    public static LanguageManager instance;
    [SerializeField] public List<LanguageDetail> languageDetails;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (!DataHolder.instance.isFirstOpen)
        {
            if (DataHolder.instance.playerData.language == "id") SetIdLanguage();
            else SetEnLanguage();
        }
    }

    public void SetIdLanguage()
    {
        DataHolder.instance.playerData.language = "id";
        APIManager.instance.PostChangeLanguage();

        foreach (LanguageDetail lang in languageDetails)
        {
            if (lang.gameobjectId != null &&
                lang.gameobjectEn != null)
            {
                lang.gameobjectId.SetActive(true);
                lang.gameobjectEn.SetActive(false);
            }
        }
    }

    public void SetEnLanguage()
    {
        DataHolder.instance.playerData.language = "en";
        APIManager.instance.PostChangeLanguage();

        foreach (LanguageDetail lang in languageDetails)
        {
            if (lang.gameobjectId != null &&
                lang.gameobjectEn != null)
            {
                lang.gameobjectId.SetActive(false);
                lang.gameobjectEn.SetActive(true);
            }
        }
    }
}
