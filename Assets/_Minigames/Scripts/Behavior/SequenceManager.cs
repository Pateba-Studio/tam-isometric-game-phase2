using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class SequenceManager : MonoBehaviour
{
    public string _server;
    [SerializeField] private TextAsset _questionJSON;
    [SerializeField] private List<string> _gameScenes;

    [SerializeField] private bool _autoExitOnComplete = true;
    [SerializeField] private string _homeScene = "Halls";

    private Dictionary<string, string> _sceneMap;

    [SerializeField] private bool _debug;

    private bool isEntry = true;
    private bool firstFrameUpdate = true;

    [HideInInspector] public string _masterValueById;
    [HideInInspector] public string _subMasterValueById;
    [HideInInspector] public string _ticket;
    private bool isAlternate = false;
    private Structures.Question _questions;
    [HideInInspector] public int _currentQuestionId;
    //private System.Random _random;

    [HideInInspector] public string ScoreType = "";
    [HideInInspector] public int CurrentSceneId = 0;

    private static SequenceManager instance;

    [HideInInspector] public bool ShowConfirmNext = false;
    [HideInInspector] public bool IsSequence = false;

    public List<int> _hallBList;
    [HideInInspector] public int HallBId = 0;

    public bool IsMediaIntro = false;
    public Structures.MediaSource MediaSrc;
    public Structures.MediaSource MediaCaseSrc;

    public bool CaseVideoReady = false;

    public string CorrectLetter;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void Start() 
    {
        foreach (BoothIdDetail booth in DataHolder.instance.currGameSessionDetail.hallDetails[2].boothDetails)
        {
            _hallBList.Add(booth.boothId);
        }

        _sceneMap = new()
        {
            { "Duck Rifle", "game01-rifle" },
            { "Whack A Mole", "game02-whackamole" },
            { "Ball Kick", "game03-football" },
            { "Ball Kick 2", "game03-football-alt" },
            { "Fishing Boat", "game04-fishingboat" },
            { "Fishing Boat 2", "game04-fishingboat-alt" },
            { "Runnerman", "game05-runnerman" },
            { "Runnerman 2", "game05-runnerman-alt" },
            { "Runerman", "game05-runnerman" },
            { "Runerman 2", "game05-runnerman-alt" },
            { "Claw Machine", "game06-clawmachine" },
            { "Target Practice", "game07-targetpractice" }
        };

        if (DataHolder.instance != null)
        {
            _ticket = DataHolder.instance.playerData.ticket;
            _masterValueById = DataHolder.instance.playerData.masterValueId;
            _subMasterValueById = DataHolder.instance.playerData.subMasterValueId;
        }

        /**
        if (_debug)
        {
            _ticket = "5277182D";
            _subMasterValueById = "14";
        }
        */

        // LoadQuestion();
    }

    public static SequenceManager Instance
    {
        get { return instance; }
    }

    private void Update()
    {
        if (isEntry)
        {
            isEntry = false;
            LoadQuestion();
        }
    }

    private void LoadQuestion()
    {
        if (_ticket != null)
        {
            StartCoroutine(FetchQuestion());
        }
    }

    private IEnumerator FetchQuestion()
    {
        WWWForm form = new();
        form.AddField("ticket", _ticket);
        form.AddField("sub_master_value_id", _subMasterValueById);

        Debug.Log("Connecting to " + _server + "/api/game-question");

        using (UnityWebRequest webRequest = UnityWebRequest.Post(_server + "/api/game-question", form))
        {
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:

                    _questions = JsonUtility.FromJson<Structures.Question>(webRequest.downloadHandler.text);

                    if (_questions.data.Length > 0)
                    {
                        ScoreType = _questions.data[0].type;

                        IsSequence = false;
                        for (int i = 0; i < _hallBList.Count; i++)
                        {
                            if (_hallBList[i].ToString() == _subMasterValueById)
                            {
                                IsSequence = true;
                                IsMediaIntro = true;
                                StartCoroutine(GetIntroValue());
                                StartCoroutine(GetCaseIntro());
                                break;
                            }
                        }
                    }
                    else
                    {
                        int pointing = -1;
                        for (int i = 0; i < _hallBList.Count; i++)
                        {
                            if (_hallBList[i].ToString() == _subMasterValueById)
                            {
                                pointing = i + 1;
                                break;
                            }
                        }

                        if (pointing == -1)
                        {
                            ExitMinigame();
                        }
                        else
                        {
                            if (pointing < _hallBList.Count)
                            {
                                _subMasterValueById = _hallBList[pointing].ToString();
                                HallBId++;
                                RestartFetchQuestion();
                            }
                            else
                            {
                                ExitMinigame();
                            }
                        }
                    }

                    if (!IsMediaIntro) Launch();

                    break;
            }
            webRequest.Dispose();
        }
    }

    private IEnumerator GetIntroValue()
    {
        WWWForm form = new();
        form.AddField("ticket", _ticket);
        form.AddField("master_value_id", _masterValueById);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(_server + "/api/master-value/intro", form))
        {
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    MediaSrc = JsonUtility.FromJson<Structures.MediaSource>(webRequest.downloadHandler.text);
                    Launch();
                    break;
            }

            webRequest.Dispose();
        }
    }

    private IEnumerator GetCaseIntro()
    {
        WWWForm form = new();
        form.AddField("ticket", _ticket);
        form.AddField("sub_master_value_id", _subMasterValueById);

        Debug.Log(_ticket + " " + _subMasterValueById);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(_server + "/api/sub-master-value/intro", form))
        {
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(webRequest.downloadHandler.text);
                    MediaCaseSrc = JsonUtility.FromJson<Structures.MediaSource>(webRequest.downloadHandler.text);
                    // Launch();
                    break;
            }

            CaseVideoReady = true;

            webRequest.Dispose();
        }
    }

    private void Launch()
    {
        if (_questions.data.Length > 0)
        {
            if (_sceneMap.ContainsKey(_questions.data[0].game_type))
            {
                SceneManager.LoadScene(_sceneMap[_questions.data[0].game_type], LoadSceneMode.Single);
            }
            else
            {
                SceneManager.LoadScene(_sceneMap["Duck Rifle"], LoadSceneMode.Single);
            }
        }
    }

    private void RestartFetchQuestion()
    {
        StartCoroutine(FetchQuestion());
    }

    private IEnumerator SubmitAnswer(int questionNumber, int answerId)
    {
        WWWForm form = new();
        form.AddField("ticket", _ticket);
        form.AddField("game_question_id", _questions.data[questionNumber].id);
        form.AddField("answer_id", answerId);

        Debug.Log(questionNumber + " " + answerId);
        Debug.Log("Connecting to " + _server + "/api/game-question/push-answer");

        using (UnityWebRequest webRequest = UnityWebRequest.Post(_server + "/api/game-question/push-answer", form))
        {
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    //Debug.Log(webRequest.downloadHandler.text);
                    Debug.Log("Answer Submitted");
                    break;
            }

            webRequest.Dispose();
        }
    }

    private IEnumerator FlagBoothComplete()
    {
        WWWForm form = new();
        form.AddField("ticket", _ticket);
        form.AddField("sub_master_value_id", _subMasterValueById);

        Debug.Log("Connecting to " + _server + "/api/game-question/push-checkpoint");

        using (UnityWebRequest webRequest = UnityWebRequest.Post(_server + "/api/game-question/push-checkpoint", form))
        {
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    if (IsSequence)
                    {
                        //Invoke("InvokeNextStage", 3f);
                        Invoke("InvokeShowConfirmNext", 5f);
                    }
                    else
                    {
                        if (_autoExitOnComplete) Invoke("ExitMinigame", 3f);
                    }
                    Debug.Log("Booth flag complete");
                    break;
            }

            webRequest.Dispose();
        }
    }

    public Structures.QuestionData GetQuestion()
    {
        return _questions.data[_currentQuestionId];
    }

    public string GetCorrectAnswer()
    {
        return _questions.data[_currentQuestionId].answer_correct.answer;
    }

    public void NextQuestion(int answerId, float delay)
    {
        //  Submit Answer
        if (!_debug)
            StartCoroutine(SubmitAnswer(_currentQuestionId, answerId));

        _currentQuestionId++;
        if (_currentQuestionId < _questions.data.Length)
        {
            Invoke("InvokeNextStage", 3.5f + delay);
        }
        else
        {
            StartCoroutine(FlagBoothComplete());
        }
    }

    private void InvokeShowConfirmNext()
    {
        ShowConfirmNext = true;
    }

    private void InvokeNextStage()
    {
        NextScene();
    }

    private void NextScene()
    {
        if (_sceneMap.ContainsKey(_questions.data[_currentQuestionId].game_type))
        {
            SceneManager.LoadScene(_sceneMap[_questions.data[_currentQuestionId].game_type], LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene(_sceneMap["Duck Rifle"], LoadSceneMode.Single);
        }
    }

    public void ExitMinigame()
    {
        if (_homeScene != null)
        {
            if (_homeScene != string.Empty)
            {
                SceneManager.LoadScene(_homeScene, LoadSceneMode.Single);
            }
            else
            {
                SceneManager.LoadScene(0, LoadSceneMode.Single);
            }
        }
        else
        {
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }
        Destroy(this.gameObject);
    }

    public void InterfaceContinue(bool isContinue)
    {
        if (isContinue)
        {
            Debug.Log("Continue");
            // NextScene();
            isEntry = true;
            HallBId++;
            _currentQuestionId = 0;
            _subMasterValueById = _hallBList[HallBId].ToString();
            SceneManager.LoadScene("EntryPoint", LoadSceneMode.Single);
        }
        else
        {
            Debug.Log("Exit");
            ExitMinigame();
        }
    }

    public int GetQuestionCount()
    {
        if (_questions != null)
        {
            return _questions.data.Length;
        }

        return 0;
    }
}
