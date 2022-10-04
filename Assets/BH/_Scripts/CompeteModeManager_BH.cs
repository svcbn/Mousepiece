using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using Photon.Realtime;

public class CompeteModeManager_BH : MonoBehaviourPunCallbacks
{
    public RoomOptions options = new RoomOptions();

    public static CompeteModeManager_BH instance;

    public enum gameState
    {
        Ready,
        Start,
        Playing,
        Vote,
        Victory
    }

    public gameState state = gameState.Ready;
    string theme;

    public Canvas canvasR;
    public InputField IFtheme;
    public Text TextTheme;

    public Text isMasterText;

    public Button startBtn;
    public Button decisionBtn;

    public Canvas canvasL;
    public Canvas canvasF;
    public Canvas InGameCanvas;

    //public Canvas drawCanvas;

    public Transform[] canvasPos;
    public GameObject[] playerCanvas;
    public GameObject[] playerCanvas_parent;
    public Transform[] votePicPos;

    public GameObject voteWall;

    public List<PhotonView> playerPV = new List<PhotonView>();
    public GameObject[] playerList = new GameObject[8];

    Vector3[] spawnPos = new Vector3[8];

    int playeridx;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.SerializationRate = 90;
        PhotonNetwork.SendRate = 90;
        SetSpawnPos();

        playeridx = PhotonNetwork.CurrentRoom.PlayerCount - 1;
        PhotonNetwork.Instantiate("Player", spawnPos[playeridx] + Vector3.up * 0.5f, Quaternion.identity);
        

        IFtheme.onValueChanged.AddListener(OnThemeValueChanged);
        IFtheme.onSubmit.AddListener(OnThemeSubmit);
        IFtheme.onEndEdit.AddListener(OnThemeEndEdit);


        DelayText.enabled = false;
        DelayTimeText.enabled = false;
        inGameTheme.enabled = false;
        inGameTimer.enabled = false;
        voteWall.SetActive(false);
        voteTimerText.enabled = false;
        voteText.enabled = false;
        //drawCanvas.enabled = false;

        if(PhotonNetwork.IsMasterClient)
        {
            isMasterText.text = "¹æ ¼³Á¤À» ¹Ù²ãÁÖ¼¼¿ä!!";
        }

        //playerCanvas[0].transform.position = votePicPos[0].transform.position;
        //playerCanvas[0].transform.forward = GameObject.Find("VoteWall").transform.forward;
        //playerCanvas[0].transform.localScale = Vector3.one * 1.35f;
        PhotonNetwork.CurrentRoom.SetPropertiesListedInLobby(new string[] { "timer", "theme", "gamemode", "roomName" });

    }

    float currentTime = 0;
    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;

        // Å×½ºÆ®¿ë ½Ã°£°¨±â
        if (Input.GetKey(KeyCode.Equals))
        {
            leftTime -= 1f;
            photonView.RPC("TimeIsRunningOut", RpcTarget.Others, leftTime);
            PhotonNetwork.CurrentRoom.SetPropertiesListedInLobby(new string[] { "timer", "theme", "gamemode", "roomName" });
        }



        //if (currentTime > 5f)
        //{
        //    RoomOptionChanged();
        //    currentTime = 0f;
        //}

    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);
        //RoomOptionChanged();
    }

    void RoomOptionChanged()
    {

        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
        string m = string.Format("{0:D2}", (int)(leftTime / 60));
        string s = string.Format("{0:D2}", (int)(leftTime % 60));

        hash["timer"] = m + ":" + s;
        hash["theme"] = theme;
        hash["gamemode"] = "Compete";
        hash["roomName"] = PhotonNetwork.CurrentRoom.Name;

        PhotonNetwork.CurrentRoom.SetCustomProperties(hash);

    }

    void SetSpawnPos()
    {
        float angle = 360f / 8;
        for (int i = 0; i < 8; i++)
        {
            spawnPos[i] = transform.position + transform.forward * 2f;
            transform.Rotate(0, angle, 0);
        }
    }

    [PunRPC]
    void TimeIsRunningOut(float _leftTime)
    {
        leftTime = _leftTime;
    }

    void StateChange()
    {
        switch (state)
        {
            case gameState.Ready:
                Ready();
                break;
            case gameState.Start:
                RoundStart();
                break;
            case gameState.Playing:
                Playing();
                break;
            case gameState.Vote:
                Vote();
                break;
            case gameState.Victory:
                Victory();
                break;

        }
    }

    #region GameState
    private void Ready()
    {
        

    }

    private void RoundStart()
    {
        if (!isStart)
        {
            isStart = true;
            IFtheme.enabled = false;
            canvasR.enabled = false;
            isMasterText.enabled = false;

            DelayText.text = "°ÔÀÓÀÌ ½ÃÀÛµË´Ï´Ù!";
            DelayText.enabled = true;
            DelayTimeText.enabled = true;

            StartCoroutine(StartDelayTimer());
        }
    }

    bool isPlaying = false;
    public Text inGameTheme;
    public Text inGameTimer;

    private void Playing()
    {
        if (!isPlaying)
        {
            isPlaying = true;
            //drawCanvas.enabled = true;
            inGameTheme.text = theme;
            inGameTheme.enabled = true;
            inGameTimer.enabled = true;
            StartCoroutine(InGameTimer());
        }
    }

    bool isVote = false;
    public Text voteTimerText;
    public Text voteText;

    private void Vote()
    {
        if (!isVote)
        {
            isVote = true;
            //drawCanvas.enabled = false;

            //for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
            //{
            //    playerList[i].gameObject.transform.position = spawnPos[i];
            //}
            
                
            CameraRotate_BH.cursorType++;
            StartCoroutine(VoteTournament());
        }

    }

    private void Victory()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            state = gameState.Ready;
        }
    }
    #endregion

    void OnThemeValueChanged(string s)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            TextTheme.text = "\" " + IFtheme.text + " \"";
        }
    }

    void OnThemeSubmit(string s)
    {
        DecideTheme();
    }

    void OnThemeEndEdit(string s)
    {
        DecideTheme();
    }

    public void OnDecisionBtnClicked()
    {
        DecideTheme();
    }

    public void OnRandomBtnClicked()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            string[] s = {"ÀÌ¿µÈ£", "±èÇöÁø", "µ¹°í·¡", "½Ã°£", "°î±ªÀÌ", "¿©¿ì", "Ã»Ãá", "°¡¸é", "¿¬ÀÎ",
            "ºÒ", "¼±¹°", "ÃÊÄÝ¸´", "¹ö½º", "¼ÕÀüµî", "Çà¿î", "¿À¿°", "¾Ö¿Ïµ¿¹°", "ÇØµ¸ÀÌ", "±ÙÀ°",
        "±âÅ¸", "»§", "¿©Çà", "°Ü¿ï", "ÇØÀÏ", "¹Ýµ÷ºÒ", "°¥¸Å±â", "°úÀÚ", "Åä³¢", "À§Çè", "¼±ÅÃ" };
            IFtheme.text = s[Random.Range(0, s.Length)];
            DecideTheme();
        }
    }

    void DecideTheme()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            AlphaberCheck();
            if (IFtheme.text.Length > 0)
            {
                theme = IFtheme.text;
                print("Å×¸¶ ¼³Á¤ : " + theme);
                photonView.RPC("RpcDecideTheme", RpcTarget.Others, theme);
            }
        }
        RoomOptionChanged();
    }

    [PunRPC]
    void RpcDecideTheme(string _theme)
    {
        theme = _theme;
        TextTheme.text = "\" " + _theme + " \"";
    }

    void AlphaberCheck()
    {
        if (!Regex.IsMatch(IFtheme.text, @"[°¡-ÆR]$"))
        {
            IFtheme.text = "";
        }
    }

    float[] timerSettings = { 600f, 480f, 300f, 180f };
    int timerType = 0;
    float leftTime = 600f;
    public Button timerBtn;

    public void OnTimerBtnClicked()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            timerType++;
            timerType %= 4;
            leftTime = timerSettings[timerType];
            timerBtn.GetComponentInChildren<Text>().text = Mathf.Floor(leftTime / 60f) + " : 00";
            print("Å¸ÀÌ¸Ó ¼³Á¤ : " + Mathf.Floor(leftTime / 60f) + " ºÐ");

            photonView.RPC("RpcTimerBtnClicked", RpcTarget.Others, leftTime);
        }
        RoomOptionChanged();
    }


    [PunRPC]
    void RpcTimerBtnClicked(float _leftTime)
    {
        timerBtn.GetComponentInChildren<Text>().text = Mathf.Floor(_leftTime / 60f) + " : 00";
        leftTime = _leftTime;
    }

    bool isStart = false;
    public void OnStartBtnClicked()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            state = gameState.Start;
            StateChange();
            photonView.RPC("RpcStateStart", RpcTarget.Others, gameState.Start);
        }
    }

    [PunRPC]
    void RpcStateStart(gameState _gamestate)
    {
        state = _gamestate;
        StateChange();
    }

    public Text DelayText;
    public Text DelayTimeText;

    IEnumerator StartDelayTimer()
    {
        float delayTime = 5f;
        //float delayTime = 1f;

        while (delayTime > 0)
        {
            delayTime -= Time.deltaTime;
            DelayTimeText.text = Mathf.Floor(delayTime) + "";
            yield return null;
        }

        DelayText.enabled = false;
        DelayTimeText.enabled = false;
        canvasL.enabled = false;
        canvasF.enabled = false;

        state = gameState.Playing;
        StateChange();
    }

    IEnumerator InGameTimer()
    {
        while (leftTime > 0)
        {
            leftTime -= Time.deltaTime;
            inGameTimer.text = Mathf.Floor(leftTime / 60) + " : " + Mathf.Floor(leftTime % 60);
            yield return null;
        }

        inGameTheme.enabled = false;
        inGameTimer.enabled = false;

        DelayText.text = "°ÔÀÓÀÌ Á¾·áµË´Ï´Ù!";
        DelayText.enabled = true;
        DelayTimeText.enabled = true;

        StartCoroutine(EndDelayTimer());
    }

    IEnumerator EndDelayTimer()
    {
        float delayTime = 5f;
        //float delayTime = 1f;

        while (delayTime > 0)
        {
            delayTime -= Time.deltaTime;
            DelayTimeText.text = Mathf.Floor(delayTime) + "";
            yield return null;
        }

        DelayText.enabled = false;
        DelayTimeText.enabled = false;

        state = gameState.Vote;
        StateChange();
    }

    
    IEnumerator VoteTimer()
    {
        //float voteTime = 5f;
        float voteTime = 20f;
        voteTimerText.color = Color.black;
        voteTimerText.enabled = true;
        voteText.enabled = true;
        PlayerVote.instance.CanVote = true;

        while (voteTime > 0)
        {
            voteTime -= Time.deltaTime;
            voteTimerText.text = Mathf.Floor(voteTime) + "";
            if (voteTime < 5f)
            {
                voteTimerText.color = Color.red;
            }

            yield return null;
        }

        voteTimerText.enabled = false;
        voteText.enabled = false;
        isTimerEnd = true;
        PlayerVote.instance.CanVote = false;

    }

    List<int> finalIdx = new List<int>();
    int winnerIdx = -1;


    IEnumerator VoteTournament()
    {
        voteWall.SetActive(true);
        
        //if(PhotonNetwork.CurrentRoom.PlayerCount < 4)
        //{
        //    HangOnWall(PhotonNetwork.CurrentRoom.PlayerCount);
        //    yield return new WaitUntil(() => isTimerEnd);

        //    Qualifier(PhotonNetwork.CurrentRoom.PlayerCount);

        //    Final(PhotonNetwork.CurrentRoom.PlayerCount);
        //    yield return new WaitUntil(() => isTimerEnd);

        //    Winner();
        //}
        //else if(PhotonNetwork.CurrentRoom.PlayerCount == 4)
        //{
        //    HangOnWall(2, 0);
        //    yield return new WaitUntil(() => isTimerEnd);
        //    Qualifier(2, 0);

        //    HangOnWall(2, 2);
        //    yield return new WaitUntil(() => isTimerEnd);
        //    Qualifier(2, 2);

        //    Final(2);
        //    yield return new WaitUntil(() => isTimerEnd);

        //    Winner();
        //}
        //else if(PhotonNetwork.CurrentRoom.PlayerCount == 5)
        //{
        //    HangOnWall(3, 0);
        //    yield return new WaitUntil(() => isTimerEnd);
        //    Qualifier(3, 0);

        //    HangOnWall(2, 3);
        //    yield return new WaitUntil(() => isTimerEnd);
        //    Qualifier(2, 3);

        //    Final(2);
        //    yield return new WaitUntil(() => isTimerEnd);

        //    Winner();
        //}
        //else if (PhotonNetwork.CurrentRoom.PlayerCount == 6)
        //{
        //    HangOnWall(3, 0);
        //    yield return new WaitUntil(() => isTimerEnd);
        //    Qualifier(3, 0);

        //    HangOnWall(3, 3);
        //    yield return new WaitUntil(() => isTimerEnd);
        //    Qualifier(3, 3);

        //    Final(2);
        //    yield return new WaitUntil(() => isTimerEnd);

        //    Winner();
        //}
        //else if (PhotonNetwork.CurrentRoom.PlayerCount == 7)
        //{
        //    HangOnWall(4, 0);
        //    yield return new WaitUntil(() => isTimerEnd);
        //    Qualifier(4, 0);

        //    HangOnWall(3, 4);
        //    yield return new WaitUntil(() => isTimerEnd);
        //    Qualifier(3, 4);

        //    Final(2);
        //    yield return new WaitUntil(() => isTimerEnd);

        //    Winner();
        //}
        //if (PhotonNetwork.CurrentRoom.PlayerCount == 8)
        //{
        HangOnWall(4, 0);
        yield return new WaitUntil(() => isTimerEnd);
        Qualifier(4, 0);

        HangOnWall(4, 4);
        yield return new WaitUntil(() => isTimerEnd);
        Qualifier(4, 4);

        Final(2);
        yield return new WaitUntil(() => isTimerEnd);

        Winner();
        //}
    }


    bool isTimerEnd = false;
    void HangOnWall(int playerCount, int playerNum = 0)
    {
        print("HangOnWall");

        isTimerEnd = false;

        for (int i = 0; i < 4; i++)
        {
            votePicPos[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < 8; i++)
        {
            playerCanvas[i].transform.position = canvasPos[i].transform.position;
            playerCanvas[i].transform.forward = canvasPos[i].transform.forward;
            playerCanvas[i].transform.localScale = Vector3.one;
        }
        
        for (int i = 0; i < playerCount; i++)
        {
            votePicPos[i].gameObject.SetActive(true);
            playerCanvas[i + playerNum].transform.position = votePicPos[i].transform.position;
            playerCanvas[i + playerNum].transform.forward = GameObject.Find("VoteWall").transform.forward;
            playerCanvas[i + playerNum].transform.localScale = Vector3.one * 1.35f;
        }

        StartCoroutine(VoteTimer());
        
    }

    void Qualifier(int playerCount, int playerNum = 0)
    {
        print("Qualifier");

        int maxIdx = -1;
        int max = 0;

        for (int i = 0; i < playerCount; i++)
        {
            int likes = playerCanvas[i + playerNum].GetComponentInChildren<Likes>().likes;
            if(max < likes)
            {
                max = likes;
                maxIdx = i + playerNum;
            }
        }

        finalIdx.Add(maxIdx);
    }

    void Final(int playerCount)
    {
        print("Final");

        isTimerEnd = false;

        for (int i = 0; i < 4; i++)
        {
            votePicPos[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < 8; i++)
        {
            playerCanvas[i].transform.position = canvasPos[i].transform.position;
            playerCanvas[i].transform.forward = canvasPos[i].transform.forward;
            playerCanvas[i].transform.localScale = Vector3.one;
        }

        for (int i = 0; i < 2; i++)
        {
            votePicPos[i].gameObject.SetActive(true);
            playerCanvas[finalIdx[i]].GetComponentInChildren<Likes>().likes = 0;
            playerCanvas[finalIdx[i]].transform.position = votePicPos[i].transform.position;
            playerCanvas[finalIdx[i]].transform.forward = GameObject.Find("VoteWall").transform.forward;
            playerCanvas[finalIdx[i]].transform.localScale = Vector3.one * 1.35f;
        }

        StartCoroutine(VoteTimer());

    }

    void Winner()
    {
        print("Winner");

        int maxIdx = -1;
        int max = 0;

        for (int i = 0; i < 2; i++)
        {
            int likes = playerCanvas[finalIdx[i]].GetComponentInChildren<Likes>().likes;
            if (max < likes)
            {
                max = likes;
                maxIdx = finalIdx[i];
            }
        }

        winnerIdx = maxIdx;
        state = gameState.Victory;

        for (int i = 0; i < 4; i++)
        {
            votePicPos[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < 8; i++)
        {
            playerCanvas[i].transform.position = canvasPos[i].transform.position;
            playerCanvas[i].transform.forward = canvasPos[i].transform.forward;
            playerCanvas[i].transform.localScale = Vector3.one;
        }

        votePicPos[4].gameObject.SetActive(true);
        playerCanvas[winnerIdx].transform.position = votePicPos[4].transform.position;
        playerCanvas[winnerIdx].transform.forward = GameObject.Find("VoteWall").transform.forward;
        playerCanvas[winnerIdx].transform.localScale = Vector3.one * 2f;

    }
}
