using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class YS_Capture : MonoBehaviour
{
    string name = "capture";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Shot();
        }
    }

    public void Shot()
    {
        // Ä¸ÃÄ (Å¸°Ù¸¸)
        RenderTexture renderTex = GetComponent<Camera>().targetTexture;
        Texture2D tex = new Texture2D(renderTex.width, renderTex.height, TextureFormat.RGBA32, false);
        RenderTexture.active = renderTex;
        tex.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        tex.Apply();

        // ÆÄÀÏ ÀúÀå
        File.WriteAllBytes($"{Application.dataPath}/{name}.png", tex.EncodeToPNG());
    }
}
