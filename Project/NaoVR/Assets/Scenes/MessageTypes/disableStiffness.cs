/*
This message class is generated automatically with 'ServiceMessageGenerator' of ROS#
*/

using Newtonsoft.Json;
using RosSharp.RosBridgeClient.Messages.Geometry;
using RosSharp.RosBridgeClient.Messages.Navigation;
using RosSharp.RosBridgeClient.Messages.Sensor;
using RosSharp.RosBridgeClient.Messages.Standard;
using RosSharp.RosBridgeClient.Messages.Actionlib;

namespace RosSharp.RosBridgeClient.Services
{
    public class disableStiffnessRequest : Message
    {
        [JsonIgnore]
        public const string RosMessageName = "/nao_robot/pose/body_stiffness/disable";

        public disableStiffnessRequest()
        {

        }
    }
    public class disableStiffnessResponse : Message
    {
        [JsonIgnore]
        public const string RosMessageName = "/nao_robot/pose/body_stiffness/disable";

    }
}


