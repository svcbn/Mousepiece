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
        string[] s = {"�̿�ȣ", "������", "����", "�ð�", "���", "����", "û��", "����", "����",
            "��", "����", "���ݸ�", "����", "������", "���", "����", "�ֿϵ���", "�ص���", "����", 
        "��Ÿ", "��", "����", "�ܿ�", "����", "�ݵ���", "���ű�", "����", "�䳢", "����", "����" };
        IFtheme.text = s[Random.Range(0, s.Length)];
        DecideTheme();
    }

    void DecideTheme()
    {
        theme = IFtheme.text;
        print("�׸� ���� : " + theme);
    }



}
