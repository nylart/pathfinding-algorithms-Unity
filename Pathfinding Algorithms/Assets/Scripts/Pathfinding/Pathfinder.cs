using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pathfinder : MonoBehaviour {

    Node m_startNode;
    Node m_goalNode;

    Graph m_graph;
    GraphView m_graphView;

    Queue<Node> m_frontierNodes;
    List<Node> m_exploredNodes;
    List<Node> m_pathNodes;

    // Colors for nodes
    public Color startColor = Color.green;
    public Color goalColor = Color.red;
    public Color frontierColor = Color.yellow;
    public Color exploredColor = Color.grey;
    public Color pathColor = Color.cyan;

    public bool isComplete = false;
    int m_iterations = 0;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="graph"></param>
    /// <param name="graphView"></param>
    /// <param name="start"></param>
    /// <param name="goal"></param>
    public void Init(Graph graph, GraphView graphView, Node start, Node goal)
    {
        // Test for errors
        if (start == null || goal == null || graph == null || graphView == null)
        {
            Debug.LogWarning("Pathfinder: Missing component(s)");
            return;
        }

        if (start.nodeType == NodeType.Blocked || goal.nodeType == NodeType.Blocked)
        {
            Debug.LogWarning("Pathfinder: Start and goal nodes must be unblocked");
            return;
        }

        m_graph = graph;
        m_graphView = graphView;
        m_startNode = start;
        m_goalNode = goal;

        ShowColors(graphView, start, goal);

        // Set up the frontier nodes
        m_frontierNodes = new Queue<Node>();
        m_frontierNodes.Enqueue(start);

        // Set up the explored nodes
        m_exploredNodes = new List<Node>();

        // Set up the path nodes
        m_pathNodes = new List<Node>();

        for (int x = 0; x < m_graph.Width; x++)
        {
            for (int y = 0; y < m_graph.Height; y++)
            {
                m_graph.nodes[x, y].Reset();
            }
        }
        isComplete = false;
        m_iterations = 0;

    }

    void ShowColors()
    {
        ShowColors(m_graphView, m_startNode, m_goalNode);
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="graphView"></param>
    /// <param name="start"></param>
    /// <param name="goal"></param>
    void ShowColors(GraphView graphView, Node start, Node goal)
    {
        if (graphView == null || start == null || goal == null)
        {
            return;
        }

        // for diagnostic purposes
        if(m_frontierNodes != null)
        {
            graphView.ColorNodes(m_frontierNodes.ToList(), frontierColor);
        }

        // for diagnostic purposes
        if (m_exploredNodes != null)
        {
            graphView.ColorNodes(m_exploredNodes, exploredColor);
        }

        // Set up the start node
        NodeView startNodeView = graphView.nodeViews[start.xIndex, start.yIndex];
        if (startNodeView != null)
        {
            startNodeView.ColorNode(startColor);
        }

        // Set up the goal node
        NodeView goalNodeView = graphView.nodeViews[goal.xIndex, goal.yIndex];
        if (goalNodeView != null)
        {
            goalNodeView.ColorNode(goalColor);
        }
    }

    public IEnumerator SearchRoutine(float timeStep = 0.1f)
    {
        yield return null;

        while (!isComplete)
        {
            if (m_frontierNodes.Count > 0)
            {
                Node currentNode = m_frontierNodes.Dequeue();
                m_iterations++;

                if (!m_exploredNodes.Contains(currentNode))
                {
                    m_exploredNodes.Add(currentNode);
                }

                ExpandFrontier(currentNode);
                ShowColors();

                yield return new WaitForSeconds(timeStep);
            }
            else
            {
                isComplete = true;
            }
        }
    }

    void ExpandFrontier(Node node)
    {
        if (node != null)
        {
            for (int i = 0; i < node.neighbors.Count; i++)
            {
                if (!m_exploredNodes.Contains(node.neighbors[i]) 
                    && !m_frontierNodes.Contains(node.neighbors[i]))
                {
                    node.neighbors[i].previousNode = node;
                    m_frontierNodes.Enqueue(node.neighbors[i]);
                }
            }
        }
    }
}
