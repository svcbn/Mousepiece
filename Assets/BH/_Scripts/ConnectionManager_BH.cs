using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class ConnectionManager_BH : MonoBehaviourPunCallbacks
{
    public Transform startPos;
    public Transform endPos;
    public Light light;
    

    // Start is called before the first frame update
    void Start()
    {
        fadeImage.color = new Color(1, 1, 1, 0);
        fadeImage.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            OnClickConnect();
        }

        light.transform.eulerAngles += new Vector3(0, 0.05f, 0);
    }

    public void OnClickConnect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnected()
    {
        base.OnConnected();
        print("마스터 서버 접속 성공");
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        print("마스터 서버에 접속중");

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        print("로비 접속 성공");

        StartCoroutine(FadeInSceneChange());
        StartCoroutine(FadeInCameraMove());
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }

    public Image fadeImage;
    IEnumerator FadeInSceneChange()
    {
        fadeImage.enabled = true;
        float alpha = 0f;
        while(alpha < 0.95f)
        {
            alpha = Mathf.Lerp(alpha, 1, Time.deltaTime * 1f);
            fadeImage.color = new Color(1, 1, 1, alpha);

            yield return null;
        }

        fadeImage.color = new Color(1, 1, 1, 1);
        PhotonNetwork.LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);

    }

    IEnumerator FadeInCameraMove()
    {
        while(true)
        {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, endPos.position, Time.deltaTime * 0.5f);

            yield return null;
        }
    }

}
