using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YS_Palette : MonoBehaviour
{
    public Image palette;
    public Color color;
    Texture2D texture;

    Vector3 colorPos;

    // Start is called before the first frame update
    void Start()
    {
        texture = (Texture2D)palette.mainTexture;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(mouseRay, out hit))
            {
                if (hit.transform.name == palette.transform.name)
                {
                    colorPos = Input.mousePosition;
                    color = texture.GetPixelBilinear(colorPos.x, colorPos.y);
                }
            }
        }
    }
}
