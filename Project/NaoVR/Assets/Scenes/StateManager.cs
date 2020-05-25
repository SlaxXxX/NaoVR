﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using static StateManager;

public abstract class StateListener : MonoBehaviour
{
    public StateManager stateManager;
    protected State state => stateManager.GetState();

    protected void Register()
    {
        stateManager.RegisterListener(this);
    }

    public virtual void StateChanged(State newState)
    {
    }
}

public abstract class Predicate : MonoBehaviour
{
    public abstract bool CriteriaMet();
}

public class StateManager : MonoBehaviour
{
    public Predicate HeadCheck, LeftHandCheck, RightHandCheck;
    public enum State { init, positioned, calibrated, disarmed, armed }
    private State state = State.init;

    private List<StateListener> listeners = new List<StateListener>();
    private SteamVR_Action_Boolean calibrate,arm;
    public void RegisterListener(StateListener self)
    {
        listeners.Add(self);
    }

    private void ChangeState(State newState)
    {
        state = newState;
        listeners.ForEach(l => l.StateChanged(state));
    }

    public State GetState()
    {
        return state;
    }

    // Start is called before the first frame update
    void Start()
    {
        calibrate = SteamVR_Actions._default.Calibrate;
        arm = SteamVR_Actions._default.Arm;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.init && HeadCheck.CriteriaMet())
            ChangeState(State.positioned);
        if (calibrate.GetStateDown(SteamVR_Input_Sources.Any) && (state >= State.positioned && state <= State.disarmed))
            ChangeState(State.calibrated);
        if (arm.GetStateDown(SteamVR_Input_Sources.Any) && state >= State.calibrated)
        {
            if (state < State.armed && LeftHandCheck.CriteriaMet() && RightHandCheck.CriteriaMet())
                ChangeState(State.armed);
            else
                ChangeState(State.disarmed);
        }
    }
}
