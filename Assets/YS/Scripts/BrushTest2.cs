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

    // 사이즈
    float size;

    // Start is called before the first frame update
    void Start()
    {
        planObj = new Plane(Camera.main.transform.forward, transform.position);
    }

    // Update is called once per frame
    void Update()
    {
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

        // 사이즈 조절
        Size();
    }

    void Size()
    {
        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            size += 0.1f;
        }

    }
}
