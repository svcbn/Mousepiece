using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YS_ButtonManager : MonoBehaviour
{
    public GameObject colorPicker, picker, back, front, eraser, clear, basicBrush, oilPaintBrush, waterColorBrush, pencil, calligraphy, marker, crayon, spray, fingerBlending, brushSize, size;
    public BrushTest_BH brush;
    public BrushTest5 bt5;
    // Start is called before the first frame update
    void Start()
    {
        brush = GameObject.Find("DrawManager").GetComponent<BrushTest_BH>();
        bt5 = GameObject.Find("DrawManager").GetComponent<BrushTest5>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PaletteOnOff()
    {
        if(colorPicker.activeSelf == false)
        {
            colorPicker.SetActive(true);
            picker.SetActive(true);
            back.SetActive(true);
            front.SetActive(true);
            eraser.SetActive(true);
            clear.SetActive(true);
            basicBrush.SetActive(true);
            oilPaintBrush.SetActive(true);
            waterColorBrush.SetActive(true);
            pencil.SetActive(true);
            calligraphy.SetActive(true);
            marker.SetActive(true);
            crayon.SetActive(true);
            spray.SetActive(true);
            fingerBlending.SetActive(true);
            brushSize.SetActive(true);
        }
        else
        {
            colorPicker.SetActive(false);
            picker.SetActive(false);
            back.SetActive(false);
            front.SetActive(false);
            eraser.SetActive(false);
            clear.SetActive(false);
            basicBrush.SetActive(false);
            oilPaintBrush.SetActive(false);
            waterColorBrush.SetActive(false);
            pencil.SetActive(false);
            calligraphy.SetActive(false);
            marker.SetActive(false);
            crayon.SetActive(false);
            spray.SetActive(false);
            fingerBlending.SetActive(false);
            brushSize.SetActive(false);
        }
    }

    public void Eraser()
    {
        if(brush.b_eraser == false || bt5.b_eraser == false)
        {
            brush.b_eraser = true;
            bt5.b_eraser = true;

            // 지우개 동적 할당
            bt5.drawPrefab_temp = bt5.drawPrefab;
            bt5.drawPrefab = Resources.Load<GameObject>("YS/Eraser");
        }
        else
        {
            brush.b_eraser = false;
            bt5.b_eraser = false;

            // 지우개를 사용하기 전 도구로
            bt5.drawPrefab = bt5.drawPrefab_temp;
        }
    }

    public void BackFunction()
    {
        for (int i = bt5.lines.Count - 1; i >= 0; i--)
        {
            if (bt5.lines[i].activeSelf == true)
            {
                bt5.lines[i].SetActive(false);
                break;
            }
        }
    }

    public void FrontFunction()
    {
        for (int i = 0; i < bt5.lines.Count; i++)
        {
            if (bt5.lines[i].activeSelf == false)
            {
                bt5.lines[i].SetActive(true);
                break;
            }
        }
    }

    public void CanvasClear()
    {
        for (int i = 0; i < bt5.lines.Count; i++)
        {
            Destroy(bt5.lines[i].gameObject);
        }
        bt5.lines.Clear();
    }

    public void BasicBrush()
    {
        bt5.toolNum = 1;

        // 브러쉬 동적 할당
        bt5.drawPrefab = Resources.Load<GameObject>("YS/Brush");
    }

    public void OilPaintBrush()
    {
        bt5.toolNum = 8;

        // 유화 동적 할당
        bt5.drawPrefab = Resources.Load<GameObject>("YS/OilPaint");
    }

    public void WaterColorBrush()
    {
        bt5.toolNum = 9;

        // 수채화 동적 할당
        bt5.drawPrefab = Resources.Load<GameObject>("YS/WaterPaint");
    }

    public void Pencil()
    {
        bt5.toolNum = 4;

        // 연필 동적 할당
        bt5.drawPrefab = Resources.Load<GameObject>("YS/Pencil");
    }

    public void Calligraphy()
    {
        bt5.toolNum = 5;

        // 캘리그라피 동적 할당
        bt5.drawPrefab = Resources.Load<GameObject>("YS/Calligraphy");
    }

    public void Marker()
    {
        bt5.toolNum = 3;

        // 마커 동적 할당
        bt5.drawPrefab = Resources.Load<GameObject>("YS/Marker");
    }

    public void Crayon()
    {
        bt5.toolNum = 6;

        // 크레용 동적 할당
        bt5.drawPrefab = Resources.Load<GameObject>("YS/Crayon");
    }

    public void Spray()
    {
        bt5.toolNum = 7;

        // 스프레이 동적 할당
        bt5.drawPrefab = Resources.Load<GameObject>("YS/Spray");
    }

    public void FingerBlending()
    {
        bt5.toolNum = 2;

        // 블렌딩 동적 할당
        bt5.drawPrefab = Resources.Load<GameObject>("YS/Blending");
    }

    public void BrushSize()
    {

    }
}
