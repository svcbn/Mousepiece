using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YS_BrushSize : MonoBehaviour
{
    public Image circle;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MousePointerDown()
    {
        MoveCircle();
    }

    public void MouseDrag()
    {
        MoveCircle();
    }

    void MoveCircle()
    {
        Vector3 offset = Input.mousePosition - transform.position;

        offset.y = Mathf.Clamp(offset.y, -121, -121);
        offset.x = Mathf.Clamp(offset.x, -180, 180);
        offset.y += 121f;

        circle.transform.position = transform.position + offset;
    }
}
