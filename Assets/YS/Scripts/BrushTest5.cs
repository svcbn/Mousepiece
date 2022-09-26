using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushTest5 : MonoBehaviour
{
    // 도구 번호
    public int toolNum = 1;

    public GameObject drawPrefab;
    public GameObject drawCanvas, drawCanvas_parent;
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
    public GameObject drawPrefab_temp;

    // 이동관련
    Vector3 canvasPos;
    Vector3 dis;

    // 모든 선 저장
    public List<GameObject> lines = new List<GameObject>();

    // 스포이트
    public Color spuit;

    // 선을 캔버스 위에서 그리고 있는지 판단(특히 그리다가 삐져나갈 경우)
    bool b_onCanvas;

    // 레이어
    string[] layerName = new string[] { "Layer1", "Layer2", "Layer3" };
    int layerNum = 0;

    // Order in SortingLayer
    int sortingOrderNum;

    // Start is called before the first frame update
    void Start()
    {
        planObj = new Plane(Camera.main.transform.forward, drawCanvas.transform.position);

        canvasPos = drawCanvas.transform.position;

        drawPrefab = Resources.Load<GameObject>("YS/Brush");
        drawPrefab.GetComponent<LineRenderer>().useWorldSpace = false;
        drawPrefab.GetComponent<LineRenderer>().sortingLayerName = layerName[layerNum];
        sortingOrderNum = drawPrefab.GetComponent<LineRenderer>().sortingOrder;
    }

    // Update is called once per frame
    void Update()
    {
        // 도구 (1번은 브러쉬, 2번은 블렌딩, 3번은 마커, 4번은 연필, 5번은 캘리그라피, 6번은 크레용, 7번은 스프레이, 8번은 유화, 9번은 수채화)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            toolNum = 1;

            // 브러쉬 동적 할당
            drawPrefab = Resources.Load<GameObject>("YS/Brush");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            toolNum = 2;

            // 블렌딩 동적 할당
            drawPrefab = Resources.Load<GameObject>("YS/Blending");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            toolNum = 3;

            // 마커 동적 할당
            drawPrefab = Resources.Load<GameObject>("YS/Marker");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            toolNum = 4;

            // 연필 동적 할당
            drawPrefab = Resources.Load<GameObject>("YS/Pencil");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            toolNum = 5;

            // 캘리그라피 동적 할당
            drawPrefab = Resources.Load<GameObject>("YS/Calligraphy");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            toolNum = 6;

            // 크레용 동적 할당
            drawPrefab = Resources.Load<GameObject>("YS/Crayon");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            toolNum = 7;

            // 스프레이 동적 할당
            drawPrefab = Resources.Load<GameObject>("YS/Spray");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            toolNum = 8;

            // 유화 동적 할당
            drawPrefab = Resources.Load<GameObject>("YS/OilPaint");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            toolNum = 9;

            // 수채화 동적 할당
            drawPrefab = Resources.Load<GameObject>("YS/WaterPaint");
        }

        // 블렌딩 모드
        if (toolNum == 2 && b_eraser == false)
        {
            // 블렌딩 색 받아오기
            drawPrefab.GetComponent<LineRenderer>().startColor = spuit;
            drawPrefab.GetComponent<LineRenderer>().endColor = spuit;
        }

        //if(GameManager_BH.instance.state == GameManager_BH.gameState.Playing)
        //{
        // 그림 그리기
        Draw();
        //}

        // 사이즈 조절
        Size();

        // 지우개
        Eraser();

        // 그림 이동
        Moving();

        // 실행취소(뒤로)
        CtrZ();

        // 되돌리기(앞으로)
        CtrY();

        // 전부 지우기
        AllClear();

        // 스포이트
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
        {
            StartCoroutine(Spuit());
        }

        // 레이어
        Layer();
    }

    void Draw()
    {
        Color eraser = Color.white;

        Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        //Vector2 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(mouseRay, out hit))
            {
                if (hit.transform.name == drawCanvas.transform.name)
                {
                    // 캔버스 위에서 그리고 있음!
                    b_onCanvas = true;

                    // 브러쉬일 때, 마커일 때, 연필일 때, 캘리그라피일 때, 크레용일 때, 스프레이일 때, 유화일 때, 수채화일 때
                    if(toolNum == 1 || toolNum == 3 || toolNum == 4 || toolNum == 5 || toolNum == 6 || toolNum == 7 || toolNum == 8 || toolNum == 9)
                    {
                        // 선을 그리기 전, 사이즈 설정
                        drawPrefab.GetComponent<LineRenderer>().widthMultiplier = size;
                        if(toolNum == 4) // 연필은 사이즈가 다른 도구들에 비해 작음!
                        {
                            drawPrefab.GetComponent<LineRenderer>().widthMultiplier = size - 0.04f;
                        }
                        // 선을 그리기 전, 색 설정
                        drawPrefab.GetComponent<LineRenderer>().startColor = colorObject.GetComponent<ColorPickerTest>().selectedColor;
                        drawPrefab.GetComponent<LineRenderer>().endColor = colorObject.GetComponent<ColorPickerTest>().selectedColor;
                    }

                    // 지우개
                    if (b_eraser == true)
                    {
                        drawPrefab.GetComponent<LineRenderer>().startColor = eraser;
                        drawPrefab.GetComponent<LineRenderer>().endColor = eraser;
                    }

                    // 나중에 생긴 선은 위에 올라오게끔
                    sortingOrderNum++;
                    drawPrefab.GetComponent<LineRenderer>().sortingOrder = sortingOrderNum;

                    // 선 생성
                    theTrail = (GameObject)Instantiate(drawPrefab, Vector3.zero, Quaternion.identity);
                    theTrail.transform.SetParent(drawCanvas_parent.transform, false);
                    // 만약 생성될 때, 리스트에 active가 false인 것들은 삭제
                    for (int i = 0; i < lines.Count; i++)
                    {
                        if (lines[i].activeSelf == false)
                        {
                            Destroy(lines[i].gameObject); // 데이터 관리(쓸모없는 것은 지우기)
                            lines.RemoveAt(i);
                            i--;
                        }
                    }
                    // 생성 후, 리스트에 넣어주기
                    lines.Add(theTrail);

                    if (Physics.Raycast(mouseRay, out hit))
                    {
                        //startPos = mouseRay.GetPoint(_dis);
                        //startPos.z = drawCanvas.transform.position.z;
                        startPos = hit.point;

                        theTrail.GetComponent<LineRenderer>().SetPosition(0, startPos);
                        theTrail.GetComponent<LineRenderer>().SetPosition(1, startPos);
                    }
                }
                else
                {
                    // 캔버스 위에서 그리고 있지 않음!
                    b_onCanvas = false;
                }
            }
        }
        else if (Input.GetMouseButton(0))
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(mouseRay, out hit))
            {
                if (hit.transform.name == drawCanvas.transform.name)
                {
                    // 선이 캔버스를 나갔다가 들어왔을 때, 들어오는 그 자리부터 다시 생성
                    if(b_onCanvas == false)
                    {
                        // 선 생성
                        theTrail = (GameObject)Instantiate(drawPrefab, Vector3.zero, Quaternion.identity);
                        theTrail.transform.SetParent(drawCanvas_parent.transform, false);
                        // 만약 생성될 때, 리스트에 active가 false인 것들은 삭제
                        for (int i = 0; i < lines.Count; i++)
                        {
                            if (lines[i].activeSelf == false)
                            {
                                Destroy(lines[i].gameObject); // 데이터 관리(쓸모없는 것은 지우기)
                                lines.RemoveAt(i);
                                i--;
                            }
                        }
                        // 생성 후, 리스트에 넣어주기
                        lines.Add(theTrail);

                        if (Physics.Raycast(mouseRay, out hit))
                        {
                            //startPos = mouseRay.GetPoint(_dis);
                            //startPos.z = drawCanvas.transform.position.z;
                            startPos = hit.point;

                            theTrail.GetComponent<LineRenderer>().SetPosition(0, startPos);
                            theTrail.GetComponent<LineRenderer>().SetPosition(1, startPos);
                        }
                    }

                    // 캔버스 위에서 그리고 있음!
                    b_onCanvas = true;

                    // 브러쉬 일 때, 마커일 때, 연필일 때, 캘리그라피일 때, 크레용일 때, 스프레이일 때, 유화일 때, 수채화일 때
                    if (toolNum == 1 || toolNum == 3 || toolNum == 4 || toolNum == 5 || toolNum == 6 || toolNum == 7 || toolNum == 8 || toolNum == 9)
                    {
                        //nextPos = mouseRay.GetPoint(_dis);
                        //nextPos.z = drawCanvas.transform.position.z;
                        nextPos = hit.point;

                        theTrail.GetComponent<LineRenderer>().positionCount++;
                        int positionIndex = theTrail.GetComponent<LineRenderer>().positionCount - 1;
                        theTrail.GetComponent<LineRenderer>().SetPosition(positionIndex, nextPos);
                    }
                    // 블렌딩 모드 일 때
                    else if(toolNum == 2)
                    {
                        nextPos = hit.point;

                        theTrail.GetComponent<LineRenderer>().positionCount++;
                        int positionIndex = theTrail.GetComponent<LineRenderer>().positionCount - 1;
                        theTrail.GetComponent<LineRenderer>().SetPosition(positionIndex, nextPos);

                        if(theTrail.GetComponent<LineRenderer>().startColor != spuit)
                        {
                            // 선 생성
                            theTrail = (GameObject)Instantiate(drawPrefab, Vector3.zero, Quaternion.identity);
                            theTrail.transform.SetParent(drawCanvas_parent.transform, false);
                            // 만약 생성될 때, 리스트에 active가 false인 것들은 삭제
                            for (int i = 0; i < lines.Count; i++)
                            {
                                if (lines[i].activeSelf == false)
                                {
                                    Destroy(lines[i].gameObject); // 데이터 관리(쓸모없는 것은 지우기)
                                    lines.RemoveAt(i);
                                    i--;
                                }
                            }
                            // 생성 후, 리스트에 넣어주기
                            lines.Add(theTrail);

                            if (Physics.Raycast(mouseRay, out hit))
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
                else
                {
                    // 캔버스 위에서 그리고 있지 않음!
                    b_onCanvas = false;
                }
            }
        }
    }

    void Size()
    {
        if(Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            size += 0.01f;
        }
        else if(Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            size -= 0.01f;
        }
    }

    void Eraser()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            if(b_eraser == false)
            {
                b_eraser = true;
                // 지우개 동적 할당
                drawPrefab_temp = drawPrefab;
                drawPrefab = Resources.Load<GameObject>("YS/Eraser");
            }
            else if(b_eraser == true)
            {
                b_eraser = false;
                // 지우개를 사용하기 전 도구로
                drawPrefab = drawPrefab_temp;
            }
        }
    }

    void Moving()
    {
        /*// canvasPos와 drawCanvas의 위치를 비교해서 drawCanvas가 움직였는지 판단
        if (drawCanvas.transform.position != canvasPos)
        {
            dis = drawCanvas.transform.position - canvasPos;

            for (int i = 0; i < lines.Count; i++)
            {
                for (int j = 0; j < lines[i].GetComponent<LineRenderer>().positionCount; j++)
                {
                    Vector3 drawPos = lines[i].GetComponent<LineRenderer>().GetPosition(j);

                    lines[i].GetComponent<LineRenderer>().SetPosition(j, drawPos + dis);
                }
            }
        }

        canvasPos = drawCanvas.transform.position; // 이전 drawCanva의 위치를 넣어준다.*/
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

    IEnumerator Spuit()
    {
        // ReadPixels함수는 현재 프레임 버퍼에서 픽셀들을 불러오기 때문에, 해당 프레임의 렌더링이 완전히 끝난 뒤 실행되어야 한다.
        // (즉, ReadPixels함수 보다 먼저 WaitForEndOfFrame이 실행되어야 한다.)
        yield return new WaitForEndOfFrame();
        
        // 스크린 샷 찍기
        Texture2D screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
        screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenShot.Apply();

        spuit = screenShot.GetPixel((int)Input.mousePosition.x, (int)Input.mousePosition.y);

        // 메모리 삭제(안해주면 과부하)
        Destroy(screenShot);

        /*Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(mouseRay, out hit))
        {
            if(hit.transform.GetComponent<LineRenderer>())
            {
                spuit = hit.transform.GetComponent<LineRenderer>().startColor;
            }
            else
            {
                spuit = hit.transform.GetComponent<Renderer>().material.GetColor("_Color");
            }
        }*/
    }

    void Layer()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            layerNum++;

            if(layerNum > 2)
            {
                layerNum = 0;
            }
        }

        drawPrefab.GetComponent<LineRenderer>().sortingLayerName = layerName[layerNum];
    }
}
