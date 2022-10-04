using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PhotonView))]
public class PlayerMove_BH : MonoBehaviourPun,IPunObservable
{
    public float speed = 5;
    
    CharacterController cc;

    bool isground;
    float gravity = -5f;

    public Text nickName;

    private void Awake()
    {
        //GameManager_BH.instance.playerPV.Add(GetComponent<PhotonView>());
        //GameManager_BH.instance.playerList[PhotonNetwork.CurrentRoom.PlayerCount - 1] = gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
        nickName.text = PhotonNetwork.NickName;
    }

    Vector3 recievePos;
    Quaternion recieveRot;
    float lerpSpeed;

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            //anim.SetFloat("Speed", v);
            //anim.SetFloat("Direction", h);

            if(cc.isGrounded)
            {
                gravity = 0;
            }

            gravity -= Time.deltaTime;

            Vector3 dir = new Vector3(h, 0, v);
            dir = Camera.main.transform.TransformDirection(dir);
            dir.y = gravity;

            cc.Move(dir * speed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, recievePos, speed * Time.deltaTime);
            transform.rotation = recieveRot;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else if(stream.IsReading)
        {
            recievePos = (Vector3)stream.ReceiveNext();
            recieveRot = (Quaternion)stream.ReceiveNext();
        }    
    }

    
}
