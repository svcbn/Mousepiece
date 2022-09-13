using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushTest2 : MonoBehaviour
{
    public GameObject drawPrefab;
    GameObject theTrail;
    Plane planObj;
    Vector3 startPos;
    // Start is called before the first frame update
    void Start()
    {
        planObj = new Plane(Camera.main.transform.forward * -1, transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);

        if(Input.GetMouseButtonDown(0))
        {
            theTrail = (GameObject)Instantiate(drawPrefab, objPosition, Quaternion.identity);

            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            float _dis;
            if(planObj.Raycast(mouseRay, out _dis))
            {
                startPos = mouseRay.GetPoint(_dis);
            }
        }
        else if(Input.GetMouseButton(0))
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            float _dis;
            if(planObj.Raycast(mouseRay, out _dis))
            {
                theTrail.transform.position = mouseRay.GetPoint(_dis);
            }
        }
    }
}
