using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Likes : MonoBehaviourPun, IPunObservable
{

    [SerializeField]
    public int likes = 0;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(likes);
        }
        else if (stream.IsReading)
        {
            likes = (int)stream.ReceiveNext();
        }
    }
}
