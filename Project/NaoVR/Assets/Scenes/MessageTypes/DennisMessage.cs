/*
This message class is generated automatically with 'SimpleMessageGenerator' of ROS#
*/ 

using Newtonsoft.Json;
using RosSharp.RosBridgeClient.Messages.Standard;

namespace RosSharp.RosBridgeClient.Messages
{
public class DennisMessage : Message
{
[JsonIgnore]
public const string RosMessageName = "naoqi_bridge_msgs/JointAnglesWithSpeed";

public Header header;
public string[] joint_names;
public float[] joint_angles;
public float speed;

public DennisMessage()
{
header = new Header();
joint_names = new string[0];
joint_angles = new float[0];
speed = new float();
}
}
}

