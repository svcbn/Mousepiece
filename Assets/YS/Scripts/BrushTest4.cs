using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushTest4 : MonoBehaviour
{
    public GameObject brush;

    LineRenderer lineRenderer;

    Vector2 lastPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Draw();
    }

    void Draw()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            CreateBrush();
        }
        if(Input.GetKey(KeyCode.Mouse0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if(mousePos != lastPos)
            {
                AddPoint(mousePos);
                lastPos = mousePos;
            }
        }
    }

    void CreateBrush()
    {
        GameObject brushInstance = Instantiate(brush);
        lineRenderer = brushInstance.GetComponent<LineRenderer>();

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        lineRenderer.SetPosition(0, mousePos);
        lineRenderer.SetPosition(1, mousePos);
    }

    void AddPoint(Vector2 pointPos)
    {
        lineRenderer.positionCount++;
        int positionIndex = lineRenderer.positionCount - 1;
        lineRenderer.SetPosition(positionIndex, pointPos);
    }
}
