using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoseSolver : MonoBehaviour
{
    public GameObject SegmentPrefab;
    public bool Calibrated = false;
    private GameObject[] nodeInstances, segmentInstances;
    private List<List<GameObject>> arms = new List<List<GameObject>>();

    void Start()
    {
        NodeData[] nodeData = gameObject.GetComponentsInChildren<NodeData>();
        nodeInstances = new GameObject[nodeData.Length];
        segmentInstances = new GameObject[nodeData.Length];

        for (int i = 0; i < nodeData.Length; i++)
        {
            nodeInstances[i] = nodeData[i].gameObject;
            if (nodeInstances[i].TryGetComponent<IKHook>(out _))
            {
                List<GameObject> arm = new List<GameObject>();
                arm.Add(nodeInstances[i]);
                while (arm.Last().GetComponent<NodeData>().Parent != null)
                {
                    arm.Add(arm.Last().GetComponent<NodeData>().Parent);
                }
                arms.Add(arm);
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
        if (Calibrated)
        {
            arms.ForEach(SolveArms);
            UpdateSegments();
        }
    }

    void SolveArms(List<GameObject> nodes)
    {
        nodes[0].GetComponent<IKHook>().UpdateHook();

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
}
