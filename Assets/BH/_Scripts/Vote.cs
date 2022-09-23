using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vote : MonoBehaviour
{
    Camera cam;

    bool canVote = 0;

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
    
    void Start()
    {
        cam = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager_BH.instance.state == GameManager_BH.gameState.Vote)
        {
            if(Input.GetButtonDown("Fire1"))
            {
                VoteFavorite();
            }
        }
    }

    void VoteFavorite()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit hitInfo;

        if(Physics.Raycast(ray, out hitInfo))
        {
            if(hitInfo.collider.)
        }
    }
}
