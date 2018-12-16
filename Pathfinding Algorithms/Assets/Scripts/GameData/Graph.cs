using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour {

    public Node[,] nodes;

    public List<Node> wallNodes = new List<Node>();

    int[,] m_mapData;
    int m_width;
    int m_height;

    public int Width { get { return m_width; } }
    public int Height {  get { return m_height; } }

    /// <summary>
    /// Every direction to check for neighbors: N, S, E, W, NE, SE, NW, SW
    /// </summary>
    public static readonly Vector2[] allDirections =
    {
        new Vector2(0f, 1f),
        new Vector2(1f, 1f),
        new Vector2(1f, 0f),
        new Vector2(1f, -1f),
        new Vector2(0f, -1f),
        new Vector2(-1f, -1f),
        new Vector2(-1f, 0f),
        new Vector2(-1f, 1f)
    };

    /// <summary>
    /// Initialize the map data and positions of nodes and types
    /// </summary>
    /// <param name="mapData"></param>
    public void Init(int[,] mapData)
    {
        m_mapData = mapData;
        m_width = mapData.GetLength(0);
        m_height = mapData.GetLength(1);

        nodes = new Node[m_width, m_height];

        for(int y = 0; y < m_height; y++)
        {
            for (int x = 0; x < m_width; x++)
            {
                // cast the mapData position (which will be 1 or 0) to a NodeType
                NodeType type = (NodeType)mapData[x, y];
                Node newNode = new Node(x, y, type);
                nodes[x, y] = newNode;

                newNode.position = new Vector3(x, 0, y);

                // if this new node is of blocked type, then add it to the walls list
                if(type == NodeType.Blocked)
                {
                    wallNodes.Add(newNode);
                }
            }
        }

        for(int y = 0; y < m_height; y++)
        {
            for(int x = 0; x < m_width; x++)
            {
                if(nodes[x,y].nodeType != NodeType.Blocked)
                {
                    nodes[x, y].neighbors = GetNeighbors(x, y);
                }
            }
        }
    }

    /// <summary>
    /// Check if the position is within the bounds
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool IsWithinBounds(int x, int y)
    {
        return (x >= 0 && x < m_width && y >= 0 && y < m_height);
    }

    /// <summary>
    /// Create and return a list of all neighboring nodes
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="nodeArray"></param>
    /// <param name="directions"></param>
    /// <returns></returns>
    List<Node> GetNeighbors(int x, int y, Node[,] nodeArray, Vector2[] directions)
    {
        List<Node> neighborNodes = new List<Node>();
        foreach (Vector2 dir in directions)
        {
            int newX = x + (int)dir.x;
            int newY = y + (int)dir.y;

            // if the position is within bounds, is not null, and is not a wall
            if(IsWithinBounds(newX, newY) && nodeArray[newX, newY] != null &&
                nodeArray[newX, newY].nodeType != NodeType.Blocked)
            {
                neighborNodes.Add(nodeArray[newX, newY]);
            }
        }
        return neighborNodes;
    }

    /// <summary>
    /// Get the neighboring nodes
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    List<Node> GetNeighbors(int x, int y)
    {
        return GetNeighbors(x, y, nodes, allDirections);
    }

}
