using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class CalibrateKinematicModel : MonoBehaviour
{
    private SteamVR_Action_Boolean doCalibrate;

    public GameObject CalibrateObject, CalibrationReference, CalibrationLink;

    void Start()
    {
        doCalibrate = SteamVR_Actions._default.GrabGrip;
    }
    void Update()
    {
        if (doCalibrate.GetStateDown(SteamVR_Input_Sources.Any))
        {
            float scaleFactor = CalibrationReference.transform.position.y / CalibrationLink.transform.position.y;
            Debug.Log($"Calibrating... Reference is at {CalibrationReference.transform.position.y}, Link is at {CalibrationLink.transform.position.y}. Resulting scale is {scaleFactor}");
            CalibrateObject.transform.localScale = scaleFactor * CalibrateObject.transform.localScale;
            gameObject.GetComponent<NaoIK>().Calibrated = true;
        }
    }
}
