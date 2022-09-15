using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YS_Palette : MonoBehaviour
{
    public Image palette;
    public Image picker;
    public Color color;
    Texture2D texture;

    Vector2 colorPos;

    Vector2 paletteSize;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        paletteSize = new Vector2(palette.GetComponent<RectTransform>().rect.width, palette.GetComponent<RectTransform>().rect.height);
    }

    public void SelectColor()
    {
        /*if (Input.GetMouseButtonDown(0))
        {*/
        //Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        //RaycastHit hit;

        //if (Physics.Raycast(mouseRay, out hit))
        //{
        //if (hit.transform.name == palette.transform.name)
        //{
        colorPos = transform.position;
        Vector2 palettePos = palette.transform.position;

        Vector2 pos = colorPos - palettePos + (paletteSize * 0.5f);
        Vector2 normalizePos = new Vector2(pos.x / (paletteSize.x), pos.y / (paletteSize.y));

        texture = palette.mainTexture as Texture2D;

        color = texture.GetPixelBilinear(normalizePos.x, normalizePos.y);
                //}
            //}
        //}
    }
}
