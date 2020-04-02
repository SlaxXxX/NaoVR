using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveIt : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
   
    }

    // Update is called once per frame
    void Update()
    {
        GameObject Neck = GameObject.FindGameObjectWithTag("Neck");
        GameObject Head = GameObject.FindGameObjectWithTag("Head");

        string name = "";
        float position = 0;
        float velocity = 0;
        float effort = 0;
        Neck.GetComponent<RosSharp.RosBridgeClient.JointStateReader>().Read(out name, out position, out velocity, out effort);
        float oldState_Neck = position;
        Head.GetComponent<RosSharp.RosBridgeClient.JointStateReader>().Read(out name, out position, out velocity, out effort);
        float oldState_Head = position;
        if (Input.GetAxis("Horizontal") < 0)
        {
            Neck.GetComponent<RosSharp.RosBridgeClient.JointStateWriter>().Write(oldState_Neck + 0.003f);
        }
        else if (Input.GetAxis("Horizontal") > 0)
        {
            Neck.GetComponent<RosSharp.RosBridgeClient.JointStateWriter>().Write(oldState_Neck - 0.003f);
        }
        if (Input.GetAxis("Vertical") < 0)
        {
            Head.GetComponent<RosSharp.RosBridgeClient.JointStateWriter>().Write(oldState_Head - 0.003f);
        }
        else if (Input.GetAxis("Vertical") > 0)
        {
            Head.GetComponent<RosSharp.RosBridgeClient.JointStateWriter>().Write(oldState_Head + 0.003f);
        }
    }
}
