using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPickerTest : MonoBehaviour
{
    public Image circlePalette;
    public Image picker;
    public Color selectedColor;

    private Vector2 sizeOfPalette;
    //private CircleCollider2D paletteCollider;
    private BoxCollider2D paletteCollider;

    void Start()
    {
        //paletteCollider = circlePalette.GetComponent<CircleCollider2D>();
        paletteCollider = circlePalette.GetComponent<BoxCollider2D>();

        sizeOfPalette = new Vector2(
            circlePalette.GetComponent<RectTransform>().rect.width+170,
            circlePalette.GetComponent<RectTransform>().rect.height+170);
    }

    public void mousePointerDown()
    {
        selectColor();
    }

    public void mouseDrag()
    {
        selectColor();
    }

    private Color getColor()
    {
        Vector2 circlePalettePosition = circlePalette.transform.position;
        Vector2 pickerPosition = picker.transform.position;

        Vector2 position = pickerPosition - circlePalettePosition + sizeOfPalette * 0.5f;
        Vector2 normalized = new Vector2((position.x / (circlePalette.GetComponent<RectTransform>().rect.width+170)), (position.y / (circlePalette.GetComponent<RectTransform>().rect.height+170)));

        Texture2D texture = circlePalette.mainTexture as Texture2D;
        Color circularSelectedColor = texture.GetPixelBilinear(normalized.x, normalized.y);
        circularSelectedColor.a = 1;

        return circularSelectedColor;
    }

    private void selectColor()
    {
        Vector3 offset = Input.mousePosition - transform.position;
        offset.x = Mathf.Clamp(offset.x, -180, 170);
        offset.y = Mathf.Clamp(offset.y, -180, 170);

        picker.transform.position = transform.position + offset;

        selectedColor = getColor();
    }
}