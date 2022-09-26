using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YS_MousePointer : MonoBehaviour
{
    public Texture2D[] cursorTexArray = new Texture2D[9];
    public Texture2D cursorTex;
    public BrushTest5 bt5;

    // ���� �ٲ������ Ȯ���ϱ� ���� ����
    int toolNum_temp;

    // Start is called before the first frame update
    void Start()
    {
        cursorTex = cursorTexArray[0];

        StartCoroutine("MouseCursor");

        bt5 = GameObject.Find("DrawManager").GetComponent<BrushTest5>();
        toolNum_temp = bt5.toolNum;
    }

    // Update is called once per frame
    void Update()
    {
        if(bt5.toolNum != toolNum_temp)
        {
            cursorTex = cursorTexArray[bt5.toolNum - 1];

            StartCoroutine("MouseCursor");

            toolNum_temp = bt5.toolNum;
        }
    }

    IEnumerator MouseCursor()
    {
        // �������� �Ϸ�� ������ ���
        yield return new WaitForEndOfFrame();

        Cursor.SetCursor(cursorTex, Vector2.zero, CursorMode.Auto);
    }
}
