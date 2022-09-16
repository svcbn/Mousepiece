using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushTest2 : MonoBehaviour
{
    public GameObject drawPrefab;
    public GameObject drawCanvas;
    GameObject theTrail;
    Plane planObj;
    Vector3 startPos;

    // ������
    float size = 0.15f;
    // �� ����
    public GameObject colorObject;

    // ���찳
    bool b_eraser;

    // �̵�����
    Vector3 canvasPos;
    Vector3 dis;

    // Start is called before the first frame update
    void Start()
    {
        planObj = new Plane(Camera.main.transform.forward, transform.position);

        canvasPos = drawCanvas.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Color eraser = Color.white;

        Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        if(Input.GetMouseButtonDown(0))
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            float _dis;
            if(Physics.Raycast(mouseRay, out hit))
            {
                if(hit.transform.name == drawCanvas.transform.name)
                {
                    // ���� �׸��� ��, ������ ����
                    drawPrefab.GetComponent<TrailRenderer>().widthMultiplier = size;
                    // ���� �׸��� ��, �� ����
                    drawPrefab.GetComponent<TrailRenderer>().startColor = colorObject.GetComponent<ColorPickerTest>().selectedColor;
                    drawPrefab.GetComponent<TrailRenderer>().endColor = colorObject.GetComponent<ColorPickerTest>().selectedColor;
                    // ���찳
                    if(b_eraser == true)
                    {
                        drawPrefab.GetComponent<TrailRenderer>().startColor = eraser;
                        drawPrefab.GetComponent<TrailRenderer>().endColor = eraser;
                    }
                    // ���߿� ���� ���� ���� �ö���Բ�
                    drawPrefab.GetComponent<TrailRenderer>().sortingOrder++;

                    // �� ����
                    theTrail = (GameObject)Instantiate(drawPrefab, objPosition, Quaternion.identity);
                    theTrail.transform.SetParent(drawCanvas.transform, false);

                    if(planObj.Raycast(mouseRay, out _dis))
                    {
                        startPos = mouseRay.GetPoint(_dis);
                        theTrail.transform.position = startPos;
                    }
                }
            }
        }
        else if(Input.GetMouseButton(0))
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            float _dis;
            if (Physics.Raycast(mouseRay, out hit))
            {
                if (hit.transform.name == drawCanvas.transform.name)
                {
                    if (planObj.Raycast(mouseRay, out _dis))
                    {
                        theTrail.transform.position = mouseRay.GetPoint(_dis);
                    }
                }
            }
        }

        // ������ ����
        Size();

        // ���찳
        Eraser();

        // �̵�
        Moving();
    }

    void Size()
    {
        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            size += 0.1f;
        }
        else if(Input.GetKeyDown(KeyCode.Alpha9))
        {
            size -= 0.1f;
        }
    }

    void Eraser()
    {
        if(Input.GetKeyDown(KeyCode.E) && b_eraser == false)
        {
            b_eraser = true;
        }
        else if(Input.GetKeyDown(KeyCode.E) && b_eraser == true)
        {
            b_eraser = false;
        }
    }

    void Moving()
    {
        if (drawCanvas.transform.position != canvasPos)
        {
            dis = drawCanvas.transform.position - canvasPos;

            for (int i = 0; i < theTrail.GetComponent<TrailRenderer>().positionCount; i++)
            {
                Vector3 drawPos = theTrail.GetComponent<TrailRenderer>().GetPosition(i);

                theTrail.GetComponent<TrailRenderer>().SetPosition(i, drawPos + dis);
            }
        }

        canvasPos = drawCanvas.transform.position;
    }
}
