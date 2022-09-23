using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushTest5 : MonoBehaviour
{
    // ���� ��ȣ
    int toolNum = 1;

    public GameObject drawPrefab;
    public GameObject drawCanvas, drawCanvas_parent, drawCanvas_parent2, drawCanvas_parent3;
    GameObject theTrail;
    Plane planObj;
    Vector3 startPos;
    Vector3 nextPos;

    // ������
    float size = 0.05f;
    // �� ����
    public GameObject colorObject;

    // ���찳
    public bool b_eraser;

    // �̵�����
    Vector3 canvasPos;
    Vector3 dis;

    // ��� �� ����
    List<GameObject> lines = new List<GameObject>();

    // ������Ʈ
    public Color spuit;

    // ���� ĵ���� ������ �׸��� �ִ��� �Ǵ�(Ư�� �׸��ٰ� �������� ���)
    bool b_onCanvas;

    // Start is called before the first frame update
    void Start()
    {
        planObj = new Plane(Camera.main.transform.forward, drawCanvas.transform.position);

        canvasPos = drawCanvas.transform.position;

        //drawPrefab = Resources.Load<GameObject>("YS/Brush");
        //drawPrefab.GetComponent<LineRenderer>().useWorldSpace = false;
    }

    // Update is called once per frame
    void Update()
    {
        // ���� (1���� �귯��, 2���� ����)
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            toolNum = 1;

            // �귯�� ���� �Ҵ�
            drawPrefab = Resources.Load<GameObject>("YS/Brush");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            toolNum = 2;

            // ���� ���� �Ҵ�
            drawPrefab = Resources.Load<GameObject>("YS/Blending");
        }

        // ���� ���
        if(toolNum == 2)
        {
            // ���� �� �޾ƿ���
            drawPrefab.GetComponent<LineRenderer>().startColor = spuit;
            drawPrefab.GetComponent<LineRenderer>().endColor = spuit;
        }

        //if(GameManager_BH.instance.state == GameManager_BH.gameState.Playing)
        //{
        // �׸� �׸���
        Draw();
        //}

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

        // ������Ʈ
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
        {
            StartCoroutine(Spuit());
        }
    }

    void Draw()
    {
        Color eraser = Color.white;

        Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(mouseRay, out hit))
            {
                if (hit.transform.name == drawCanvas.transform.name)
                {
                    // ĵ���� ������ �׸��� ����!
                    b_onCanvas = true;

                    // �귯���� ��
                    if(toolNum == 1)
                    {
                        // ���� �׸��� ��, ������ ����
                        drawPrefab.GetComponent<LineRenderer>().widthMultiplier = size;
                        // ���� �׸��� ��, �� ����
                        drawPrefab.GetComponent<LineRenderer>().startColor = colorObject.GetComponent<ColorPickerTest>().selectedColor;
                        drawPrefab.GetComponent<LineRenderer>().endColor = colorObject.GetComponent<ColorPickerTest>().selectedColor;
                        // ���찳
                        if (b_eraser == true)
                        {
                            drawPrefab.GetComponent<LineRenderer>().startColor = eraser;
                            drawPrefab.GetComponent<LineRenderer>().endColor = eraser;
                        }
                    }
                    // ���߿� ���� ���� ���� �ö���Բ�
                    drawPrefab.GetComponent<LineRenderer>().sortingOrder++;

                    // �� ����
                    theTrail = (GameObject)Instantiate(drawPrefab, objPosition, Quaternion.identity);
                    theTrail.transform.SetParent(drawCanvas.transform, false);
                    // ���� ������ ��, ����Ʈ�� active�� false�� �͵��� ����
                    for (int i = 0; i < lines.Count; i++)
                    {
                        if (lines[i].activeSelf == false)
                        {
                            Destroy(lines[i].gameObject); // ������ ����(������� ���� �����)
                            lines.RemoveAt(i);
                            i--;
                        }
                    }
                    // ���� ��, ����Ʈ�� �־��ֱ�
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
                    if(b_onCanvas == false)
                    {
                        // �� ����
                        theTrail = (GameObject)Instantiate(drawPrefab, objPosition, Quaternion.identity);
                        theTrail.transform.SetParent(drawCanvas.transform, false);
                        // ���� ������ ��, ����Ʈ�� active�� false�� �͵��� ����
                        for (int i = 0; i < lines.Count; i++)
                        {
                            if (lines[i].activeSelf == false)
                            {
                                Destroy(lines[i].gameObject); // ������ ����(������� ���� �����)
                                lines.RemoveAt(i);
                                i--;
                            }
                        }
                        // ���� ��, ����Ʈ�� �־��ֱ�
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

                    // ĵ���� ������ �׸��� ����!
                    b_onCanvas = true;

                    // �귯�� �� ��
                    if (toolNum == 1)
                    {
                        //nextPos = mouseRay.GetPoint(_dis);
                        //nextPos.z = drawCanvas.transform.position.z;
                        nextPos = hit.point;

                        theTrail.GetComponent<LineRenderer>().positionCount++;
                        int positionIndex = theTrail.GetComponent<LineRenderer>().positionCount - 1;
                        theTrail.GetComponent<LineRenderer>().SetPosition(positionIndex, nextPos);
                    }
                    // ���� ��� �� ��
                    else if(toolNum == 2)
                    {
                        nextPos = hit.point;

                        theTrail.GetComponent<LineRenderer>().positionCount++;
                        int positionIndex = theTrail.GetComponent<LineRenderer>().positionCount - 1;
                        theTrail.GetComponent<LineRenderer>().SetPosition(positionIndex, nextPos);

                        if(theTrail.GetComponent<LineRenderer>().startColor != spuit)
                        {
                            // �� ����
                            theTrail = (GameObject)Instantiate(drawPrefab, objPosition, Quaternion.identity);
                            theTrail.transform.SetParent(drawCanvas.transform, false);
                            // ���� ������ ��, ����Ʈ�� active�� false�� �͵��� ����
                            for (int i = 0; i < lines.Count; i++)
                            {
                                if (lines[i].activeSelf == false)
                                {
                                    Destroy(lines[i].gameObject); // ������ ����(������� ���� �����)
                                    lines.RemoveAt(i);
                                    i--;
                                }
                            }
                            // ���� ��, ����Ʈ�� �־��ֱ�
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
                    // ĵ���� ������ �׸��� ���� ����!
                    b_onCanvas = false;
                }
            }
        }
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
        /*// canvasPos�� drawCanvas�� ��ġ�� ���ؼ� drawCanvas�� ���������� �Ǵ�
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

        canvasPos = drawCanvas.transform.position; // ���� drawCanva�� ��ġ�� �־��ش�.*/

        if (drawCanvas.transform.position != canvasPos)
        {
            //dis = drawCanvas.transform.position - canvasPos;

            for (int i = 0; i < lines.Count; i++)
            {
                for (int j = 0; j < lines[i].GetComponent<LineRenderer>().positionCount; j++)
                {
                    Vector3 drawPos = lines[i].GetComponent<LineRenderer>().GetPosition(j);

                    lines[i].GetComponent<LineRenderer>().SetPosition(j, drawCanvas.transform.localToWorldMatrix * drawCanvas_parent.transform.localToWorldMatrix * drawCanvas_parent2.transform.localToWorldMatrix * drawCanvas_parent3.transform.localToWorldMatrix * new Vector4(drawPos.x, drawPos.y, drawPos.z, 1));
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
}
