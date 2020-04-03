using RosSharp.RosBridgeClient;
using System;
using UnityEngine;
using rosapi = RosSharp.RosBridgeClient.Services.RosApi;
using RosSharp.RosBridgeClient.Services;

public class StiffnessControler : MonoBehaviour
{
    private RosSocket socket;
    public bool stiffness;
    // Start is called before the first frame update
    void Start()
    {
        GameObject Connector = GameObject.FindWithTag("Connector");
        socket = Connector.GetComponent<RosConnector>()?.RosSocket;
        socket.CallService<disableStiffnessRequest, disableStiffnessResponse>("/nao_robot/pose/body_stiffness/disable", ServiceCallHandler, new disableStiffnessRequest());
        stiffness = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (stiffness)
            {
                socket.CallService<disableStiffnessRequest, disableStiffnessResponse>("/nao_robot/pose/body_stiffness/disable", ServiceCallHandler, new disableStiffnessRequest());
                stiffness = false;
            }
            if (!stiffness)
            {
                socket.CallService<rosapi.GetParamRequest, rosapi.GetParamResponse>("/nao_robot/pose/body_stiffness/enable", ServiceCallHandler, new rosapi.GetParamRequest("/enable", "default"));
                stiffness = true;
            }
        }
    }

    private static void ServiceCallHandler(disableStiffnessResponse message)
    {

    }
    private static void ServiceCallHandler(rosapi.GetParamResponse message)
    {

    }
}
