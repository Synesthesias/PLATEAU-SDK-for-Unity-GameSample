using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
    public enum State
    {
        Normal,
        HookshotFlyingPlayer,
        //HookshotThrown
    }
    public State state;

   
    private StarterAssets.ThirdPersonController thirdPersonController;

    private StarterAssets.HookshotHandle hookshotHandle;

    private bool hookshotAble;

    private void Awake()
    {
        thirdPersonController = GetComponent<StarterAssets.ThirdPersonController>();

        hookshotHandle = GetComponent<StarterAssets.HookshotHandle>();

        state = State.Normal;
        hookshotAble = hookshotHandle.hookshotAble;
        //hookshotHandle.enabled = false;

    }

    private void Update()
    {
        if(hookshotAble == true)
        {
            thirdPersonController.enabled = false;
            state = State.HookshotFlyingPlayer;
        }
        else
        {
            thirdPersonController.enabled = true;
            state = State.Normal;
        }

        switch (state)
        {
            default:
            case State.Normal:
                thirdPersonController.enabled = true;
                break;
            case State.HookshotFlyingPlayer:
                thirdPersonController.enabled = false;
                break;

        }

        Debug.Log(state);
    }

}
