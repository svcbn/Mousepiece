using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class GameManager_BH : MonoBehaviourPun
{
    public enum gameState
    {
        Ready,
        Start,
        Playing,
        Vote,
        Victory
    }

    public static gameState state = gameState.Ready;

    public InputField theme;

    // Start is called before the first frame update
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            case gameState.Ready:
                Ready();
                break;
            case gameState.Start:
                RoundStart();
                break;
            case gameState.Playing:
                Playing();
                break;
            case gameState.Vote:
                Vote();
                break;
            case gameState.Victory:
                Victory();
                break;

        }
        
    }

    private void Ready()
    {
        
    }

    private void RoundStart()
    {
        throw new NotImplementedException();
    }

    private void Playing()
    {
        throw new NotImplementedException();
    }

    private void Vote()
    {
        throw new NotImplementedException();
    }

    private void Victory()
    {
        throw new NotImplementedException();
    }
   
    
}
