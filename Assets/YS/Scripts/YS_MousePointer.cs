using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YS_MousePointer : MonoBehaviour
{
    public Texture2D[] cursorTexArray = new Texture2D[9];
    public Texture2D cursorTex;
    //public BrushTest5 bt5;
    public BrushNet_YS brushNet;

    // 툴이 바뀌었는지 확인하기 위한 변수
    int toolNum_temp;

    // Start is called before the first frame update
    void Start()
    {
        cursorTex = cursorTexArray[0];

        StartCoroutine("MouseCursor");

        //bt5 = GameObject.Find("DrawManager").GetComponent<BrushTest5>();
        toolNum_temp = brushNet.toolNum;
    }

    // Update is called once per frame
    void Update()
    {
        // 브러쉬 툴 넣어주기
        if (brushNet == null)
        {
            if (CollaborateModeManager_BH.instance)
            {
                brushNet = GameObject.Find("Player1(Clone)").GetComponent<BrushNet_YS>();
            }
            else if (CompeteModeManager_BH.instance)
            {
                brushNet = GameObject.Find("Player(Clone)").GetComponent<BrushNet_YS>();
            }
        }

        if (brushNet.toolNum != toolNum_temp)
        {
            cursorTex = cursorTexArray[brushNet.toolNum - 1];

            StartCoroutine("MouseCursor");

            toolNum_temp = brushNet.toolNum;
        }
    }

    IEnumerator MouseCursor()
    {
        // 렌더링이 완료될 때까지 대기
        yield return new WaitForEndOfFrame();

        Cursor.SetCursor(cursorTex, Vector2.zero, CursorMode.Auto);
    }
}
