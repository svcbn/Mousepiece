using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class ConnectionManager_BH : MonoBehaviourPunCallbacks
{
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            OnClickConnect();
        }
    }

    public void OnClickConnect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnected()
    {
        base.OnConnected();
        print("������ ���� ���� ����");
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        print("������ ������ ������");

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        print("�κ� ���� ����");

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }

}
