using RosSharp.RosBridgeClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using std_msgs = RosSharp.RosBridgeClient.Messages.Standard;

public class DiagnosticConnector : MonoBehaviour
{
    public RosSocket socket;
    // Start is called before the first frame update
    void Start()
    {
        socket = GetComponentInParent<RosConnector>()?.RosSocket;
        // Subscription:
       // string subscription_id = socket.Subscribe<DiagnosticArray>("/diagnostics", SubscriptionHandler);
       // subscription_id = socket.Subscribe<DiagnosticArray>("/diagnostics", SubscriptionHandler);
    }

    /*private static void SubscriptionHandler(DiagnosticArray message)
    {
        Console.WriteLine((message).data);
    }*/

    // Update is called once per frame
    void Update()
    {
        
    }
}
