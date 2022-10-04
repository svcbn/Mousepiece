using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerVote : MonoBehaviourPun
{
    Camera cam;

    bool canVote = false;

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
        if(CompeteModeManager_BH.instance.state == CompeteModeManager_BH.gameState.Vote)
        {
            if(Input.GetButtonDown("Fire1"))
            {
                VoteFavorite();
            }
            
        }
        
    }

    void VoteFavorite()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        

        if(Physics.Raycast(ray, out hitInfo))
        {
            if (hitInfo.collider.gameObject.GetComponent<Likes>())
            {
                hitInfo.collider.gameObject.GetComponent<Likes>().Like++;

                photonView.RPC("RPCVote", RpcTarget.Others, hitInfo);
                //canVote = false;
            }
        }
    }

    [PunRPC]
    void RPCVote(RaycastHit _hitInfo)
    {
        _hitInfo.collider.gameObject.GetComponent<Likes>().Like++;
    }
}
