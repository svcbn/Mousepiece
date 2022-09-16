using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate_BH : MonoBehaviour
{
    public float rotSpeed = 200;

    float mx;
    float my;


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

        my = Mathf.Clamp(my, -60, 60);

        transform.eulerAngles = new Vector3(-my, mx, 0);
    }
}
