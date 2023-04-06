using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BrushTest_BH : MonoBehaviourPun
{
    // 도구 번호
    public int toolNum = 1;

    public GameObject drawPrefab;
    public GameObject drawCanvas, drawCanvas_parent;
    GameObject theTrail;
    Vector3 startPos;
    Vector3 nextPos;

    // 사이즈
    public float size = 0.05f;
    // 색 설정
    public GameObject colorObject;

    // 지우개
    public bool b_eraser;
    public GameObject drawPrefab_temp;

    // 모든 선 저장
    //List<GameObject> lines = new List<GameObject>();

    // 스포이트
    public Color spuit;

    // 선을 캔버스 위에서 그리고 있는지 판단(특히 그리다가 삐져나갈 경우)
    bool b_onCanvas;

    // 레이어
    public string[] layerName = new string[] { "Layer1", "Layer2", "Layer3" };
    public int layerNum = 0;

    // 사운드
    AudioSource sound;

    // 네트워크
    public int myCanvasIdx;
    int sortingOrder;
    public string drawPrefabName; // 네트워크로 넘겨줄 현재 drawPrefab의 이름
    public List<List<GameObject>> lines = new List<List<GameObject>>(); //네트워크에서 사용할 2차원 리스트 (모든 선 저장)

    // Start is called before the first frame update
    void Start()
    {
        if(photonView.IsMine)
        {
            if(CollaborateModeManager_BH.instance)
            {
                drawCanvas = CompeteModeManager_BH.instance.playerCanvas[0].GetComponentsInChildren<Transform>()[1].gameObject;
                drawCanvas_parent = CompeteModeManager_BH.instance.playerCanvas[0].GetComponent<Transform>().gameObject;
                //planObj = new Plane(Camera.main.transform.forward, drawCanvas.transform.position);
                colorObject = GameObject.Find("ColorPicker");
                colorObject.SetActive(false);
                myCanvasIdx = 0;
            }
            else if(CompeteModeManager_BH.instance)
            {
                myCanvasIdx = PhotonNetwork.CurrentRoom.PlayerCount - 1;
                drawCanvas = CompeteModeManager_BH.instance.playerCanvas[myCanvasIdx].GetComponentsInChildren<Transform>()[1].gameObject;
                drawCanvas_parent = CompeteModeManager_BH.instance.playerCanvas[myCanvasIdx].GetComponent<Transform>().gameObject;
                //planObj = new Plane(Camera.main.transform.forward, drawCanvas.transform.position);
                colorObject = GameObject.Find("ColorPicker");
                colorObject.SetActive(false);

                //sortingOrder = drawPrefab.GetComponent<LineRenderer>().sortingOrder;
            }
        }

        drawPrefab = Resources.Load<GameObject>("YS/Brush");
        drawPrefabName = "YS/Brush";
        drawPrefab.GetComponent<LineRenderer>().useWorldSpace = false;
        drawPrefab.GetComponent<LineRenderer>().sortingLayerName = layerName[layerNum];

        // 2차원 리스트 크기 지정
        for(int i = 0; i < 8; i++)
        {
            lines.Add(new List<GameObject>());
        }

        // 사운드
        sound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            /*if (GameManager_BH.instance.state == GameManager_BH.gameState.Playing)
            {
                // 그림 그리기
                Draw();

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
            }*/

            // 도구 (1번은 브러쉬, 2번은 블렌딩, 3번은 마커, 4번은 연필, 5번은 캘리그라피, 6번은 크레용, 7번은 스프레이, 8번은 유화, 9번은 수채화)
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                toolNum = 1;

                // 브러쉬 동적 할당
                drawPrefab = Resources.Load<GameObject>("YS/Brush");
                drawPrefabName = "YS/Brush";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                toolNum = 2;

                // 블렌딩 동적 할당
                drawPrefab = Resources.Load<GameObject>("YS/Blending");
                drawPrefabName = "YS/Blending";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                toolNum = 3;

                // 마커 동적 할당
                drawPrefab = Resources.Load<GameObject>("YS/Marker");
                drawPrefabName = "YS/Marker";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                toolNum = 4;

                // 연필 동적 할당
                drawPrefab = Resources.Load<GameObject>("YS/Pencil");
                drawPrefabName = "YS/Pencil";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                toolNum = 5;

                // 캘리그라피 동적 할당
                drawPrefab = Resources.Load<GameObject>("YS/Calligraphy");
                drawPrefabName = "YS/Calligraphy";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                toolNum = 6;

                // 크레용 동적 할당
                drawPrefab = Resources.Load<GameObject>("YS/Crayon");
                drawPrefabName = "YS/Crayon";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                toolNum = 7;

                // 스프레이 동적 할당
                drawPrefab = Resources.Load<GameObject>("YS/Spray");
                drawPrefabName = "YS/Spray";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                toolNum = 8;

                // 유화 동적 할당
                drawPrefab = Resources.Load<GameObject>("YS/OilPaint");
                drawPrefabName = "YS/OilPaint";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                toolNum = 9;

                // 수채화 동적 할당
                drawPrefab = Resources.Load<GameObject>("YS/WaterPaint");
                drawPrefabName = "YS/WaterPaint";
            }

            // 블렌딩 모드
            if (toolNum == 2 && b_eraser == false)
            {
                // 스포이트
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
                {
                    StartCoroutine(Spuit());
                }

                // 블렌딩 색 받아오기
                drawPrefab.GetComponent<LineRenderer>().startColor = spuit;
                drawPrefab.GetComponent<LineRenderer>().endColor = spuit;
            }

            // 그림 그리기
            Draw();

            // 사이즈 조절
            Size();

            // 지우개
            Eraser();

            // 실행취소(뒤로)
            CtrZ();

            // 되돌리기(앞으로)
            CtrY();

            // 전부 지우기
            AllClear();

            // 스포이트
            /*if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
            {
                StartCoroutine(Spuit());
            }*/

            // 레이어
            Layer();
        }
    }
    void Draw()
    {
        Color eraser = Color.white;

        Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        if (Input.GetMouseButtonDown(0))
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(mouseRay, out hit))
            {
                if (hit.transform.name == drawCanvas.transform.name)
                {
                    // 사운드
                    sound.Play();

                    // 캔버스 위에서 그리고 있음!
                    b_onCanvas = true;

                    // 브러쉬일 때, 마커일 때, 연필일 때, 캘리그라피일 때, 크레용일 때, 스프레이일 때, 유화일 때, 수채화일 때
                    if (toolNum == 1 || toolNum == 3 || toolNum == 4 || toolNum == 5 || toolNum == 6 || toolNum == 7 || toolNum == 8 || toolNum == 9)
                    {
                        // 선을 그리기 전, 사이즈 설정
                        drawPrefab.GetComponent<LineRenderer>().widthMultiplier = size;
                        if (toolNum == 4) // 연필은 사이즈가 다른 도구들에 비해 작음!
                        {
                            // 선 굵기 제한
                            if (size <= 0.04)
                            {
                                drawPrefab.GetComponent<LineRenderer>().widthMultiplier = 0.003f;
                            }
                            else
                            {
                                drawPrefab.GetComponent<LineRenderer>().widthMultiplier = size - 0.04f;
                            }
                        }
                        // 선을 그리기 전, 색 설정
                        drawPrefab.GetComponent<LineRenderer>().startColor = colorObject.GetComponent<ColorPickerTest>().selectedColor;
                        drawPrefab.GetComponent<LineRenderer>().endColor = colorObject.GetComponent<ColorPickerTest>().selectedColor;
                    }

                    /*// 선을 그리기 전, 사이즈 설정
                    theTrail.GetComponent<LineRenderer>().widthMultiplier = size;
                    // 선을 그리기 전, 색 설정
                    theTrail.GetComponent<LineRenderer>().startColor = colorObject.GetComponent<ColorPickerTest>().selectedColor;
                    theTrail.GetComponent<LineRenderer>().endColor = colorObject.GetComponent<ColorPickerTest>().selectedColor;*/
                    Color color = colorObject.GetComponent<ColorPickerTest>().selectedColor;

                    // 지우개
                    if (b_eraser == true)
                    {
                        drawPrefab.GetComponent<LineRenderer>().startColor = eraser;
                        drawPrefab.GetComponent<LineRenderer>().endColor = eraser;
                        color = eraser;
                    }

                    // 나중에 생긴 선은 위에 올라오게끔
                    sortingOrder++;
                    drawPrefab.GetComponent<LineRenderer>().sortingOrder = sortingOrder;

                    // 선 생성
                    theTrail = Instantiate(drawPrefab, Vector3.zero, Quaternion.identity);
                    theTrail.transform.SetParent(drawCanvas_parent.transform, false);
                    theTrail.transform.localEulerAngles = -theTrail.transform.parent.localEulerAngles;

                    // 만약 생성될 때, 리스트에 active가 false인 것들은 삭제
                    for (int i = 0; i < lines[myCanvasIdx].Count; i++)
                    {
                        if (lines[myCanvasIdx][i].activeSelf == false)
                        {
                            Destroy(lines[myCanvasIdx][i].gameObject); // 데이터 관리(쓸모없는 것은 지우기)
                            lines[myCanvasIdx].RemoveAt(i);
                            i--;
                        }
                    }
                    // 생성 후, 리스트에 넣어주기
                    lines[myCanvasIdx].Add(theTrail);

                    if (Physics.Raycast(mouseRay, out hit))
                    {
                        startPos = hit.point - theTrail.transform.parent.position;

                        theTrail.GetComponent<LineRenderer>().SetPosition(0, startPos);
                        theTrail.GetComponent<LineRenderer>().SetPosition(1, startPos);
                    }

                    // 원격 프로시저 호출
                    photonView.RPC("RpcDrawStart", RpcTarget.OthersBuffered, size, color.r, color.g, color.b, sortingOrder, myCanvasIdx, startPos, drawPrefabName);
                    //sortingOrder++;
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
                    if (b_onCanvas == false)
                    {
                        // 사운드
                        sound.Play();

                        // 선 생성
                        theTrail =Instantiate(drawPrefab, Vector3.zero, Quaternion.identity);
                        theTrail.transform.SetParent(drawCanvas_parent.transform, false);
                        theTrail.transform.localEulerAngles = -theTrail.transform.parent.localEulerAngles;

                        Color color = colorObject.GetComponent<ColorPickerTest>().selectedColor;

                        // 지우개
                        if (b_eraser == true)
                        {
                            drawPrefab.GetComponent<LineRenderer>().startColor = eraser;
                            drawPrefab.GetComponent<LineRenderer>().endColor = eraser;
                            color = eraser;
                        }

                        // 만약 생성될 때, 리스트에 active가 false인 것들은 삭제
                        for (int i = 0; i < lines[myCanvasIdx].Count; i++)
                        {
                            if (lines[myCanvasIdx][i].activeSelf == false)
                            {
                                Destroy(lines[myCanvasIdx][i].gameObject); // 데이터 관리(쓸모없는 것은 지우기)
                                lines[myCanvasIdx].RemoveAt(i);
                                i--;
                            }
                        }
                        // 생성 후, 리스트에 넣어주기
                        lines[myCanvasIdx].Add(theTrail);

                        if (Physics.Raycast(mouseRay, out hit))
                        {
                            /*nextPos = hit.point - theTrail.transform.parent.position;

                            theTrail.GetComponent<LineRenderer>().positionCount++;
                            int positionIndex = theTrail.GetComponent<LineRenderer>().positionCount - 1;
                            theTrail.GetComponent<LineRenderer>().SetPosition(positionIndex, nextPos);*/

                            startPos = hit.point - theTrail.transform.parent.position;

                            theTrail.GetComponent<LineRenderer>().SetPosition(0, startPos);
                            theTrail.GetComponent<LineRenderer>().SetPosition(1, startPos);

                            // 네트워크(내가 그리는 것이 다른 사람들한테 보이게끔만 하는 함수 따로 실행)
                            photonView.RPC("RpcDrawStart", RpcTarget.OthersBuffered, size, color.r, color.g, color.b, sortingOrder, myCanvasIdx, startPos, drawPrefabName);
                        }
                    }

                    // 캔버스 위에서 그리고 있음!
                    b_onCanvas = true;

                    // 브러쉬 일 때, 마커일 때, 연필일 때, 캘리그라피일 때, 크레용일 때, 스프레이일 때, 유화일 때, 수채화일 때
                    if (toolNum == 1 || toolNum == 3 || toolNum == 4 || toolNum == 5 || toolNum == 6 || toolNum == 7 || toolNum == 8 || toolNum == 9)
                    {
                        //nextPos = mouseRay.GetPoint(_dis);
                        //nextPos.z = drawCanvas.transform.position.z;
                        nextPos = hit.point - theTrail.transform.parent.position;

                        theTrail.GetComponent<LineRenderer>().positionCount++;
                        int positionIndex = theTrail.GetComponent<LineRenderer>().positionCount - 1;
                        theTrail.GetComponent<LineRenderer>().SetPosition(positionIndex, nextPos);

                        // 네트워크(내가 그리는 것이 다른 사람들한테 보이게끔만 하는 함수 따로 실행)
                        photonView.RPC("RpcDrawing", RpcTarget.OthersBuffered, theTrail.GetComponent<LineRenderer>().positionCount, positionIndex, nextPos);
                    }
                    // 블렌딩 모드 일 때
                    else if (toolNum == 2)
                    {
                        nextPos = hit.point - theTrail.transform.parent.position;

                        theTrail.GetComponent<LineRenderer>().positionCount++;
                        int positionIndex = theTrail.GetComponent<LineRenderer>().positionCount - 1;
                        theTrail.GetComponent<LineRenderer>().SetPosition(positionIndex, nextPos);

                        // 네트워크(내가 그리는 것이 다른 사람들한테 보이게끔만 하는 함수 따로 실행)
                        photonView.RPC("RpcDrawing", RpcTarget.OthersBuffered, theTrail.GetComponent<LineRenderer>().positionCount, positionIndex, nextPos);

                        if (theTrail.GetComponent<LineRenderer>().startColor != spuit)
                        {
                            // 선 생성
                            theTrail = (GameObject)Instantiate(drawPrefab, Vector3.zero, Quaternion.identity);
                            theTrail.transform.SetParent(drawCanvas_parent.transform, false);
                            // 만약 생성될 때, 리스트에 active가 false인 것들은 삭제
                            for (int i = 0; i < lines[myCanvasIdx].Count; i++)
                            {
                                if (lines[myCanvasIdx][i].activeSelf == false)
                                {
                                    Destroy(lines[myCanvasIdx][i].gameObject); // 데이터 관리(쓸모없는 것은 지우기)
                                    lines[myCanvasIdx].RemoveAt(i);
                                    i--;
                                }
                            }
                            // 생성 후, 리스트에 넣어주기
                            lines[myCanvasIdx].Add(theTrail);

                            if (Physics.Raycast(mouseRay, out hit))
                            {
                                //startPos = mouseRay.GetPoint(_dis);
                                //startPos.z = drawCanvas.transform.position.z;
                                startPos = hit.point - theTrail.transform.parent.position;

                                theTrail.GetComponent<LineRenderer>().SetPosition(0, startPos);
                                theTrail.GetComponent<LineRenderer>().SetPosition(1, startPos);

                                // 네트워크(내가 그리는 것이 다른 사람들한테 보이게끔만 하는 함수 따로 실행)
                                Color color = drawPrefab.GetComponent<LineRenderer>().startColor;
                                photonView.RPC("RpcDrawStart", RpcTarget.OthersBuffered, size, color.r, color.g, color.b, sortingOrder, myCanvasIdx, startPos, drawPrefabName);
                            }
                        }
                    }
                }
                else
                {
                    // 캔버스 위에서 그리고 있지 않음!
                    b_onCanvas = false;

                    // 사운드
                    sound.Stop();
                }
            }
        }
        else if(Input.GetMouseButtonUp(0))
        {
            // 사운드
            sound.Stop();
        }
    }

    [PunRPC]
    void RpcDrawStart(float _size, float r, float g, float b, int _sortingOrder, int _myCanvasIdx, Vector3 _startPos, string _drawPrefabName)
    {
        drawPrefab = Resources.Load<GameObject>(_drawPrefabName);
        drawCanvas = CompeteModeManager_BH.instance.playerCanvas[_myCanvasIdx].GetComponentsInChildren<Transform>()[1].gameObject;
        drawCanvas_parent = CompeteModeManager_BH.instance.playerCanvas[_myCanvasIdx].GetComponent<Transform>().gameObject;

        Color color = new Color(r, g, b);

        theTrail = Instantiate(drawPrefab, Vector3.zero, Quaternion.identity);
        theTrail.transform.SetParent(drawCanvas_parent.transform, false);
        theTrail.transform.localEulerAngles = -theTrail.transform.parent.localEulerAngles;

        LineRenderer lr = theTrail.GetComponent<LineRenderer>();
        ColorPickerTest cp = theTrail.GetComponent<ColorPickerTest>();
        
        // 선을 그리기 전, 사이즈 설정
        theTrail.GetComponent<LineRenderer>().widthMultiplier = _size;
        
        // 선을 그리기 전, 색 설정
        theTrail.GetComponent<LineRenderer>().startColor = color;
        theTrail.GetComponent<LineRenderer>().endColor = color;
       
        // 나중에 생긴 선은 위에 올라오게끔
        theTrail.GetComponent<LineRenderer>().sortingOrder = _sortingOrder;

        // 만약 생성될 때, 리스트에 active가 false인 것들은 삭제
        for (int i = 0; i < lines[_myCanvasIdx].Count; i++)
        {
            if (lines[_myCanvasIdx][i].activeSelf == false)
            {
                Destroy(lines[_myCanvasIdx][i].gameObject); // 데이터 관리(쓸모없는 것은 지우기)
                lines[_myCanvasIdx].RemoveAt(i);
                i--;
            }
        }

        // 생성 후, 리스트에 넣어주기
        lines[_myCanvasIdx].Add(theTrail);
        theTrail.GetComponent<LineRenderer>().SetPosition(0, _startPos);
        theTrail.GetComponent<LineRenderer>().SetPosition(1, _startPos);      
    }

    [PunRPC]
    void RpcDrawing(int _positionCount, int _positionIndex, Vector3 _nextPos)
    {
        theTrail.GetComponent<LineRenderer>().positionCount = _positionCount;
        theTrail.GetComponent<LineRenderer>().SetPosition(_positionIndex, _nextPos);
    }

    void Size()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            size += 0.01f;
        }
        else if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            size -= 0.01f;
        }
    }

    void Eraser()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (b_eraser == false)
            {
                b_eraser = true;
                // 지우개 동적 할당
                drawPrefab_temp = drawPrefab;
                drawPrefab = Resources.Load<GameObject>("YS/Eraser");
                drawPrefabName = "YS/Eraser";
            }
            else if (b_eraser == true)
            {
                b_eraser = false;
                // 지우개를 사용하기 전 도구로
                drawPrefab = drawPrefab_temp;
            }
        }
    }

    void CtrZ()
    {
        if(Input.GetKeyDown(KeyCode.Home))
        {
            for(int i = lines[myCanvasIdx].Count - 1; i >= 0; i--)
            {
                if(lines[myCanvasIdx][i].activeSelf == true)
                {
                    lines[myCanvasIdx][i].SetActive(false);
                    break;
                }
            }

            // 네트워크 (다른 사람들한테도 적용)
            photonView.RPC("RpcCtrZ", RpcTarget.OthersBuffered, myCanvasIdx);
        }
    }

    [PunRPC]
    void RpcCtrZ(int _myCanvasIdx)
    {
        for (int i = lines[_myCanvasIdx].Count - 1; i >= 0; i--)
        {
            if (lines[_myCanvasIdx][i].activeSelf == true)
            {
                lines[_myCanvasIdx][i].SetActive(false);
                break;
            }
        }
    }

    void CtrY()
    {
        if (Input.GetKeyDown(KeyCode.End))
        {
            for (int i = 0; i < lines[myCanvasIdx].Count; i++)
            {
                if (lines[myCanvasIdx][i].activeSelf == false)
                {
                    lines[myCanvasIdx][i].SetActive(true);
                    break;
                }
            }

            // 네트워크 (다른 사람들한테도 적용)
            photonView.RPC("RpcCtrY", RpcTarget.OthersBuffered, myCanvasIdx);
        }
    }

    [PunRPC]
    void RpcCtrY(int _myCanvasIdx)
    {
        for (int i = 0; i < lines[_myCanvasIdx].Count; i++)
        {
            if (lines[_myCanvasIdx][i].activeSelf == false)
            {
                lines[_myCanvasIdx][i].SetActive(true);
                break;
            }
        }
    }

    void AllClear()
    {
        if(Input.GetKeyDown(KeyCode.Backspace))
        {
            for(int i = 0; i < lines[myCanvasIdx].Count; i++)
            {
                Destroy(lines[myCanvasIdx][i].gameObject);
            }
            lines[myCanvasIdx].Clear();

            // 네트워크 (다른 사람들한테도 적용)
            photonView.RPC("RcpAllClear", RpcTarget.OthersBuffered, myCanvasIdx);
        }
    }

    [PunRPC]
    void RcpAllClear(int _myCanvasIdx)
    {
        for (int i = 0; i < lines[_myCanvasIdx].Count; i++)
        {
            Destroy(lines[_myCanvasIdx][i].gameObject);
        }
        lines[_myCanvasIdx].Clear();
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
    }

    void Layer()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            layerNum++;

            if (layerNum > 2)
            {
                layerNum = 0;
            }

            // 네트워크 (다른 사람들한테도 적용)
            photonView.RPC("RpcLayer", RpcTarget.OthersBuffered, drawPrefabName, layerNum);
        }

        drawPrefab.GetComponent<LineRenderer>().sortingLayerName = layerName[layerNum];
    }

    [PunRPC]
    void RpcLayer(string _drawPrefabName, int _layerNum)
    {
        drawPrefab = Resources.Load<GameObject>(_drawPrefabName);

        drawPrefab.GetComponent<LineRenderer>().sortingLayerName = layerName[_layerNum];
    }
}
