using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BrushNet_YS : MonoBehaviourPun
{
    // ���� ��ȣ
    public int toolNum = 1;

    public GameObject drawPrefab;
    public GameObject drawCanvas, drawCanvas_parent;
    GameObject theTrail;
    Vector3 startPos;
    Vector3 nextPos;

    // ������
    public float size = 0.05f;
    // �� ����
    public GameObject colorObject;

    // ���찳
    public bool b_eraser;
    public GameObject drawPrefab_temp;

    // ��� �� ����
    //List<GameObject> lines = new List<GameObject>();

    // ������Ʈ
    public Color spuit;

    // ���� ĵ���� ������ �׸��� �ִ��� �Ǵ�(Ư�� �׸��ٰ� �������� ���)
    bool b_onCanvas;

    // ���̾�
    public string[] layerName = new string[] { "Layer1", "Layer2", "Layer3" };
    int layerNum = 0;

    // ����
    AudioSource sound;

    // ��Ʈ��ũ
    public int myCanvasIdx;
    int sortingOrder;
    public string drawPrefabName; // ��Ʈ��ũ�� �Ѱ��� ���� drawPrefab�� �̸�
    public List<List<GameObject>> lines = new List<List<GameObject>>(); //��Ʈ��ũ���� ����� 2���� ����Ʈ (��� �� ����)

    // Start is called before the first frame update
    void Start()
    {
        if(photonView.IsMine)
        {
            myCanvasIdx = PhotonNetwork.CurrentRoom.PlayerCount - 1;
            drawCanvas = CompeteModeManager_BH.instance.playerCanvas[myCanvasIdx].GetComponentsInChildren<Transform>()[1].gameObject;
            drawCanvas_parent = CompeteModeManager_BH.instance.playerCanvas[myCanvasIdx].GetComponent<Transform>().gameObject;
            //planObj = new Plane(Camera.main.transform.forward, drawCanvas.transform.position);
            colorObject = GameObject.Find("ColorPicker");
            colorObject.SetActive(false);

            //sortingOrder = drawPrefab.GetComponent<LineRenderer>().sortingOrder;
        }

        drawPrefab = Resources.Load<GameObject>("YS/Brush");
        drawPrefabName = "YS/Brush";
        drawPrefab.GetComponent<LineRenderer>().useWorldSpace = false;
        drawPrefab.GetComponent<LineRenderer>().sortingLayerName = layerName[layerNum];

        // 2���� ����Ʈ ũ�� ����
        for(int i = 0; i < 8; i++)
        {
            lines.Add(new List<GameObject>());
        }

        // ����
        sound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            /*if (GameManager_BH.instance.state == GameManager_BH.gameState.Playing)
            {
                // �׸� �׸���
                Draw();

                // ������ ����
                Size();

                // ���찳
                Eraser();

                // �׸� �̵�
                Moving();

                // �������(�ڷ�)
                CtrZ();

                // �ǵ�����(������)
                CtrY();

                // ���� �����
                AllClear();
            }*/

            // ���� (1���� �귯��, 2���� ����, 3���� ��Ŀ, 4���� ����, 5���� Ķ���׶���, 6���� ũ����, 7���� ��������, 8���� ��ȭ, 9���� ��äȭ)
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                toolNum = 1;

                // �귯�� ���� �Ҵ�
                drawPrefab = Resources.Load<GameObject>("YS/Brush");
                drawPrefabName = "YS/Brush";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                toolNum = 2;

                // ���� ���� �Ҵ�
                drawPrefab = Resources.Load<GameObject>("YS/Blending");
                drawPrefabName = "YS/Blending";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                toolNum = 3;

                // ��Ŀ ���� �Ҵ�
                drawPrefab = Resources.Load<GameObject>("YS/Marker");
                drawPrefabName = "YS/Marker";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                toolNum = 4;

                // ���� ���� �Ҵ�
                drawPrefab = Resources.Load<GameObject>("YS/Pencil");
                drawPrefabName = "YS/Pencil";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                toolNum = 5;

                // Ķ���׶��� ���� �Ҵ�
                drawPrefab = Resources.Load<GameObject>("YS/Calligraphy");
                drawPrefabName = "YS/Calligraphy";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                toolNum = 6;

                // ũ���� ���� �Ҵ�
                drawPrefab = Resources.Load<GameObject>("YS/Crayon");
                drawPrefabName = "YS/Crayon";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                toolNum = 7;

                // �������� ���� �Ҵ�
                drawPrefab = Resources.Load<GameObject>("YS/Spray");
                drawPrefabName = "YS/Spray";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                toolNum = 8;

                // ��ȭ ���� �Ҵ�
                drawPrefab = Resources.Load<GameObject>("YS/OilPaint");
                drawPrefabName = "YS/OilPaint";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                toolNum = 9;

                // ��äȭ ���� �Ҵ�
                drawPrefab = Resources.Load<GameObject>("YS/WaterPaint");
                drawPrefabName = "YS/WaterPaint";
            }

            // ���� ���
            if (toolNum == 2 && b_eraser == false)
            {
                // ������Ʈ
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
                {
                    StartCoroutine(Spuit());
                }

                // ���� �� �޾ƿ���
                drawPrefab.GetComponent<LineRenderer>().startColor = spuit;
                drawPrefab.GetComponent<LineRenderer>().endColor = spuit;
            }

            // �׸� �׸���
            Draw();

            // ������ ����
            Size();

            // ���찳
            Eraser();

            // �������(�ڷ�)
            CtrZ();

            // �ǵ�����(������)
            CtrY();

            // ���� �����
            AllClear();

            // ������Ʈ
            /*if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
            {
                StartCoroutine(Spuit());
            }*/

            // ���̾�
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
                    // ����
                    sound.Play();

                    // ĵ���� ������ �׸��� ����!
                    b_onCanvas = true;

                    // �귯���� ��, ��Ŀ�� ��, ������ ��, Ķ���׶����� ��, ũ������ ��, ���������� ��, ��ȭ�� ��, ��äȭ�� ��
                    if (toolNum == 1 || toolNum == 3 || toolNum == 4 || toolNum == 5 || toolNum == 6 || toolNum == 7 || toolNum == 8 || toolNum == 9)
                    {
                        // ���� �׸��� ��, ������ ����
                        drawPrefab.GetComponent<LineRenderer>().widthMultiplier = size;
                        if (toolNum == 4) // ������ ����� �ٸ� �����鿡 ���� ����!
                        {
                            // �� ���� ����
                            if (size <= 0.04)
                            {
                                drawPrefab.GetComponent<LineRenderer>().widthMultiplier = 0.003f;
                            }
                            else
                            {
                                drawPrefab.GetComponent<LineRenderer>().widthMultiplier = size - 0.04f;
                            }
                        }
                        // ���� �׸��� ��, �� ����
                        drawPrefab.GetComponent<LineRenderer>().startColor = colorObject.GetComponent<ColorPickerTest>().selectedColor;
                        drawPrefab.GetComponent<LineRenderer>().endColor = colorObject.GetComponent<ColorPickerTest>().selectedColor;
                    }

                    /*// ���� �׸��� ��, ������ ����
                    theTrail.GetComponent<LineRenderer>().widthMultiplier = size;
                    // ���� �׸��� ��, �� ����
                    theTrail.GetComponent<LineRenderer>().startColor = colorObject.GetComponent<ColorPickerTest>().selectedColor;
                    theTrail.GetComponent<LineRenderer>().endColor = colorObject.GetComponent<ColorPickerTest>().selectedColor;*/
                    Color color = colorObject.GetComponent<ColorPickerTest>().selectedColor;

                    // ���찳
                    if (b_eraser == true)
                    {
                        drawPrefab.GetComponent<LineRenderer>().startColor = eraser;
                        drawPrefab.GetComponent<LineRenderer>().endColor = eraser;
                        color = eraser;
                    }

                    // ���߿� ���� ���� ���� �ö���Բ�
                    sortingOrder++;
                    drawPrefab.GetComponent<LineRenderer>().sortingOrder = sortingOrder;

                    // �� ����
                    theTrail = Instantiate(drawPrefab, Vector3.zero, Quaternion.identity);
                    theTrail.transform.SetParent(drawCanvas_parent.transform, false);
                    theTrail.transform.localEulerAngles = -theTrail.transform.parent.localEulerAngles;

                    // ���� ������ ��, ����Ʈ�� active�� false�� �͵��� ����
                    for (int i = 0; i < lines[myCanvasIdx].Count; i++)
                    {
                        if (lines[myCanvasIdx][i].activeSelf == false)
                        {
                            Destroy(lines[myCanvasIdx][i].gameObject); // ������ ����(������� ���� �����)
                            lines[myCanvasIdx].RemoveAt(i);
                            i--;
                        }
                    }
                    // ���� ��, ����Ʈ�� �־��ֱ�
                    lines[myCanvasIdx].Add(theTrail);

                    if (Physics.Raycast(mouseRay, out hit))
                    {
                        startPos = hit.point - theTrail.transform.parent.position;

                        theTrail.GetComponent<LineRenderer>().SetPosition(0, startPos);
                        theTrail.GetComponent<LineRenderer>().SetPosition(1, startPos);
                    }

                    // ��Ʈ��ũ(���� �׸��� ���� �ٸ� ��������� ���̰Բ��� �ϴ� �Լ� ���� ����)
                    photonView.RPC("RpcDrawStart", RpcTarget.OthersBuffered, size, color.r, color.g, color.b, sortingOrder, myCanvasIdx, startPos, drawPrefabName);
                    //sortingOrder++;
                }
                else
                {
                    // ĵ���� ������ �׸��� ���� ����!
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
                    // ���� ĵ������ �����ٰ� ������ ��, ������ �� �ڸ����� �ٽ� ����
                    if (b_onCanvas == false)
                    {
                        // ����
                        sound.Play();

                        // �� ����
                        theTrail =Instantiate(drawPrefab, Vector3.zero, Quaternion.identity);
                        theTrail.transform.SetParent(drawCanvas_parent.transform, false);
                        theTrail.transform.localEulerAngles = -theTrail.transform.parent.localEulerAngles;

                        Color color = colorObject.GetComponent<ColorPickerTest>().selectedColor;

                        // ���찳
                        if (b_eraser == true)
                        {
                            drawPrefab.GetComponent<LineRenderer>().startColor = eraser;
                            drawPrefab.GetComponent<LineRenderer>().endColor = eraser;
                            color = eraser;
                        }

                        // ���� ������ ��, ����Ʈ�� active�� false�� �͵��� ����
                        for (int i = 0; i < lines[myCanvasIdx].Count; i++)
                        {
                            if (lines[myCanvasIdx][i].activeSelf == false)
                            {
                                Destroy(lines[myCanvasIdx][i].gameObject); // ������ ����(������� ���� �����)
                                lines[myCanvasIdx].RemoveAt(i);
                                i--;
                            }
                        }
                        // ���� ��, ����Ʈ�� �־��ֱ�
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

                            // ��Ʈ��ũ(���� �׸��� ���� �ٸ� ��������� ���̰Բ��� �ϴ� �Լ� ���� ����)
                            photonView.RPC("RpcDrawStart", RpcTarget.OthersBuffered, size, color.r, color.g, color.b, sortingOrder, myCanvasIdx, startPos, drawPrefabName);
                        }
                    }

                    // ĵ���� ������ �׸��� ����!
                    b_onCanvas = true;

                    // �귯�� �� ��, ��Ŀ�� ��, ������ ��, Ķ���׶����� ��, ũ������ ��, ���������� ��, ��ȭ�� ��, ��äȭ�� ��
                    if (toolNum == 1 || toolNum == 3 || toolNum == 4 || toolNum == 5 || toolNum == 6 || toolNum == 7 || toolNum == 8 || toolNum == 9)
                    {
                        //nextPos = mouseRay.GetPoint(_dis);
                        //nextPos.z = drawCanvas.transform.position.z;
                        nextPos = hit.point - theTrail.transform.parent.position;

                        theTrail.GetComponent<LineRenderer>().positionCount++;
                        int positionIndex = theTrail.GetComponent<LineRenderer>().positionCount - 1;
                        theTrail.GetComponent<LineRenderer>().SetPosition(positionIndex, nextPos);

                        // ��Ʈ��ũ(���� �׸��� ���� �ٸ� ��������� ���̰Բ��� �ϴ� �Լ� ���� ����)
                        photonView.RPC("RpcDrawing", RpcTarget.OthersBuffered, theTrail.GetComponent<LineRenderer>().positionCount, positionIndex, nextPos);
                    }
                    // ���� ��� �� ��
                    else if (toolNum == 2)
                    {
                        nextPos = hit.point - theTrail.transform.parent.position;

                        theTrail.GetComponent<LineRenderer>().positionCount++;
                        int positionIndex = theTrail.GetComponent<LineRenderer>().positionCount - 1;
                        theTrail.GetComponent<LineRenderer>().SetPosition(positionIndex, nextPos);

                        // ��Ʈ��ũ(���� �׸��� ���� �ٸ� ��������� ���̰Բ��� �ϴ� �Լ� ���� ����)
                        photonView.RPC("RpcDrawing", RpcTarget.OthersBuffered, theTrail.GetComponent<LineRenderer>().positionCount, positionIndex, nextPos);

                        if (theTrail.GetComponent<LineRenderer>().startColor != spuit)
                        {
                            // �� ����
                            theTrail = (GameObject)Instantiate(drawPrefab, Vector3.zero, Quaternion.identity);
                            theTrail.transform.SetParent(drawCanvas_parent.transform, false);
                            // ���� ������ ��, ����Ʈ�� active�� false�� �͵��� ����
                            for (int i = 0; i < lines[myCanvasIdx].Count; i++)
                            {
                                if (lines[myCanvasIdx][i].activeSelf == false)
                                {
                                    Destroy(lines[myCanvasIdx][i].gameObject); // ������ ����(������� ���� �����)
                                    lines[myCanvasIdx].RemoveAt(i);
                                    i--;
                                }
                            }
                            // ���� ��, ����Ʈ�� �־��ֱ�
                            lines[myCanvasIdx].Add(theTrail);

                            if (Physics.Raycast(mouseRay, out hit))
                            {
                                //startPos = mouseRay.GetPoint(_dis);
                                //startPos.z = drawCanvas.transform.position.z;
                                startPos = hit.point - theTrail.transform.parent.position;

                                theTrail.GetComponent<LineRenderer>().SetPosition(0, startPos);
                                theTrail.GetComponent<LineRenderer>().SetPosition(1, startPos);

                                // ��Ʈ��ũ(���� �׸��� ���� �ٸ� ��������� ���̰Բ��� �ϴ� �Լ� ���� ����)
                                Color color = drawPrefab.GetComponent<LineRenderer>().startColor;
                                photonView.RPC("RpcDrawStart", RpcTarget.OthersBuffered, size, color.r, color.g, color.b, sortingOrder, myCanvasIdx, startPos, drawPrefabName);
                            }
                        }
                    }
                }
                else
                {
                    // ĵ���� ������ �׸��� ���� ����!
                    b_onCanvas = false;

                    // ����
                    sound.Stop();
                }
            }
        }
        else if(Input.GetMouseButtonUp(0))
        {
            // ����
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
        // ���� �׸��� ��, ������ ����
        theTrail.GetComponent<LineRenderer>().widthMultiplier = _size;
        // ���� �׸��� ��, �� ����
        theTrail.GetComponent<LineRenderer>().startColor = color;
        theTrail.GetComponent<LineRenderer>().endColor = color;
       
        // ���߿� ���� ���� ���� �ö���Բ�
        theTrail.GetComponent<LineRenderer>().sortingOrder = _sortingOrder;

        // ���� ������ ��, ����Ʈ�� active�� false�� �͵��� ����
        for (int i = 0; i < lines[_myCanvasIdx].Count; i++)
        {
            if (lines[_myCanvasIdx][i].activeSelf == false)
            {
                Destroy(lines[_myCanvasIdx][i].gameObject); // ������ ����(������� ���� �����)
                lines[_myCanvasIdx].RemoveAt(i);
                i--;
            }
        }

        // ���� ��, ����Ʈ�� �־��ֱ�
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
                // ���찳 ���� �Ҵ�
                drawPrefab_temp = drawPrefab;
                drawPrefab = Resources.Load<GameObject>("YS/Eraser");
                drawPrefabName = "YS/Eraser";
            }
            else if (b_eraser == true)
            {
                b_eraser = false;
                // ���찳�� ����ϱ� �� ������
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

            // ��Ʈ��ũ (�ٸ� ��������׵� ����)
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

            // ��Ʈ��ũ (�ٸ� ��������׵� ����)
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

            // ��Ʈ��ũ (�ٸ� ��������׵� ����)
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
        // ReadPixels�Լ��� ���� ������ ���ۿ��� �ȼ����� �ҷ����� ������, �ش� �������� �������� ������ ���� �� ����Ǿ�� �Ѵ�.
        // (��, ReadPixels�Լ� ���� ���� WaitForEndOfFrame�� ����Ǿ�� �Ѵ�.)
        yield return new WaitForEndOfFrame();

        // ��ũ�� �� ���
        Texture2D screenShot = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
        screenShot.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        screenShot.Apply();

        spuit = screenShot.GetPixel((int)Input.mousePosition.x, (int)Input.mousePosition.y);

        // �޸� ����(�����ָ� ������)
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

            drawPrefab.GetComponent<LineRenderer>().sortingLayerName = layerName[layerNum];

            // ��Ʈ��ũ (�ٸ� ��������׵� ����)
            photonView.RPC("RpcLayer", RpcTarget.OthersBuffered, drawPrefabName, layerNum);
        }
    }

    [PunRPC]
    void RpcLayer(string _drawPrefabName, int _layerNum)
    {
        drawPrefab = Resources.Load<GameObject>(_drawPrefabName);

        drawPrefab.GetComponent<LineRenderer>().sortingLayerName = layerName[_layerNum];
    }
}
