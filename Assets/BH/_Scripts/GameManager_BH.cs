using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class GameManager_BH : MonoBehaviourPun
{
    public enum gameState
    {
        Ready,
        Start,
        Playing,
        Vote,
        Victory
    }

    public static gameState state = gameState.Ready;
    string theme;

    public Canvas canvasR;
    public InputField IFtheme;
    public Text TextTheme;

    public Button startBtn;
    public Button decisionBtn;

    public Canvas canvasL;

    public Canvas canvasF;

    public Canvas InGameCanvas;
    

    // Start is called before the first frame update
    void Start()
    {
        IFtheme.onValueChanged.AddListener(OnThemeValueChanged);
        IFtheme.onSubmit.AddListener(OnThemeSubmit);

    }

    // Update is called once per frame
    void Update()
    {
        switch(state)
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
    }

    private void Playing()
    {
    }

    private void Vote()
    {
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

    public void OnDecisionBtnClicked()
    {
        DecideTheme();
    }
    
    public void OnRandomBtnClicked()
    {
        string[] s = {"이영호", "김현진", "돌고래", "시간", "곡괭이", "여우", "청춘", "가면", "연인",
            "불", "선물", "초콜릿", "버스", "손전등", "행운", "오염", "애완동물", "해돋이", "근육", 
        "기타", "빵", "여행", "겨울", "해일", "반딧불", "갈매기", "과자", "토끼", "위험", "선택" };
        IFtheme.text = s[Random.Range(0, s.Length)];
        DecideTheme();
    }

    void DecideTheme()
    {
        theme = IFtheme.text;
        print("테마 설정 : " + theme);
    }



}
