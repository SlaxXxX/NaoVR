using RosSharp.RosBridgeClient;
using UnityEngine;
using std_msgs = RosSharp.RosBridgeClient.Messages.Standard;

public class SpeechControler : MonoBehaviour
{
    private RosSocket socket;
    private string publication_id;
    private std_msgs.String message;
    // Start is called before the first frame update
    void Start()
    {
        GameObject Connector = GameObject.FindWithTag("Connector");
        socket = Connector.GetComponent<RosConnector>()?.RosSocket;
        publication_id = socket.Advertise<std_msgs.String>("/speech");
        message = new std_msgs.String();
        message.data = "Hallo, ich bin Mino. Du bist erfolgreich verbunden.";
        socket.Publish(publication_id, message);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            message.data = "Los geht's.";
            socket.Publish(publication_id, message);
        }
    }

    void say(string text)
    {
        message.data = text;
        socket.Publish(publication_id, message);
    }
}
