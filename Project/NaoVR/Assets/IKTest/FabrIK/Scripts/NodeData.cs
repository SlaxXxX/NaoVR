using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeData : MonoBehaviour
{
    public GameObject Parent;
    public Vector2 XBounds, YBounds, ZBounds;
    private float distance;

    public float GetDistance()
    {
        return distance;
    }

    void Start()
    {
        if (Parent != null)
        {
            distance = Vector3.Distance(Parent.transform.position, gameObject.transform.position);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
