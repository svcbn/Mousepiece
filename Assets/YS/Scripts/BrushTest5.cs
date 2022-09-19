using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushTest5 : MonoBehaviour
{
    public GameObject drawPrefab;
    public GameObject drawCanvas;
    GameObject theTrail;
    Plane planObj;
    Vector3 startPos;
    Vector3 nextPos;

    // 사이즈
    float size = 0.05f;
    // 색 설정
    public GameObject colorObject;

    // 지우개
    public bool b_eraser;

    // 이동관련
    Vector3 canvasPos;
    Vector3 dis;

    // 모든 선 저장
    List<GameObject> lines = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        planObj = new Plane(Camera.main.transform.forward, drawCanvas.transform.position);

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
                    // 선을 그리기 전, 사이즈 설정
                    drawPrefab.GetComponent<LineRenderer>().widthMultiplier = size;
                    // 선을 그리기 전, 색 설정
                    drawPrefab.GetComponent<LineRenderer>().startColor = colorObject.GetComponent<ColorPickerTest>().selectedColor;
                    drawPrefab.GetComponent<LineRenderer>().endColor = colorObject.GetComponent<ColorPickerTest>().selectedColor;
                    // 지우개
                    if(b_eraser == true)
                    {
                        drawPrefab.GetComponent<LineRenderer>().startColor = eraser;
                        drawPrefab.GetComponent<LineRenderer>().endColor = eraser;
                    }
                    // 나중에 생긴 선은 위에 올라오게끔
                    drawPrefab.GetComponent<LineRenderer>().sortingOrder++;

                    // 선 생성
                    theTrail = (GameObject)Instantiate(drawPrefab, objPosition, Quaternion.identity);
                    theTrail.transform.SetParent(drawCanvas.transform, false);
                    // 만약 생성될 때, 리스트에 active가 false인 것들은 삭제
                    for (int i = 0; i < lines.Count; i++)
                    {
                        if(lines[i].activeSelf == false)
                        {
                            Destroy(lines[i].gameObject); // 데이터 관리(쓸모없는 것은 지우기)
                            lines.RemoveAt(i);
                            i--;
                        }
                    }
                    // 생성 후, 리스트에 넣어주기
                    lines.Add(theTrail);

                    if(Physics.Raycast(mouseRay, out hit))
                    {
                        //startPos = mouseRay.GetPoint(_dis);
                        //startPos.z = drawCanvas.transform.position.z;
                        startPos = hit.point;

                        theTrail.GetComponent<LineRenderer>().SetPosition(0, startPos);
                        theTrail.GetComponent<LineRenderer>().SetPosition(1, startPos);
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
                    if (Physics.Raycast(mouseRay, out hit))
                    {
                        //nextPos = mouseRay.GetPoint(_dis);
                        //nextPos.z = drawCanvas.transform.position.z;
                        nextPos = hit.point;

                        theTrail.GetComponent<LineRenderer>().positionCount++;
                        int positionIndex = theTrail.GetComponent<LineRenderer>().positionCount - 1;
                        theTrail.GetComponent<LineRenderer>().SetPosition(positionIndex, nextPos);
                    }
                }
            }
        }

        // 사이즈 조절
        Size();

        // 지우개
        Eraser();

        // 이동
        Moving();

        // 실행취소(뒤로)
        CtrZ();

        // 되돌리기(앞으로)
        CtrY();

        // 전부 지우기
        AllClear();
    }

    void Size()
    {
        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            size += 0.01f;
        }
        else if(Input.GetKeyDown(KeyCode.Alpha9))
        {
            size -= 0.01f;
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

            for(int i = 0; i < lines.Count; i++)
            {
                for (int j = 0; j < lines[i].GetComponent<LineRenderer>().positionCount; j++)
                {
                    Vector3 drawPos = lines[i].GetComponent<LineRenderer>().GetPosition(j);

                    lines[i].GetComponent<LineRenderer>().SetPosition(j, drawPos + dis);
                }
            }
        }

        canvasPos = drawCanvas.transform.position;
    }

    void CtrZ()
    {
        if(Input.GetKeyDown(KeyCode.Home))
        {
            for(int i = lines.Count - 1; i >= 0; i--)
            {
                if(lines[i].activeSelf == true)
                {
                    lines[i].SetActive(false);
                    break;
                }
            }
        }
    }

    void CtrY()
    {
        if (Input.GetKeyDown(KeyCode.End))
        {
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].activeSelf == false)
                {
                    lines[i].SetActive(true);
                    break;
                }
            }
        }
    }

    void AllClear()
    {
        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            for(int i = 0; i < lines.Count; i++)
            {
                Destroy(lines[i].gameObject);
            }
            lines.Clear();
        }
    }
}
