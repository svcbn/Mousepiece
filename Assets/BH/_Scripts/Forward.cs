using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Forward : MonoBehaviourPun
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            transform.forward = Camera.main.transform.forward;
        }
    }
}
