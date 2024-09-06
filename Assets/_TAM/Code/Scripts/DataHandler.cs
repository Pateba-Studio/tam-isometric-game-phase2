using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#region PlayerData
[Serializable]
public class PlayerData
{
    public string email;
    public string ticket;
    public string sub_master_value_id;
    public string language;
}

[Serializable]
public class User
{
    public string ticket_number;
    public string language;
    public string type_elearning;
}
#endregion

#region MasterValue
[Serializable]
public class MasterValueData
{
    public string name;
    public int id;
}

[Serializable]
public class MasterValue
{
    public bool success;
    public List<MasterValueData> master_values;
}
#endregion

public class DataHandler : MonoBehaviour
{
    public static DataHandler instance;
    
    public PlayerData playerData;
    public MasterValue masterValue;

    [HideInInspector] public bool isPlaying;

    private void Awake()
    {
        instance = this;
    }

    public string GetUserTicket() => playerData.ticket;
}
