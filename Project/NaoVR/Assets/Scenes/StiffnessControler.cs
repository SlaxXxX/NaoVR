using RosSharp.RosBridgeClient;
using UnityEngine;
using RosSharp.RosBridgeClient.Services;

namespace NaoApi.Stiffness
{
    public class StiffnessControler : MonoBehaviour
    {
        private RosSocket socket;
        public bool stiffness;
        //private Speech.SpeechControler speech;
        void Start()
        {
            GameObject Connector = GameObject.FindWithTag("Connector");
            socket = Connector.GetComponent<RosConnector>()?.RosSocket;
            disableStiffness();
        }
        void Update()
        {//just for debugging remove later
            if (Input.GetKeyDown(KeyCode.F1))
            {
                if (stiffness)
                {
                    disableStiffness();
                }
                if (!stiffness)
                {
                    enableStiffness();
                }
            }
        }
        //requert only for references
        private static void ServiceCallHandler(disableStiffnessResponse message){}
        //requert only for references
        private static void ServiceCallHandler(enableStiffnessResponse message){}
        //requert only for references
        private static void ServiceCallHandler(wakeupResponse message){}

        public void enableStiffness() {
            socket.CallService<enableStiffnessRequest, enableStiffnessResponse>("/nao_robot/pose/body_stiffness/enable", ServiceCallHandler, new enableStiffnessRequest());
            stiffness = true;
            //speech.say("stiffnes Enabled");
        }
        public void disableStiffness()
        {
            socket.CallService<disableStiffnessRequest, disableStiffnessResponse>("/nao_robot/pose/body_stiffness/disable", ServiceCallHandler, new disableStiffnessRequest());
            stiffness = false;
            //speech.say("stiffnes disabled");
        }
        public void wakeup()
        {
            socket.CallService<wakeupRequest, wakeupResponse>("/nao_robot/pose/wakeup", ServiceCallHandler, new wakeupRequest());
            stiffness = true;
            //speech.say("stiffnes Enabled");
        }
    }
}