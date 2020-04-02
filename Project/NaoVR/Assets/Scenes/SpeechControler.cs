using RosSharp.RosBridgeClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using rosapi = RosSharp.RosBridgeClient.MessageTypes.Rosapi;

public class SpeechControler : MonoBehaviour
{
    public RosSocket socket;
    // Start is called before the first frame update
    void Start()
    {
        socket = GetComponentInParent<RosConnector>()?.RosSocket;
        socket.CallService<rosapi.GetParamRequest, rosapi.GetParamResponse>("/rosapi/get_param", ServiceCallHandler, new rosapi.GetParamRequest("/rosdistro", "default"));
    }
}
