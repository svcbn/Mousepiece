using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Rendering.Universal;

public class CameraRotate_BH : MonoBehaviourPun
{
    public float rotSpeed = 200;

    float mx;
    float my;
    public static int cursorType = 1;
    public GameObject cam;
    Camera uiCamera;

    // Start is called before the first frame update
    void Start()
    {
        
        if (photonView.IsMine == false)
        {
            cam.SetActive(false);
        }
        
        Cursor.lockState = CursorLockMode.Confined;

        uiCamera = GameObject.Find("UICamera").GetComponent<Camera>();
        cam.GetComponent<Camera>().GetUniversalAdditionalCameraData().cameraStack.Add(uiCamera);
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            float h = Input.GetAxis("Mouse X");
            float v = Input.GetAxis("Mouse Y");

            mx += h * rotSpeed * Time.deltaTime;
            my += v * rotSpeed * Time.deltaTime;

            my = Mathf.Clamp(my, -60, 60);

            if (cursorType == 0)
            {
                transform.eulerAngles = new Vector3(-my, mx, 0);
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                cursorType++;
                cursorType %= 2;

                if (cursorType == 0)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Confined;
                }
            }
        }
    }
}
