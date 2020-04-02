using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class debugLog : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            GameObject Neck = GameObject.FindGameObjectWithTag("Neck");
            GameObject Head = GameObject.FindGameObjectWithTag("Head");

            string name = "";
            float position = 0;
            float velocity = 0;
            float effort = 0;
            Head.GetComponent<RosSharp.RosBridgeClient.JointStateReader>().Read(out name, out position, out velocity, out effort);
            Debug.Log(name + ": " + position + "; " + velocity + "; " + effort);
        }
    }
}
