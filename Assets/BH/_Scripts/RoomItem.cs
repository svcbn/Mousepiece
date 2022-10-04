using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;
using System;

public class RoomItem : MonoBehaviourPunCallbacks
{
    public Text roomPlayer;
    public Text roomTimer;
    public Text roomMode;
    public Text roomTheme;

    public Action<string, string, string, string> OnClickAction;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    string roomName = "";

    public void OnClick()
    {
        if (OnClickAction != null)
        {
            OnClickAction(roomTimer.text, roomMode.text, roomTheme.text, roomName);
        }
    }
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);

    }

    public void SetInfo(string Timer, string GameMode, string theme, int currPlayers, byte maxPlayer, string roomname)
    {
        name = roomname;
        roomPlayer.text = "(" + currPlayers + " / " + maxPlayer + ")";
    }

    public void SetInfo(RoomInfo info)
    {
        
        roomTheme.text = (string)info.CustomProperties["theme"];
        roomTimer.text = (string)info.CustomProperties["timer"];
        roomMode.text = (string)info.CustomProperties["gamemode"];
        roomName = (string)info.CustomProperties["roomname"];

        SetInfo((string)info.CustomProperties["timer"], (string)info.CustomProperties["gamemode"], (string)info.CustomProperties["theme"], info.PlayerCount, info.MaxPlayers, (string)info.CustomProperties["roomname"]);
        
    }
}
