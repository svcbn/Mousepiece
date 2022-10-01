using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YS_ButtonManager : MonoBehaviour
{
    public GameObject colorPicker, picker, back, front, eraser, clear, basicBrush, oilPaintBrush, waterColorBrush, pencil, calligraphy, marker, crayon, spray, fingerBlending, brushSize, circle;
    //public BrushTest_BH brush;
    public BrushNet_YS brushNet;
    public BrushTest5 bt5;

    float circle_temp;

    // Start is called before the first frame update
    void Start()
    {
        //brush = GameObject.Find("Player(Clone)").GetComponent<BrushTest_BH>();
        brushNet = GameObject.Find("Player(Clone)").GetComponent<BrushNet_YS>();
        bt5 = GameObject.Find("Player(Clone)").GetComponent<BrushTest5>();

        circle_temp = circle.GetComponent<RectTransform>().localPosition.x;
    }

    // Update is called once per frame
    void Update()
    {
        // �귯�� ������ ����
        //BrushSize();
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
        if(bt5.b_eraser == false) //brush.b_eraser == false || 
        {
            //brush.b_eraser = true;
            bt5.b_eraser = true;

            // ���찳 ���� �Ҵ�
            bt5.drawPrefab_temp = bt5.drawPrefab;
            bt5.drawPrefab = Resources.Load<GameObject>("YS/Eraser");
        }
        else
        {
            //brush.b_eraser = false;
            bt5.b_eraser = false;

            // ���찳�� ����ϱ� �� ������
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

        // �귯�� ���� �Ҵ�
        bt5.drawPrefab = Resources.Load<GameObject>("YS/Brush");
    }

    public void OilPaintBrush()
    {
        bt5.toolNum = 8;

        // ��ȭ ���� �Ҵ�
        bt5.drawPrefab = Resources.Load<GameObject>("YS/OilPaint");
    }

    public void WaterColorBrush()
    {
        bt5.toolNum = 9;

        // ��äȭ ���� �Ҵ�
        bt5.drawPrefab = Resources.Load<GameObject>("YS/WaterPaint");
    }

    public void Pencil()
    {
        bt5.toolNum = 4;

        // ���� ���� �Ҵ�
        bt5.drawPrefab = Resources.Load<GameObject>("YS/Pencil");
    }

    public void Calligraphy()
    {
        bt5.toolNum = 5;

        // Ķ���׶��� ���� �Ҵ�
        bt5.drawPrefab = Resources.Load<GameObject>("YS/Calligraphy");
    }

    public void Marker()
    {
        bt5.toolNum = 3;

        // ��Ŀ ���� �Ҵ�
        bt5.drawPrefab = Resources.Load<GameObject>("YS/Marker");
    }

    public void Crayon()
    {
        bt5.toolNum = 6;

        // ũ���� ���� �Ҵ�
        bt5.drawPrefab = Resources.Load<GameObject>("YS/Crayon");
    }

    public void Spray()
    {
        bt5.toolNum = 7;

        // �������� ���� �Ҵ�
        bt5.drawPrefab = Resources.Load<GameObject>("YS/Spray");
    }

    public void FingerBlending()
    {
        bt5.toolNum = 2;

        // ���� ���� �Ҵ�
        bt5.drawPrefab = Resources.Load<GameObject>("YS/Blending");
    }

    public void BrushSize()
    {
        // circle�� �����̸�,
        if(circle_temp != circle.GetComponent<RectTransform>().localPosition.x)
        {
            // circle�� �񱳴���� ���� �귯���� ũ�⿡ �ش��ϴ� ��ġ(0.05 * 0.0013812154696133)
            if (circle_temp < circle.GetComponent<RectTransform>().localPosition.x)
            {
                // ������ ��� (circle�� ������ �� �ִ� ������ 362, �귯�� ������� 0.5�� ���)
                bt5.size += (circle.GetComponent<RectTransform>().localPosition.x - circle_temp) * 0.0013812154696133f;
            }
            else if(circle_temp > circle.GetComponent<RectTransform>().localPosition.x)
            {
                // ������ ��� (circle�� ������ �� �ִ� ������ 362, �귯�� ������� 0.5�� ���)
                bt5.size -= (circle_temp - circle.GetComponent<RectTransform>().localPosition.x) * 0.0013812154696133f;
            }
        }

        // circle�� ���� ��ġ ����
        circle_temp = circle.GetComponent<RectTransform>().localPosition.x;
    }
}
