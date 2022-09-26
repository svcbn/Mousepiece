using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YS_ButtonManager : MonoBehaviour
{
    public GameObject palette, back, front, eraser;
    public BrushTest_BH brush;
    // Start is called before the first frame update
    void Start()
    {
        //brush = GameObject.Find("Player").GetComponent<BrushTest_BH>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PaletteOnOff()
    {
        if(palette.activeSelf == false)
        {
            palette.SetActive(true);
            back.SetActive(true);
            front.SetActive(true);
            eraser.SetActive(true);
        }
        else
        {
            palette.SetActive(false);
            back.SetActive(false);
            front.SetActive(false);
            eraser.SetActive(false);
        }
    }

    public void Eraser()
    {
        if(brush.b_eraser == false)
        {
            brush.b_eraser = true;
        }
        else
        {
            brush.b_eraser = false;
        }
    }

    public void BackFunction()
    {
        
    }

    public void FrontFunction()
    {

    }
}
