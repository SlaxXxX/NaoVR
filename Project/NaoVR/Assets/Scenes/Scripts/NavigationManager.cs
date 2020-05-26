using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class NavigationManager : StateListener
{
    private Dictionary<SteamVR_Action_Boolean, Action> actionMap;
    private bool isStanding = true;

    void Start()
    {
        actionMap = new Dictionary<SteamVR_Action_Boolean, Action>()
        {
            { SteamVR_Actions._default.WalkForward, TestMethod },
            { SteamVR_Actions._default.WalkBackward, TestMethod },
            { SteamVR_Actions._default.TurnLeft, TestMethod },
            { SteamVR_Actions._default.TurnRight, TestMethod },
            { SteamVR_Actions._default.CrouchStand, TestMethod },
        };
        Register();
    }


    void Update()
    {
        if (state == StateManager.State.armed)
        {
            foreach(KeyValuePair<SteamVR_Action_Boolean, Action> pair in actionMap)
            {
                if (pair.Key.GetStateDown(SteamVR_Input_Sources.Any))
                    pair.Value.Invoke();
            }
        }
    }

    public void TestMethod()
    {

    }
}
