using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System;

public class LobbyManager_BH : MonoBehaviourPunCallbacks
{
    public Button btnJoin;
    public Button btnCreate;

    Dictionary<string, RoomInfo> roomCache = new Dictionary<string, RoomInfo>();

    public Transform trListContent;

    [SerializeField]
    string selectedRoomName = null;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            CreateRoom("test");

        }
    }
    
    public void CreateRoom(string roomName)
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 8;
        PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        print("방생성 완료");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        print("방생성 실패, " + returnCode + ", " + message);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(selectedRoomName);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        print("방입장성공");
        PhotonNetwork.LoadLevel("03RoomScene_BH");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        print("방입장실패, " + returnCode + ", " + message);
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);
    }
}
