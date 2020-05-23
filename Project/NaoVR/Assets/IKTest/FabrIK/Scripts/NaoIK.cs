using Assets.IKTest.FabrIK.Scripts;
using NaoApi.Stiffness;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Valve.VR;

public class NaoIK : CalibrationListener
{
    private SteamVR_Action_Boolean checkConstraints;


    public GameObject SegmentPrefab;
    public bool IsCalibrated = false, AlwaysCheckConstraints = false, SendJoints = false, DrawDebugText = true;
    public StiffnessController stiffnessController;

    private bool isArmed = false;
    private GameObject[] nodeInstances, segmentInstances;
    private List<List<GameObject>> hookedNodeChains = new List<List<GameObject>>();

    public int maxIterations = 10;
    public float acceptableDistance = 0.02f;
    private float minStep;

    void Start()
    {
        Register();

        checkConstraints = SteamVR_Actions._default.GrabPinch;

        minStep = acceptableDistance / 10;

        NodeData[] nodeData = gameObject.GetComponentsInChildren<NodeData>();
        nodeInstances = new GameObject[nodeData.Length];
        segmentInstances = new GameObject[nodeData.Length];

        for (int i = 0; i < nodeData.Length; i++)
        {
            nodeInstances[i] = nodeData[i].gameObject;
            if (nodeInstances[i].TryGetComponent<IKHook>(out _))
            {
                List<GameObject> nodeChain = new List<GameObject>();
                nodeChain.Add(nodeInstances[i]);
                while (nodeChain.Last().GetComponent<NodeData>().Parent != null)
                {
                    nodeChain.Add(nodeChain.Last().GetComponent<NodeData>().Parent);
                }
                //Remove shoulderbase
                nodeChain.RemoveAt(nodeChain.Count - 1);
                hookedNodeChains.Add(nodeChain);
            }
            if (nodeData[i].Parent != null)
            {
                segmentInstances[i] = Instantiate(SegmentPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                SegmentData data = segmentInstances[i].GetComponent<SegmentData>();
                data.StartNode = nodeData[i].Parent;
                data.EndNode = nodeData[i].gameObject;
                segmentInstances[i].transform.localScale = new Vector3(1, 1, (data.EndNode.transform.position - data.StartNode.transform.position).magnitude);
            }
        }
        UpdateSegments();
    }

    void Update()
    {
        if (IsCalibrated)
        {
            hookedNodeChains.ForEach(ApplyFabrIK);
            if (false && (AlwaysCheckConstraints || checkConstraints.GetStateDown(SteamVR_Input_Sources.Any)))
                hookedNodeChains.ForEach(ApplyConstraints);
            if (SendJoints && isArmed)
                hookedNodeChains.ForEach(SendJointAngles);
            UpdateSegments();
        }
    }

    private void OnApplicationQuit()
    {
        stiffnessController.disableStiffness();
    }

    void ApplyFabrIK(List<GameObject> nodes)
    {
        GameObject effector = nodes[0];
        int cycle = 0;
        effector.GetComponent<IKHook>().UpdateHook();
        Vector3 startPos = effector.transform.position;
        //Quaternion startRot = effector.transform.rotation;
        do
        {
            Vector3 before = effector.transform.position;
            effector.transform.position = startPos;
            //effector.transform.rotation = startRot;
            DoFabrIKCycle(nodes);
            if (Vector3.Distance(before, effector.transform.position) < minStep)
                break;
            cycle++;
        } while (cycle < maxIterations
            && (Vector3.Distance(effector.transform.position, startPos)) > acceptableDistance);
        RotateNodesCorrectly(nodes);
    }

    void DoFabrIKCycle(List<GameObject> nodes)
    {
        Vector3 basePosition = nodes.Last().transform.position;
        //Starts at elbow because hand position gets set externally
        //Ends at elbow because shoulder is fixed point
        ApplyFabrIKToNodes(nodes, 0, nodes.Count - 1, 1);
        nodes.Last().transform.position = basePosition;
        ApplyFabrIKToNodes(nodes, nodes.Count - 1, -1, -1);
    }

    void ApplyFabrIKToNodes(List<GameObject> nodes, int start, int end, int delta)
    {
        for (int i = start + delta; i != end; i += delta)
        {
            float distance = delta == 1 ? nodes[i - 1].GetComponent<NodeData>().GetDistance() : nodes[i].GetComponent<NodeData>().GetDistance();
            Vector3 last = nodes[i - delta].transform.position;
            nodes[i].transform.position = last + (nodes[i].transform.position - last).normalized * distance;
            //ApplyConstraints(nodes[i], nodes[i - delta]);
        }
    }

    void RotateNodesCorrectly(List<GameObject> nodes)
    {
        for (int i = nodes.Count - 1; i >= 0; i--)
        {

            NodeData nodeData = nodes[i].GetComponent<NodeData>();

            GameObject parent = nodeData.Parent;
            GameObject current = nodes[i];
            GameObject child = i > 0 ? nodes[i - 1] : null;

            Vector3 parentForward = parent.transform.TransformDirection(Vector3.forward);
            Vector3 parentUp = parent.transform.TransformDirection(Vector3.up);

            float roll = 0, pitch = 0;
            if (child != null)
            {

                Vector3 childDirection = (child.transform.position - current.transform.position).normalized;

                current.transform.rotation = parent.transform.rotation;

                roll = parentUp.GetAngleOnAxis(childDirection, parentForward);
                current.transform.Rotate(Vector3.forward, roll);

                pitch = parentForward.GetAngleOnAxis(childDirection, current.transform.TransformDirection(Vector3.right));
                current.transform.Rotate(Vector3.right, pitch);

            }
            else
            {
                //special case for wrists
                roll = parentUp.GetAngleOnAxis(current.transform.up, parentForward);
            }

            nodeData.SetRotationRaw(pitch, roll);

            if (DrawDebugText && nodeData.DebugText != null)
            {
                nodeData.DebugText.text = $"Pitch: {pitch.ToString("F1")}\nRoll: {roll.ToString("F1")}";
            }
        }
    }

    void SendJointAngles(List<GameObject> nodes)
    {
        for (int i = nodes.Count - 1; i >= 0; i--)
        {
            nodes[i].GetComponent<NodeData>().WriteJointData(i > 0 ? nodes[i - 1] : null);
        }
    }

    void ApplyConstraints(List<GameObject> nodes)
    {
        for (int i = nodes.Count - 1; i >= 0; i--)
        {
            //if the node has a child
            if (i > 0)
            {
                NodeData nodeData = nodes[i].GetComponent<NodeData>();

                Vector3 parentForward = i < nodes.Count - 1 ? nodes[i + 1].transform.TransformDirection(Vector3.forward) : Vector3.right;
                Vector3 parentUp = i < nodes.Count - 1 ? nodes[i + 1].transform.TransformDirection(Vector3.up) : Vector3.up;
                Vector3 uncheckedDirection = (nodes[i - 1].transform.position - nodes[i].transform.position).normalized;

                float yRot = Vector3.SignedAngle(parentForward, uncheckedDirection, parentUp);
                float zRot = Vector3.SignedAngle(parentUp, uncheckedDirection, parentForward);

                float yCorrection = Mathf.Min(0, nodeData.YBounds.y - yRot) + Mathf.Max(0, nodeData.YBounds.x - yRot);
                float zCorrection = Mathf.Min(0, nodeData.ZBounds.y - zRot) + Mathf.Max(0, nodeData.ZBounds.x - zRot);

                nodes[i - 1].transform.RotateAround(nodes[i].transform.position, parentUp, yCorrection);
                nodes[i - 1].transform.RotateAround(nodes[i].transform.position, parentForward, zCorrection);
                //nodes[i - 1].transform.position = nodes[i].transform.position + (nodes[i - 1].transform.position - nodes[i].transform.position).normalized * nodeData.GetDistance();

                if (i < nodes.Count - 1)
                    nodes[i].transform.rotation = nodes[i + 1].transform.rotation;
                else
                    nodes[i].transform.rotation = Quaternion.Euler(Vector3.right);

                nodes[i].transform.Rotate(0, 0, zRot + zCorrection, Space.Self);
                nodes[i].transform.Rotate(0, yRot + yCorrection, 0, Space.Self);

                Debug.Log($"{i} to {i - 1}: ({yRot}, {zRot}; {yCorrection}, {zCorrection})");

                /* THIS SHIT AINT WORKIN

                //Get x, y and z angle of the direction to compare with restrictions set in NodeData
                float xRot = Vector3.SignedAngle(zeroAngle.RemoveX(), uncheckedDirection.RemoveX(), Vector3.right);
                float yRot = Vector3.SignedAngle(zeroAngle.RemoveY(), uncheckedDirection.RemoveY(), Vector3.up);
                float zRot = Vector3.SignedAngle(zeroAngle.RemoveZ(), uncheckedDirection.RemoveZ(), Vector3.forward);

                //Debug.Log($"{i} to {i - 1}: ({ xRot},{ yRot},{ zRot})");

                //Store the amount the child is out of the parents constraints in degrees
                xRot = Mathf.Min(0, nodeData.XBounds.y - xRot) + Mathf.Max(0, nodeData.XBounds.x - xRot);
                yRot = Mathf.Min(0, nodeData.YBounds.y - yRot) + Mathf.Max(0, nodeData.YBounds.x - yRot);
                zRot = Mathf.Min(0, nodeData.ZBounds.y - zRot) + Mathf.Max(0, nodeData.ZBounds.x - zRot);

                //Move child to obey constraints
                nodes[i - 1].transform.RotateAround(nodes[i].transform.position, nodes[i].transform.TransformDirection(Vector3.right), xRot);
                nodes[i - 1].transform.RotateAround(nodes[i].transform.position, nodes[i].transform.TransformDirection(Vector3.up), yRot);
                //nodes[i - 1].transform.RotateAround(nodes[i].transform.position, nodes[i].transform.TransformDirection(Vector3.forward), zRot);

                //Once the direction is consistent, rotate node to match next segment
                nodes[i].transform.LookAt(nodes[i - 1].transform);

                //if the node is not an endpoint of either side, rotate it to be inside the plane of parent and child
                if (i < nodes.Count - 1)
                {

                }

            */
            }
        }
    }
    void ApplyConstraints(GameObject current, GameObject last)
    {
        float xRot, yRot, zRot;

        Vector3 lastDirection = last.transform.TransformDirection(Vector3.back);
        Vector3 linkDirection = (current.transform.position - last.transform.position).normalized;
        NodeData nodeData = last.GetComponent<NodeData>();

        xRot = Vector3.Angle(lastDirection.RemoveX(), linkDirection.RemoveX());
        yRot = Vector3.Angle(lastDirection.RemoveY(), linkDirection.RemoveY());
        zRot = Vector3.Angle(lastDirection.RemoveZ(), linkDirection.RemoveZ());

        xRot = Mathf.Min(0, nodeData.XBounds.y - xRot) + Mathf.Max(0, nodeData.XBounds.x - xRot);
        yRot = Mathf.Min(0, nodeData.YBounds.y - yRot) + Mathf.Max(0, nodeData.YBounds.x - yRot);
        zRot = Mathf.Min(0, nodeData.ZBounds.y - zRot) + Mathf.Max(0, nodeData.ZBounds.x - zRot);

        current.transform.RotateAround(last.transform.position, last.transform.TransformDirection(Vector3.right), xRot);
        current.transform.RotateAround(last.transform.position, last.transform.TransformDirection(Vector3.up), yRot);
        current.transform.RotateAround(last.transform.position, last.transform.TransformDirection(Vector3.forward), zRot);

        current.transform.LookAt(last.transform);
    }

    void UpdateSegments()
    {
        foreach (GameObject segment in segmentInstances)
        {
            if (segment == null)
                continue;
            SegmentData data = segment.GetComponent<SegmentData>();
            segment.transform.position = data.StartNode.transform.position;
            segment.transform.rotation = Quaternion.LookRotation(data.EndNode.transform.position - data.StartNode.transform.position);
        }
    }

    public override void Calibrated()
    {
        IsCalibrated = true;
    }

    public override void SetArmed(bool isArmed)
    {
        this.isArmed = isArmed;
        if (!isArmed)
            stiffnessController.wakeup();
    }
}
