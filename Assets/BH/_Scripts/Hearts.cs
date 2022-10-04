using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Hearts : MonoBehaviourPun
{
    Likes likes;
    public Image[] hearts = new Image[8];

    // Start is called before the first frame update
    void Start()
    {
        likes = GetComponentInParent<Likes>();
        FillHeart();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if(likes.likes > i && hearts[i].enabled == false)
            {
                hearts[i].enabled = true;
            }

            if(likes.likes <= i && hearts[i].enabled == true)
            {
                hearts[i].enabled = false;
            }
        }
    }

    void FillHeart()
    {
        hearts = GetComponentsInChildren<Image>();
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].enabled = false;
        }
    }

    
}
