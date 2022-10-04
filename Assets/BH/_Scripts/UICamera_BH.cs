using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICamera_BH : MonoBehaviour
{
    float mx;
    float my;
    public GameObject cam;

    public float rotSpeed = 10f;
    public float rotClamp = 1.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Mouse X");
        float v = Input.GetAxis("Mouse Y");

        mx += h * rotSpeed * Time.deltaTime;
        my += v * rotSpeed * Time.deltaTime;

        mx = Mathf.Clamp(mx, -rotClamp, rotClamp);
        my = Mathf.Clamp(my, -rotClamp, rotClamp);

        transform.eulerAngles = new Vector3(-my, mx, 0);
        
    }
}
