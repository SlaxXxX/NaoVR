using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public abstract class CalibrationListener : MonoBehaviour
{
    public CalibrateKinematicModel calibrator;

    protected void Register()
    {
        calibrator.RegisterListener(this);
    }
    public abstract void Calibrated();
    public abstract void SetArmed(bool isArmed);
}

public class CalibrateKinematicModel : MonoBehaviour
{
    private List<CalibrationListener> listeners = new List<CalibrationListener>();
    private SteamVR_Action_Boolean doCalibrate;
    private bool isArmed, isCalibrated;

    public GameObject CalibrateObject, CalibrationReference, CalibrationLink;

    public void RegisterListener(CalibrationListener self)
    {
        listeners.Add(self);
    }

    void Start()
    {
        doCalibrate = SteamVR_Actions._default.GrabGrip;
    }
    void Update()
    {
        if (isCalibrated && doCalibrate.GetStateDown(SteamVR_Input_Sources.LeftHand))
        {
            isArmed = !isArmed;
            listeners.ForEach(l => l.SetArmed(isArmed));
        }
        if (!isCalibrated && doCalibrate.GetStateDown(SteamVR_Input_Sources.Any) || isCalibrated && doCalibrate.GetStateDown(SteamVR_Input_Sources.RightHand))
            Calibrate();
    }

    void Calibrate()
    {
        isCalibrated = true;
        float scaleFactor = CalibrationReference.transform.position.y / CalibrationLink.transform.position.y;
        Debug.Log($"Calibrating... Reference is at {CalibrationReference.transform.position.y}, Link is at {CalibrationLink.transform.position.y}. Resulting scale is {scaleFactor}");
        CalibrateObject.transform.localScale = scaleFactor * CalibrateObject.transform.localScale;

        listeners.ForEach(l => l.Calibrated());
    }
}
