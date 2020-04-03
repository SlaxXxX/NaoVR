using RosSharp.RosBridgeClient;
using UnityEngine;
using RosSharp.RosBridgeClient.Services;

namespace NaoApi.Stiffness
{
    public class StiffnessControler : MonoBehaviour
    {
        private RosSocket socket;
        public bool stiffness;
        private Speech.SpeechControler speech;
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
                    speech.say("stiffnes disabled");
                }
                if (!stiffness)
                {
                    socket.CallService<enableStiffnessRequest, enableStiffnessResponse>("/nao_robot/pose/body_stiffness/enable", ServiceCallHandler, new enableStiffnessRequest());
                    stiffness = true;
                    speech.say("stiffnes Enabled");
                }
            }
        }

        private static void ServiceCallHandler(disableStiffnessResponse message)
        {

        }
        private static void ServiceCallHandler(enableStiffnessResponse message)
        {

        }
    }
}