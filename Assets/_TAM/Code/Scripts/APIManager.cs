using System;
using System.Collections;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class APIManager : MonoBehaviour
{
    public static APIManager instance;

    [Header("Basic URL")]
    [SerializeField] string rootUrl = "https://tamconnect.com/api";
    [SerializeField] string getMasterValue = "roleplay/master-value";
    [SerializeField] string getMasterValueIntro = "roleplay/get-master-value-intro";
    [SerializeField] string getSubMasterValue = "roleplay/sub-master-value";
    [SerializeField] string getBooth = "roleplay/booths";
    [SerializeField] string getQuestionByBooth = "roleplay/get-questions";
    [SerializeField] string storeTutorial = "roleplay/store-tutorial";
    [SerializeField] string changeLanguage = "roleplay/change-language";
    [SerializeField] string submitAnswer = "roleplay/submit-answer";

    private void Awake()
    {
        instance = this;
    }

    public string SetupMasterValue() => string.Format("{0}/{1}", rootUrl, getMasterValue);
    public string SetupMasterValueIntro() => string.Format("{0}/{1}", rootUrl, getMasterValueIntro);
    public string SetupSubMasterValue() => string.Format("{0}/{1}", rootUrl, getSubMasterValue);
    public string SetupBooth() => string.Format("{0}/{1}", rootUrl, getBooth);
    public string SetupQuestionByBooth() => string.Format("{0}/{1}", rootUrl, getQuestionByBooth);
    public string SetupStoreTutorial() => string.Format("{0}/{1}", rootUrl, storeTutorial);
    public string SetupChangeLanguage() => string.Format("{0}/{1}", rootUrl, changeLanguage);
    public string SetupSubmitAnswer() => string.Format("{0}/{1}", rootUrl, submitAnswer);

    public IEnumerator PostDataCoroutine(string url, string jsonData, Action<string> SetDataEvent = null)
    {
        yield return new WaitUntil(() => !string.IsNullOrEmpty(DataHandler.instance.GetUserTicket()));
        
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
            Debug.LogError($"{url} || Error: " + request.error);
        else
            SetDataEvent?.Invoke(request.downloadHandler.text);
    }

    public IEnumerator PostDataWithTokenCoroutine(string url, string jsonData, Action<string> SetDataEvent = null)
    {
        yield return new WaitUntil(() => !string.IsNullOrEmpty(DataHandler.instance.GetUserTicket()));

        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        var request = new UnityWebRequest(url, "POST");

        // Set the authorization header
        string authHeaderValue = $"Basic dGRuYXhtb2xjYQ==";
        request.SetRequestHeader("Authorization", authHeaderValue);

        // Set the content type header
        request.SetRequestHeader("Content-Type", "application/json");

        // Set the request method and upload/download handlers
        request.method = UnityWebRequest.kHttpVerbPOST;
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        // Send the request asynchronously
        yield return request.SendWebRequest();

        // Check for errors
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
            // Handle error, for example, activate an error panel
            // errorPanel.SetActive(true);
        }
        else
        {
            // Invoke the callback with the response data
            SetDataEvent?.Invoke(request.downloadHandler.text);
        }
    }

    public IEnumerator PatchDataCoroutine(string url, string jsonData, Action<string> SetDataEvent = null)
    {
        yield return new WaitUntil(() => !string.IsNullOrEmpty(DataHandler.instance.GetUserTicket()));

        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        var request = new UnityWebRequest(url, "PATCH");

        request.SetRequestHeader("Content-Type", "application/json");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
            Debug.LogError("Error: " + request.error);
        else
            SetDataEvent(request.downloadHandler.text);
    }

    public IEnumerator GetDataCoroutine(string url, Action<string> SetDataEvent = null)
    {
        yield return new WaitUntil(() => !string.IsNullOrEmpty(DataHandler.instance.GetUserTicket()));

        using UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
            Debug.LogError("Error: " + request.error);
        else
            SetDataEvent(request.downloadHandler.text);
    }

    public IEnumerator DeleteDataCoroutine(string url)
    {
        yield return new WaitUntil(() => !string.IsNullOrEmpty(DataHandler.instance.GetUserTicket()));

        //deletePanel.SetActive(true);
        Time.timeScale = 0f;

        using (UnityWebRequest request = UnityWebRequest.Delete(url))
        {
            yield return request.SendWebRequest();
            Time.timeScale = 1f;

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to delete data: " + request.error);
            }
            else
            {
                Debug.Log("Data deleted successfully.");
                string currentUrl = Application.absoluteURL.Replace("delete", "");
                Debug.Log($"{currentUrl}");
                Application.ExternalEval("window.open('" + currentUrl + "', '_self')");
            }
        }
    }

    public IEnumerator DownloadImageCoroutine(string imageUrl, Action<Sprite> SetDataEvent = null)
    {
        yield return new WaitUntil(() => !string.IsNullOrEmpty(DataHandler.instance.GetUserTicket()));
        using UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error downloading image: " + request.error);
        }
        else
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            Sprite sprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f)
            );

            SetDataEvent(sprite);
        }
    }
}