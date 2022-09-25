using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using Photon.Realtime;

public class GameManager_BH : MonoBehaviourPunCallbacks
{
    public static GameManager_BH instance;

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

    public Canvas drawCanvas;

    public Transform[] canvasPos;
    public GameObject[] playerCanvas;
    public Transform[] votePicPos;

    public GameObject voteWall;

    public List<PhotonView> playerPV = new List<PhotonView>();
    public List<GameObject> playerList = new List<GameObject>();
    
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.SerializationRate = 60;
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.Instantiate("Player", Vector3.zero + Vector3.up * 0.5f, Quaternion.identity);

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
        drawCanvas.enabled = false;

        if(PhotonNetwork.IsMasterClient)
        {
            isMasterText.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 테스트용 시간감기
        if (Input.GetKey(KeyCode.Equals))
        {
            leftTime -= 1f;
            photonView.RPC("TimeIsRunningOut", RpcTarget.Others, leftTime);
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

            DelayText.text = "게임이 시작됩니다!";
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
            drawCanvas.enabled = true;
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
            drawCanvas.enabled = false;
            for (int i = 0; i < playerList.Count; i++)
            {
                playerList[i].transform.position = Vector3.zero;
            }
            CameraRotate_BH.cursorType++;
            StartCoroutine(VoteTournament());
        }

    }

    private void Victory()
    {

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
            string[] s = {"이영호", "김현진", "돌고래", "시간", "곡괭이", "여우", "청춘", "가면", "연인",
            "불", "선물", "초콜릿", "버스", "손전등", "행운", "오염", "애완동물", "해돋이", "근육",
        "기타", "빵", "여행", "겨울", "해일", "반딧불", "갈매기", "과자", "토끼", "위험", "선택" };
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
                print("테마 설정 : " + theme);
                photonView.RPC("RpcDecideTheme", RpcTarget.Others, theme);
            }
        }
    }

    [PunRPC]
    void RpcDecideTheme(string _theme)
    {
        theme = _theme;
        TextTheme.text = "\" " + _theme + " \"";
    }

    void AlphaberCheck()
    {
        if (!Regex.IsMatch(IFtheme.text, @"[가-힣]$"))
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
            print("타이머 설정 : " + Mathf.Floor(leftTime / 60f) + " 분");

            photonView.RPC("RpcTimerBtnClicked", RpcTarget.Others, leftTime);
        }
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
        //float delayTime = 5f;
        float delayTime = 1f;

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

        DelayText.text = "게임이 종료됩니다!";
        DelayText.enabled = true;
        DelayTimeText.enabled = true;

        StartCoroutine(EndDelayTimer());
    }

    IEnumerator EndDelayTimer()
    {
        //float delayTime = 5f;
        float delayTime = 0f;

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
        float voteTime = 20f;
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

    int[] qualifier = new int[4];
    GameObject[] final = new GameObject[2];


    IEnumerator VoteTournament()
    {
        voteWall.SetActive(true);
        
        if(playerList.Count < 4)
        {
            HangOnWall(playerList.Count);
            yield return new WaitUntil(() => isTimerEnd);
            //Winner();
        }
        else if(playerList.Count == 4)
        {
            HangOnWall(2, 0);
            yield return new WaitUntil(() => isTimerEnd);
            HangOnWall(2, 2);
            yield return new WaitUntil(() => isTimerEnd);
            Final();
        }
        else if(playerList.Count == 5)
        {
            HangOnWall(3, 0);
            yield return new WaitUntil(() => isTimerEnd);
            HangOnWall(2, 3);
            yield return new WaitUntil(() => isTimerEnd);
            Final();
        }
        else if (playerList.Count == 6)
        {
            HangOnWall(3, 0);
            yield return new WaitUntil(() => isTimerEnd);
            HangOnWall(3, 3);
            yield return new WaitUntil(() => isTimerEnd);
            Final();
        }
        else if (playerList.Count == 7)
        {
            HangOnWall(4, 0);
            yield return new WaitUntil(() => isTimerEnd);
            HangOnWall(3, 4);
            yield return new WaitUntil(() => isTimerEnd);
            Final();
        }
        else if (playerList.Count == 8)
        {
            HangOnWall(4, 0);
            yield return new WaitUntil(() => isTimerEnd);
            HangOnWall(4, 4);
            yield return new WaitUntil(() => isTimerEnd);
            Final();
        }
    }


    bool isTimerEnd = false;
    void HangOnWall(int playerCount, int playerNum = 0)
    {
        isTimerEnd = false;

        for (int i = 0; i < 4; i++)
        {
            votePicPos[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < 8; i++)
        {
            playerCanvas[i].GetComponentsInChildren<Transform>()[1].position = canvasPos[i].transform.position;
            playerCanvas[i].GetComponentsInChildren<Transform>()[1].rotation = canvasPos[i].transform.rotation;
            playerCanvas[i].GetComponentsInChildren<Transform>()[1].localScale = new Vector3(3, 4, 0.04f);
        }

        for (int i = 0; i < playerCount; i++)
        {
            votePicPos[i].gameObject.SetActive(true);
            playerCanvas[i + playerNum].GetComponentsInChildren<Transform>()[1].position = votePicPos[i].transform.position + Vector3.forward * 0.1f;
            playerCanvas[i + playerNum].GetComponentsInChildren<Transform>()[1].rotation = Quaternion.Euler(0, -9, 0);
            playerCanvas[i + playerNum].GetComponentsInChildren<Transform>()[1].localScale = new Vector3(4, 5.33f, 0.05f);
        }

        StartCoroutine(VoteTimer());
        
    }

    void Final()
    {

    }
}
