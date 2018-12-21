using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeType
{
    Open = 0,
    Blocked = 1
}

public class Node
{
    public NodeType nodeType = NodeType.Open;

    public int xIndex = -1;
    public int yIndex = -1;

    public Vector3 position;

    public List<Node> neighbors = new List<Node>();
    public Node previousNode = null;

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
    /// Reset this node's previous node to null
    /// </summary>
    public void Reset()
    {
        previousNode = null;
    }

}
