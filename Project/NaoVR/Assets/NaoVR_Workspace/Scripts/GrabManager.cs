using NaoApi.Behavior;
using RosSharp.RosBridgeClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class GrabManager : StateListener
{
    public GameObject leftHand, rightHand;
    private JointStateWriter leftWriter, rightWriter;
    public BehaviorController behaviorController;

    private SteamVR_Action_Boolean grabStuff = SteamVR_Actions._default.CloseHand;

    void Start()
    {
        Register();

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
            if (grabStuff.GetStateDown(SteamVR_Input_Sources.RightHand))
            {
                Debug.Log("closeRightHand");
                behaviorController.runBehavior("handcontroller-dc72e7/closeRightHand");
            }

            if (grabStuff.GetStateUp(SteamVR_Input_Sources.RightHand))
            {
                Debug.Log("openRightHand");
                behaviorController.runBehavior("handcontroller-dc72e7/openRightHand");
            }

            if (grabStuff.GetStateDown(SteamVR_Input_Sources.LeftHand))
            {
                Debug.Log("closeLeftHand");
                behaviorController.runBehavior("handcontroller-dc72e7/closeLeftHand");
            }

            if (grabStuff.GetStateUp(SteamVR_Input_Sources.LeftHand))
            {
                Debug.Log("openLeftHand");
                behaviorController.runBehavior("handcontroller-dc72e7/openLeftHand");
            }
        }
    }
}
