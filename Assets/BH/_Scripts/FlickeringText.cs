using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlickeringText : MonoBehaviour
{
    float speed = 1f;
    public Text pressToStart;
    float alpha = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }
     
    // Update is called once per frame
    void Update()
    {
        alpha = Mathf.Sin(speed * Time.time);
        pressToStart.color = new Color(pressToStart.color.r, pressToStart.color.g, pressToStart.color.b, Mathf.Abs(alpha));
    }
}