using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// force the graph view game object to also have a graph script attached
[RequireComponent(typeof(Graph))]
public class GraphView : MonoBehaviour {

    public GameObject nodeViewPrefab;
    public Color baseColor = Color.white;
    public Color wallColor = Color.black;

    /// <summary>
    /// Initialize each node and color it properly
    /// </summary>
    /// <param name="graph"></param>
    public void Init(Graph graph)
    {
        if (graph != null)
        {
            foreach (Node n in graph.nodes)
            {
                GameObject instance = Instantiate(nodeViewPrefab, Vector3.zero, Quaternion.identity);
                NodeView nodeView = instance.GetComponent<NodeView>();

                if (nodeView != null)
                {
                    nodeView.Init(n);

                    if(n.nodeType == NodeType.Blocked)
                    {
                        nodeView.ColorNode(wallColor);
                    }
                    else
                    {
                        nodeView.ColorNode(baseColor);
                    }
                }
            }

        }
        else
        {
            Debug.LogWarning("GraphView: No graph to initialize");
            return;
        }
    }
    
}
