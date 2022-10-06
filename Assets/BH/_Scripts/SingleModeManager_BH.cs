using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using Photon.Realtime;

public class SingleModeManager_BH : MonoBehaviourPunCallbacks
{

    //public Canvas drawCanvas;


    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.Instantiate("Player1", Vector3.zero + Vector3.up * 0.5f, Quaternion.identity);

    }

    // Update is called once per frame
    void Update()
    {


    }


}
