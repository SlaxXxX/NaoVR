using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetOffset : MonoBehaviour
{
    public Vector3 CartesianOffset;
    public Vector3 RotationalOffset;
    public bool DoCalculateOnCalibration;

    public Transform GetOffset()
    {
        return gameObject.transform;
    }
}
