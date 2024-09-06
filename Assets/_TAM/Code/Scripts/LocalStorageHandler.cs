using UnityEngine;

public class LocalStorageHandler : MonoBehaviour
{
    public static void Save(string key, string jsonValue)
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            Application.ExternalCall("saveToLocalStorage", key, jsonValue);
        }
        else
        {
            PlayerPrefs.SetString(key, jsonValue);
        }
    }

    public static void Load(string key, string gameObjectName, string callbackMethod)
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            Application.ExternalCall("getFromLocalStorage", key, gameObjectName, callbackMethod);
        }
        else
        {
            string jsonValue = PlayerPrefs.GetString(key, "{}");
            GameObject.Find(gameObjectName).SendMessage(callbackMethod, jsonValue);
        }
    }

    public static void Remove(string key)
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            Application.ExternalCall("removeFromLocalStorage", key);
        }
        else
        {
            PlayerPrefs.DeleteKey(key);
        }
    }

    public static void Clear()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            Application.ExternalCall("clearLocalStorage");
        }
        else
        {
            PlayerPrefs.DeleteAll();
        }
    }
}
