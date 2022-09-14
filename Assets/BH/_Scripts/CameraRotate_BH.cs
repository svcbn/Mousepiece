using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    public float rotSpeed = 200;

    float mx;
    float my;

    public bool bUseHorizontal = true;
    public bool bUseVertical = true;

    // Start is called before the first frame update
    void Start()
    {
        if (bUseHorizontal) mx = transform.eulerAngles.y;
        if (bUseVertical) my = -transform.eulerAngles.x;
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
