using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerVote : MonoBehaviourPun
{
    Camera cam;

    bool canVote = false;

    GameObject go;

    public bool CanVote
    {
        get
        {
            return canVote;
        }
        set
        {
            canVote = value;
        }
    }

    public static PlayerVote instance;

    private void Awake()
    {
        instance = this;    

    }

    void Start()
    {
        cam = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if(photonView.IsMine)
        {
            if(CompeteModeManager_BH.instance.state == CompeteModeManager_BH.gameState.Vote)
            {
                if(Input.GetButtonDown("Fire1"))
                {
                    VoteFavorite();
                }
            
            }
        }
        
    }

    

    void VoteFavorite()
    {
        
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo))
        {
            if (hitInfo.collider.gameObject.GetComponent<Likes>())
            {
                hitInfo.collider.gameObject.GetComponent<Likes>().likes++;

                go = hitInfo.collider.gameObject;
                int viewID = go.GetPhotonView().ViewID;
                
                if(!PhotonNetwork.IsMasterClient)
                {
                    canVote = false;

                }
                photonView.RPC("RPCVote", RpcTarget.OthersBuffered, viewID);
            }
        }

    }

    [PunRPC]
    void RPCVote(int id)
    {
        //if (gameobj.GetComponent<Likes>())
        //{
        //gameobj.GetComponent<Likes>().likes++;

        //canVote = false;
        //}

        PhotonView.Find(id).GetComponent<Likes>().likes++;

    }
}
