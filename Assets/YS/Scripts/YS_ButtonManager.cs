using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class YS_ButtonManager : MonoBehaviourPun
{
    public GameObject colorPicker, picker, back, front, eraser, clear, basicBrush, oilPaintBrush, waterColorBrush, pencil, calligraphy, marker, crayon, spray, fingerBlending, brushSize, circle, layers, spuit;
    public BrushNet_YS brushNet;
    public Image palette;

    // 이전 원의 위치 값
    float circle_temp;
    // 스포이트 실행 중
    bool b_spuit = false;

    // Start is called before the first frame update
    void Start()
    {
        circle_temp = circle.GetComponent<RectTransform>().localPosition.x;
        palette = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        // 브러쉬 툴 넣어주기
        if(brushNet == null)
        {
            if(CollaborateModeManager_BH.instance)
            {
                brushNet = GameObject.Find("Player1(Clone)").GetComponent<BrushNet_YS>();
            }
            else if(CompeteModeManager_BH.instance)
            {
                brushNet = GameObject.Find("Player(Clone)").GetComponent<BrushNet_YS>();
            }
        }

        // 브러쉬 사이즈 조절
        BrushSize();

        // 플레이 중일 때만
        if(CompeteModeManager_BH.instance.state == CompeteModeManager_BH.gameState.Playing)
        {
            palette.enabled = true;
        }

        // 스포이트
        if(b_spuit == true)
        {
            Spuiting();
        }
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
            layers.SetActive(true);
            spuit.SetActive(true);
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
            layers.SetActive(false);
            spuit.SetActive(false);
        }
    }

    public void Eraser()
    {
        if(brushNet.b_eraser == false)
        {
            brushNet.b_eraser = true;

            // 지우개 동적 할당
            brushNet.drawPrefab_temp = brushNet.drawPrefab;
            brushNet.drawPrefab = Resources.Load<GameObject>("YS/Eraser");
            brushNet.drawPrefabName = "YS/Eraser";
        }
        else
        {
            brushNet.b_eraser = false;

            // 지우개를 사용하기 전 도구로
            brushNet.drawPrefab = brushNet.drawPrefab_temp;
        }
    }

    public void BackFunction()
    {
        for (int i = brushNet.lines[brushNet.myCanvasIdx].Count - 1; i >= 0; i--)
        {
            if (brushNet.lines[brushNet.myCanvasIdx][i].activeSelf == true)
            {
                brushNet.lines[brushNet.myCanvasIdx][i].SetActive(false);
                break;
            }
        }

        // 네트워크 (다른 사람들한테도 적용)
        brushNet.photonView.RPC("RpcCtrZ", RpcTarget.OthersBuffered, brushNet.myCanvasIdx);
    }

    public void FrontFunction()
    {
        for (int i = 0; i < brushNet.lines[brushNet.myCanvasIdx].Count; i++)
        {
            if (brushNet.lines[brushNet.myCanvasIdx][i].activeSelf == false)
            {
                brushNet.lines[brushNet.myCanvasIdx][i].SetActive(true);
                break;
            }
        }

        // 네트워크 (다른 사람들한테도 적용)
        brushNet.photonView.RPC("RpcCtrY", RpcTarget.OthersBuffered, brushNet.myCanvasIdx);
    }

    public void CanvasClear()
    {
        for (int i = 0; i < brushNet.lines[brushNet.myCanvasIdx].Count; i++)
        {
            Destroy(brushNet.lines[brushNet.myCanvasIdx][i].gameObject);
        }
        brushNet.lines[brushNet.myCanvasIdx].Clear();

        // 네트워크 (다른 사람들한테도 적용)
        brushNet.photonView.RPC("RcpAllClear", RpcTarget.OthersBuffered, brushNet.myCanvasIdx);
    }

    public void BasicBrush()
    {
        if(brushNet.b_eraser == true)
        {
            // 지우개 상태였다면 취소!
            brushNet.b_eraser = false;
        }

        brushNet.toolNum = 1;

        // 브러쉬 동적 할당
        brushNet.drawPrefab = Resources.Load<GameObject>("YS/Brush");
        brushNet.drawPrefabName = "YS/Brush";
    }

    public void OilPaintBrush()
    {
        if (brushNet.b_eraser == true)
        {
            // 지우개 상태였다면 취소!
            brushNet.b_eraser = false;
        }

        brushNet.toolNum = 8;

        // 유화 동적 할당
        brushNet.drawPrefab = Resources.Load<GameObject>("YS/OilPaint");
        brushNet.drawPrefabName = "YS/OilPaint";
    }

    public void WaterColorBrush()
    {
        if (brushNet.b_eraser == true)
        {
            // 지우개 상태였다면 취소!
            brushNet.b_eraser = false;
        }

        brushNet.toolNum = 9;

        // 수채화 동적 할당
        brushNet.drawPrefab = Resources.Load<GameObject>("YS/WaterPaint");
        brushNet.drawPrefabName = "YS/WaterPaint";
    }

    public void Pencil()
    {
        if (brushNet.b_eraser == true)
        {
            // 지우개 상태였다면 취소!
            brushNet.b_eraser = false;
        }

        brushNet.toolNum = 4;

        // 연필 동적 할당
        brushNet.drawPrefab = Resources.Load<GameObject>("YS/Pencil");
        brushNet.drawPrefabName = "YS/Pencil";
    }

    public void Calligraphy()
    {
        if (brushNet.b_eraser == true)
        {
            // 지우개 상태였다면 취소!
            brushNet.b_eraser = false;
        }

        brushNet.toolNum = 5;

        // 캘리그라피 동적 할당
        brushNet.drawPrefab = Resources.Load<GameObject>("YS/Calligraphy");
        brushNet.drawPrefabName = "YS/Calligraphy";
    }

    public void Marker()
    {
        if (brushNet.b_eraser == true)
        {
            // 지우개 상태였다면 취소!
            brushNet.b_eraser = false;
        }

        brushNet.toolNum = 3;

        // 마커 동적 할당
        brushNet.drawPrefab = Resources.Load<GameObject>("YS/Marker");
        brushNet.drawPrefabName = "YS/Marker";
    }

    public void Crayon()
    
    {
        if (brushNet.b_eraser == true)
        {
            // 지우개 상태였다면 취소!
            brushNet.b_eraser = false;
        }

        brushNet.toolNum = 6;

        // 크레용 동적 할당
        brushNet.drawPrefab = Resources.Load<GameObject>("YS/Crayon");
        brushNet.drawPrefabName = "YS/Crayon";
    }

    public void Spray()
    {
        if (brushNet.b_eraser == true)
        {
            // 지우개 상태였다면 취소!
            brushNet.b_eraser = false;
        }

        brushNet.toolNum = 7;

        // 스프레이 동적 할당
        brushNet.drawPrefab = Resources.Load<GameObject>("YS/Spray");
        brushNet.drawPrefabName = "YS/Spray";
    }

    public void FingerBlending()
    {
        if (brushNet.b_eraser == true)
        {
            // 지우개 상태였다면 취소!
            brushNet.b_eraser = false;
        }

        brushNet.toolNum = 2;

        // 블렌딩 동적 할당
        brushNet.drawPrefab = Resources.Load<GameObject>("YS/Blending");
        brushNet.drawPrefabName = "YS/Blending";
    }

    public void BrushSize()
    {
        // circle이 움직이면,
        if(circle_temp != circle.GetComponent<RectTransform>().localPosition.x)
        {
            // circle의 비교대상은 시작 브러쉬의 크기에 해당하는 위치(0.05 * 0.0013812154696133)
            if (circle_temp < circle.GetComponent<RectTransform>().localPosition.x)
            {
                // 비율로 계산 (circle의 움직일 수 있는 범위는 362, 브러쉬 사이즈는 0.5로 계산)
                brushNet.size += (circle.GetComponent<RectTransform>().localPosition.x - circle_temp) * 0.0013812154696133f;
            }
            else if(circle_temp > circle.GetComponent<RectTransform>().localPosition.x)
            {
                // 비율로 계산 (circle의 움직일 수 있는 범위는 362, 브러쉬 사이즈는 0.5로 계산)
                brushNet.size -= (circle_temp - circle.GetComponent<RectTransform>().localPosition.x) * 0.0013812154696133f;
            }
        }

        // circle의 이전 위치 저장
        circle_temp = circle.GetComponent<RectTransform>().localPosition.x;
    }

    public void Layer1()
    {
        brushNet.drawPrefab.GetComponent<LineRenderer>().sortingLayerName = brushNet.layerName[0];
        brushNet.layerNum = 0;

        // 네트워크 (다른 사람들한테도 적용)
        brushNet.photonView.RPC("RpcLayer", RpcTarget.OthersBuffered, brushNet.drawPrefabName, 0);
    }

    public void Layer2()
    {
        brushNet.drawPrefab.GetComponent<LineRenderer>().sortingLayerName = brushNet.layerName[1];
        brushNet.layerNum = 1;

        // 네트워크 (다른 사람들한테도 적용)
        brushNet.photonView.RPC("RpcLayer", RpcTarget.OthersBuffered, brushNet.drawPrefabName, 1);
    }

    public void Layer3()
    {
        brushNet.drawPrefab.GetComponent<LineRenderer>().sortingLayerName = brushNet.layerName[2];
        brushNet.layerNum = 2;

        // 네트워크 (다른 사람들한테도 적용)
        brushNet.photonView.RPC("RpcLayer", RpcTarget.OthersBuffered, brushNet.drawPrefabName, 2);
    }

    public void SpuitOn()
    {
        b_spuit = true;
    }

    void Spuiting()
    {
        brushNet.StartCoroutine("Spuit");

        if (Input.GetMouseButtonDown(0))
        {
            brushNet.colorObject.GetComponent<ColorPickerTest>().selectedColor = brushNet.spuit;

            b_spuit = false;
        }
    }
}
