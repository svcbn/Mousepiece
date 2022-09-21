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
        
    }

    // Update is called once per frame
    void Update()
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

        // Å×½ºÆ®¿ë ½Ã°£°¨±â
        if (Input.GetKey(KeyCode.Equals))
        {
            leftTime -= 0.5f;
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
            //for (int i = 0; i < playerList.Count; i++)
            //{
            //    playerList[i].transform.position = Vector3.zero;
            //}
            StartCoroutine(VoteTournament());
        }

    }

    private void Victory()
    {

    }
    #endregion

    void OnThemeValueChanged(string s)
    {
        TextTheme.text = "\" " + IFtheme.text + " \"";
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
        string[] s = {"ÀÌ¿µÈ£", "±èÇöÁø", "µ¹°í·¡", "½Ã°£", "°î±ªÀÌ", "¿©¿ì", "Ã»Ãá", "°¡¸é", "¿¬ÀÎ",
            "ºÒ", "¼±¹°", "ÃÊÄÝ¸´", "¹ö½º", "¼ÕÀüµî", "Çà¿î", "¿À¿°", "¾Ö¿Ïµ¿¹°", "ÇØµ¸ÀÌ", "±ÙÀ°",
        "±âÅ¸", "»§", "¿©Çà", "°Ü¿ï", "ÇØÀÏ", "¹Ýµ÷ºÒ", "°¥¸Å±â", "°úÀÚ", "Åä³¢", "À§Çè", "¼±ÅÃ" };
        IFtheme.text = s[Random.Range(0, s.Length)];
        DecideTheme();
    }

    void DecideTheme()
    {
        AlphaberCheck();
        if (IFtheme.text.Length > 0)
        {
            theme = IFtheme.text;
            print("Å×¸¶ ¼³Á¤ : " + theme);
        }
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
        timerType++;
        timerType %= 4;
        leftTime = timerSettings[timerType];
        timerBtn.GetComponentInChildren<Text>().text = Mathf.Floor(leftTime / 60f) + " : 00";
        print("Å¸ÀÌ¸Ó ¼³Á¤ : " + Mathf.Floor(leftTime / 60f) + " ºÐ");
    }

    bool isStart = false;
    public void OnStartBtnClicked()
    {
        state = gameState.Start;
    }

    public Text DelayText;
    public Text DelayTimeText;

    IEnumerator StartDelayTimer()
    {
        float delayTime = 5f;
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

        while (delayTime > 0)
        {
            delayTime -= Time.deltaTime;
            DelayTimeText.text = Mathf.Floor(delayTime) + "";
            yield return null;
        }

        DelayText.enabled = false;
        DelayTimeText.enabled = false;

        state = gameState.Vote;
    }

    
    IEnumerator VoteTimer()
    {
        float voteTime = 20f;
        voteTimerText.enabled = true;
        voteText.enabled = true;

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

    }


    IEnumerator VoteTournament()
    {
        voteWall.SetActive(true);
        int count = 0;
        
        for (int i = 0; i < playerList.Count / 5 + 1; i++)
        {
            HangOnWall(count);
            yield return new WaitUntil(() => isTimerEnd);
            count+=4;
        }
    }

    bool isTimerEnd = false;
    void HangOnWall(int count)
    {
        isTimerEnd = false;
        int n = playerList.Count;

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

        if (n > 4 && count == 0)
        {
            n = 4;
        }

        for (int i = 0; i < (n - count) % 5; i++)
        {
            votePicPos[i].gameObject.SetActive(true);
            playerCanvas[i + count].GetComponentsInChildren<Transform>()[1].position = votePicPos[i].transform.position + Vector3.forward * 0.1f;
            playerCanvas[i + count].GetComponentsInChildren<Transform>()[1].rotation = Quaternion.Euler(0, -9, 0);
            playerCanvas[i + count].GetComponentsInChildren<Transform>()[1].localScale = new Vector3(4, 5.33f, 0.05f);
        }

        StartCoroutine(VoteTimer());
        
    }
}
