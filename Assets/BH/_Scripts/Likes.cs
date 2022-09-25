using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Likes : MonoBehaviourPun
{

    [SerializeField]
    int _likes = 0;

    public int Like
    {
        get 
        {
            return _likes;
        }
        set 
        {
            _likes = value;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   
}
