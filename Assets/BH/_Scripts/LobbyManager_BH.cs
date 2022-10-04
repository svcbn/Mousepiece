using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System;

public class LobbyManager_BH : MonoBehaviourPunCallbacks
{
    public Transform trListContent;

    public Text roomTimerTxt;
    public Text roomModeTxt;
    public Text roomThemeTxt;
    public Button joinBtn;

    public Text CreateRoomModeTxt;

    public Button multiPlayBtn;
    public GameObject multiPlayPanel;
    public Button competeModeBtn;
    public Button collaborateModeBtn;
    public Button soloPlayBtn;

    public Text nickNameTxt;
    public InputField nickNameIF;
    public Button nickNameBtn;

    public Button btnCreate;

    enum gameType
    {
        Compete,
        Collaborate,
        Single
    }

    [SerializeField]
    gameType type;

    Dictionary<string, RoomInfo> roomCache = new Dictionary<string, RoomInfo>();


    [SerializeField]
    string selectedRoomName = null;

    // Start is called before the first frame update
    void Start()
    {
        nickNameIF.onValueChanged.AddListener(OnNicknameValueChanged);
        nickNameIF.onSubmit.AddListener(OnNicknameSubmit);
        nickNameTxt.text = "";

        roomTimerTxt.text = "";
        roomModeTxt.text = "";
        roomThemeTxt.text = "";
        multiPlayPanel.SetActive(false);
        CreateRoomModeTxt.text = "";
        btnCreate.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire1") && multiPlayPanel.activeSelf)
        {
            StartCoroutine(AnimatedBtnClose());
        }
    }

    public void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        string roomName;

        switch (type)
        {
            case gameType.Compete:
                roomOptions.MaxPlayers = 8;
                roomOptions.IsVisible = true;

                roomName = "Compete" + UnityEngine.Random.Range(0, 1000);
                PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
                break;

            case gameType.Collaborate:
                roomOptions.MaxPlayers = 4;
                roomOptions.IsVisible = true;

                roomName = "Collaborate" + UnityEngine.Random.Range(0, 1000);
                PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
                break;

            case gameType.Single:
                roomOptions.MaxPlayers = 1;
                roomOptions.IsVisible = true;

                roomName = "Single" + UnityEngine.Random.Range(0, 1000);
                PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
                break;
        }
        
    }

   



    public void onCreateRoomClicked()
    {
        CreateRoom();
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
        
        PhotonNetwork.JoinRoom(roomname);
        
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        print("방입장성공");

        switch(type)
        {
            case gameType.Compete:
                PhotonNetwork.LoadLevel("03RoomScene_BH");
                break;
            case gameType.Collaborate:
                PhotonNetwork.LoadLevel("04CollaborateScene");
                break;
            case gameType.Single:
                PhotonNetwork.LoadLevel("05SingleRoom_BH");
                break;
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        print("방입장실패, " + returnCode + ", " + message);
    }

    public void OnMultiPlayBtnClicked()
    {
        StartCoroutine(AnimatedBtnPopUp());
    }

    public void OnCompeteBtnClicked()
    {
        type = gameType.Compete;
        CreateRoomModeTxt.text = "경쟁 모드";
        StartCoroutine(AnimatedBtnClose());
    }

    public void OnCollaborateBtnClicked()
    {
        type = gameType.Collaborate;
        CreateRoomModeTxt.text = "협동 모드";
        StartCoroutine(AnimatedBtnClose());
    }

    public void OnSoloPlayBtnClicked()
    {
        type = gameType.Single;
        CreateRoomModeTxt.text = "혼자 그리기";
    }

    #region 버튼애니메이션
    IEnumerator AnimatedBtnPopUp()
    {
        multiPlayBtn.interactable = false;
        multiPlayPanel.SetActive(true);

        multiPlayPanel.transform.localScale = new Vector3(0.55f, 0.3f, 0.37f) * 0.01f;

        while(multiPlayPanel.transform.localScale.x < 0.55f)
        {
            multiPlayPanel.transform.localScale += new Vector3(0.55f, 0.3f, 0.37f) * 0.05f;
            yield return null;
        }

        multiPlayPanel.transform.localScale = new Vector3(0.55f, 0.3f, 0.37f);
    }

    IEnumerator AnimatedBtnClose()
    {
        while (multiPlayPanel.transform.localScale.x > 0.01f)
        {
            multiPlayPanel.transform.localScale -= new Vector3(0.55f, 0.3f, 0.37f) * 0.05f;
            yield return null;
        }

        multiPlayBtn.interactable = true;
        multiPlayPanel.SetActive(false);
    }
    #endregion

    #region NickName설정부
    void OnNicknameValueChanged(string s)
    {
        DecideNickname();
        
    }

    void OnNicknameSubmit(string s)
    {
        DecideNickname();

    }

    public void OnNickNameBtnClicked()
    {
        DecideNickname();
    }

    void DecideNickname()
    {
        if(nickNameIF.text.Length > 10)
        {
            nickNameIF.text = nickNameIF.text.Remove(10);
        }

        nickNameTxt.text = nickNameIF.text;
        print("닉네임 설정 : " + nickNameTxt.text);
        PhotonNetwork.NickName = nickNameTxt.text;

        btnCreate.interactable = nickNameTxt.text.Length > 1;
        joinBtn.interactable = nickNameTxt.text.Length > 1;
    }
    #endregion

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        DeleteRoomListUI();
        UpdateRoomCache(roomList);
        CreateRoomListUI();
    }

    void DeleteRoomListUI()
    {
        foreach(Transform tr in trListContent)
        {
            Destroy(tr.gameObject);
        }
    }

    void UpdateRoomCache(List<RoomInfo> roomList)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomCache.ContainsKey(roomList[i].Name))
            {
                if (roomList[i].RemovedFromList)
                {
                    roomCache.Remove(roomList[i].Name);
                }
                else
                {
                    roomCache[roomList[i].Name] = roomList[i];
                }
            }
            else
            {
                roomCache[roomList[i].Name] = roomList[i];
            }
        }
    }

    public GameObject roomItemFac;
    void CreateRoomListUI()
    {
        foreach(RoomInfo info in roomCache.Values)
        {
            GameObject go = Instantiate(roomItemFac, trListContent);

            RoomItem item = go.GetComponent<RoomItem>();
            item.SetInfo(info);

            string timer = (string)info.CustomProperties["timer"];
            string theme = (string)info.CustomProperties["theme"];
            string gamemode = (string)info.CustomProperties["gamemode"];
            roomname = (string)info.CustomProperties["roomName"];
            
            item.OnClickAction = SetRoom;

            print(roomname);
        }
    }

    string roomname = "";
    void SetRoom(string timer, string gamemode, string theme, string roomName)
    {
        roomTimerTxt.text = timer;
        roomModeTxt.text = gamemode;
        type = gameType.Collaborate;
        roomThemeTxt.text = theme;
        
    }
}
