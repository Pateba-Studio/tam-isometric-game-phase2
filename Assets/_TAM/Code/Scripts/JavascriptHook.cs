using UnityEngine;
using UnityEngine.UI;

public class JavascriptHook : MonoBehaviour
{
#if UNITY_EDITOR
    private void Start()
    {
        PlayerNameHandler(DataHolder.instance.playerData.name);
        PlayerEmailHandler(DataHolder.instance.playerData.email);
        PlayerTicketHandler(DataHolder.instance.playerData.ticket);
        PlayerLanguageHandler(DataHolder.instance.playerData.language);
        PlayerAudioHandler(DataHolder.instance.playerData.audioState);
        PlayerMasterValueHandler(DataHolder.instance.playerData.masterValueId);
        PlayerSubMasterValueHandler(DataHolder.instance.playerData.subMasterValueId);
        PlayerGameSessionHandler(JsonUtility.ToJson(DataHolder.instance.playerData.gameSession));
    }
#endif
    
    public void PlayerNameHandler(string name)
    {
        DataHolder.instance.playerData.name = name;
        //Debug.Log("[Game] Name: " + name);
    }

    public void PlayerEmailHandler(string email)
    {
        DataHolder.instance.playerData.email = email;
        if (email == "admin") DebugHandler.instance.debugPanel.SetActive(true);
        //Debug.Log("[Game] Email: " + email);
    }

    public void PlayerTicketHandler(string ticket)
    {
        DataHolder.instance.playerData.ticket = ticket;
        //Debug.Log("[Game] Ticket: " + ticket);
    }

    public void PlayerLanguageHandler(string lang)
    {
        DataHolder.instance.playerData.language = lang;
        //Debug.Log("[Game] Lang: " + lang);
    }

    public void PlayerAudioHandler(int audio)
    {
        DataHolder.instance.playerData.audioState = audio;
        //Debug.Log("[Game] Audio: " + audio);
    }

    public void PlayerMasterValueHandler(string master)
    {
        DataHolder.instance.playerData.masterValueId = master;
        //Debug.Log("[Game] Hall: " + master);
    }

    public void PlayerSubMasterValueHandler(string sub)
    {
        DataHolder.instance.playerData.subMasterValueId = sub;
        //Debug.Log("[Game] Booth: " + sub);
    }

    public void PlayerGameSessionHandler(string json)
    {
        DataHolder.instance.playerData.gameSession = JsonUtility.FromJson<GameSession>(json);
        //Debug.Log("[Game] Session: " + json);
    }
}
