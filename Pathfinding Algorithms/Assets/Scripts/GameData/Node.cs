using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public enum NodeType
{
    Open = 0,
    Blocked = 1,
    LightTerrain = 2,
    MediumTerrain = 3,
    HeavyTerrain = 4
}

public class Node: IComparable<Node>
{
    public NodeType nodeType = NodeType.Open;

    public int xIndex = -1;
    public int yIndex = -1;

    public Vector3 position;
    public float distanceTraveled = Mathf.Infinity;

    public List<Node> neighbors = new List<Node>();
    public Node previousNode = null;

    public float priority;

    /// <summary>
    /// Constructor to set up node
    /// </summary>
    /// <param name="xIndex"></param>
    /// <param name="yIndex"></param>
    /// <param name="nodeType"></param>
    public Node(int xIndex, int yIndex, NodeType nodeType)
    {
        this.xIndex = xIndex;
        this.yIndex = yIndex;
        this.nodeType = nodeType;
    }

    /// <summary>
    /// Compare one node with another
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public int CompareTo(Node other)
    {
        if(this.priority < other.priority)
        {
            return -1;
        }
        else if (this.priority > other.priority)
        {
            return 1;
        }
        else
        {
            return 0;
        }
    }

    /// <summary>
    /// Reset this node's previous node to null
    /// </summary>
    public void Reset()
    {
        previousNode = null;
    }

}
