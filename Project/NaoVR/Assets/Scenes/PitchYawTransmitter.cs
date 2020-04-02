using RosSharp.RosBridgeClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitchYawTransmitter : MonoBehaviour
{
    private bool isActive = false;
    public GameObject pitch, yaw;

    JointStateWriter yawWriter, pitchWriter;
    // Start is called before the first frame update
    void Start()
    {
        yawWriter = yaw.GetComponent<JointStateWriter>();
        if (yawWriter == null)
            Debug.Log("No Writer Component found in Module " + transform.parent.name);

        pitchWriter = pitch.GetComponent<JointStateWriter>();
        if (pitchWriter == null)
            Debug.Log("No Writer Component found in Module " + transform.parent.name);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            isActive = !isActive;
        }

        if (isActive)
        {
            WriteValue("Yaw", yawWriter, transform.rotation.y);
            WriteValue("Pitch", pitchWriter, transform.rotation.x);
        }
    }

    void WriteValue(string name, JointStateWriter writer, float value)
    {
        writer?.Write(value);
        Debug.Log(name + " rotated to " + value);
    }
}
