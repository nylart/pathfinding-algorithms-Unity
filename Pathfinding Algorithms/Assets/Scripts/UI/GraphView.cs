using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// force the graph view game object to also have a graph script attached
[RequireComponent(typeof(Graph))]
public class GraphView : MonoBehaviour {

    public GameObject nodeViewPrefab;
    public NodeView[,] nodeViews;

    /// <summary>
    /// Initialize each node and color it properly
    /// </summary>
    /// <param name="graph"></param>
    public void Init(Graph graph)
    {
        if (graph == null)
        {
            Debug.LogWarning("GraphView: No graph to initialize!");
            return;
        }
        nodeViews = new NodeView[graph.Width, graph.Height];

        foreach (Node n in graph.nodes)
        {
            GameObject instance = Instantiate(nodeViewPrefab, Vector3.zero, Quaternion.identity);
            NodeView nodeView = instance.GetComponent<NodeView>();

            if (nodeView != null)
            {
                nodeView.Init(n);
                nodeViews[n.xIndex, n.yIndex] = nodeView;

                Color originalColor = MapData.GetColorFromNodeType(n.nodeType);
                nodeView.ColorNode(originalColor);
            }
        }
    }

    /// <summary>
    /// Color the nodes 
    /// </summary>
    /// <param name="nodes"></param>
    /// <param name="color"></param>
    public void ColorNodes(List<Node> nodes, Color color, bool lerpColor = false, float lerpValue = 0.5f)
    {
        foreach (Node n in nodes)
        {
            if (n != null)
            {
                NodeView nodeView = nodeViews[n.xIndex, n.yIndex];
                Color newColor = color;

                if (lerpColor)
                {
                    Color originalColor = MapData.GetColorFromNodeType(n.nodeType);
                    newColor = Color.Lerp(originalColor, newColor, lerpValue);
                }

                if(nodeView != null)
                {
                    nodeView.ColorNode(newColor);
                }
            }
        }
    }

    #region Arrows
    /// <summary>
    /// Show arrow for one node
    /// </summary>
    /// <param name="node"></param>
    public void ShowNodeArrows(Node node, Color color)
    {
        if (node != null)
        {
            NodeView nodeView = nodeViews[node.xIndex, node.yIndex];
            if(nodeView != null)
            {
                nodeView.ShowArrow(color);
            }
        }
    }
    
    /// <summary>
    /// Show arrow for a list of nodes
    /// </summary>
    /// <param name="nodes"></param>
    public void ShowNodeArrows(List<Node> nodes, Color color)
    {
        foreach (Node n in nodes)
        {
            ShowNodeArrows(n, color);
        }
    }
    #endregion
}
