using RosSharp.RosBridgeClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class GrabManager : StateListener
{
    public GameObject leftHand, rightHand;
    private JointStateWriter leftWriter, rightWriter;

    private SteamVR_Action_Single grabStuff;

    void Start()
    {
        Register();

        grabStuff = SteamVR_Actions._default.Squeeze;

        leftWriter = leftHand.GetComponent<JointStateWriter>();
        if (leftWriter == null)
            Debug.Log("No Writer Component found in Module " + transform.parent.name);

        rightWriter = rightHand.GetComponent<JointStateWriter>();
        if (rightWriter == null)
            Debug.Log("No Writer Component found in Module " + transform.parent.name);
    }

    void Update()
    {
        if (state == StateManager.State.armed)
        {
            //57° is a closed hand
            leftWriter?.Write(grabStuff.GetAxis(SteamVR_Input_Sources.LeftHand) * 57 * Mathf.Deg2Rad);
            rightWriter?.Write(grabStuff.GetAxis(SteamVR_Input_Sources.RightHand) * 57 * Mathf.Deg2Rad);
        }
    }
}
