using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pathfinder : MonoBehaviour
{
    public enum Mode
    {
        BreadthFirstSearch = 0,
        Dijkstra = 1,
        GreedyBestFirst = 2,
        AStar = 3
    }

    #region Variables
    public Mode mode = Mode.BreadthFirstSearch;
    Node m_startNode;
    Node m_goalNode;

    Graph m_graph;
    GraphView m_graphView;

    PriorityQueue<Node> m_frontierNodes;
    List<Node> m_exploredNodes;
    List<Node> m_pathNodes;

    // Colors for nodes
    public Color startColor = Color.green;
    public Color goalColor = Color.red;
    public Color frontierColor = Color.yellow;
    public Color exploredColor = Color.grey;
    public Color pathColor = Color.cyan;
    public Color arrowColor = new Color32(216, 216, 216, 255);
    public Color highlightColor = new Color32(216, 216, 128, 255);

    // diagnostic purposes
    public bool showIterations = true;
    public bool showColors = true;
    public bool showArrows = true;
    public bool exitOnGoal = true;

    public bool isComplete = false;
    int m_iterations = 0;

    #endregion

    /// <summary>
    /// Initialize pathfinder
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
        m_frontierNodes = new PriorityQueue<Node>();
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
        m_startNode.distanceTraveled = 0;
    }

    #region Coloring
    void ShowColors(bool lerpColor = false, float lerpValue = 0.5f)
    {
        ShowColors(m_graphView, m_startNode, m_goalNode, lerpColor, lerpValue);
    }

    /// <summary>
    /// Change the colors of nodes
    /// </summary>
    /// <param name="graphView"></param>
    /// <param name="start"></param>
    /// <param name="goal"></param>
    void ShowColors(GraphView graphView, Node start, Node goal, bool lerpColor = false, float lerpValue = 0.5f)
    {
        if (graphView == null || start == null || goal == null)
        {
            return;
        }

        // for diagnostic purposes
        if (m_frontierNodes != null)
        {
            graphView.ColorNodes(m_frontierNodes.ToList(), frontierColor, lerpColor, lerpValue);
        }

        // for diagnostic purposes
        if (m_exploredNodes != null)
        {
            graphView.ColorNodes(m_exploredNodes, exploredColor, lerpColor, lerpValue);
        }

        if (m_pathNodes != null && m_pathNodes.Count > 0)
        {
            graphView.ColorNodes(m_pathNodes, pathColor, lerpColor, lerpValue * 2f);
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
    #endregion

    /// <summary>
    /// Explore the graph
    /// </summary>
    /// <param name="timeStep"></param>
    /// <returns></returns>
    public IEnumerator SearchRoutine(float timeStep = 0.1f)
    {
        float timeStart = Time.time;

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

                switch (mode)
                {
                    case Mode.BreadthFirstSearch:
                        ExpandFrontierBFS(currentNode);
                        break;
                    case Mode.Dijkstra:
                        ExpandFrontierDijkstra(currentNode);
                        break;
                    case Mode.GreedyBestFirst:
                        ExpandFrontierGreedy(currentNode);
                        break;
                    case Mode.AStar:
                        ExpandFrontierAStar(currentNode);
                        break;
                }


                if (m_frontierNodes.Contains(m_goalNode))
                {
                    m_pathNodes = GetPathNodes(m_goalNode);
                    if (exitOnGoal)
                    {
                        isComplete = true;
                        Debug.Log("PATHFINDER mode: " + mode.ToString() +
                                  "    path length = " + m_goalNode.distanceTraveled.ToString());
                    }
                }

                // only show colors and arrows if we want to for diagnostics
                if (showIterations)
                {
                    ShowDiagnostics();
                    yield return new WaitForSeconds(timeStep);
                }
            }
            else
            {
                isComplete = true;
            }
        }
        ShowDiagnostics();
        Debug.Log("Pathfinder SearchRoutine: elapse time = " + (Time.time - timeStart).ToString() + " seconds");
    }

    private void ShowDiagnostics(bool lerpColor = false, float lerpValue = 0.5f)
    {
        if (showColors)
        {
            ShowColors();
        }

        if (m_graphView != null && showArrows)
        {
            m_graphView.ShowNodeArrows(m_frontierNodes.ToList(), arrowColor);

            if (m_frontierNodes.Contains(m_goalNode))
            {
                m_graphView.ShowNodeArrows(m_pathNodes, highlightColor);
            }

        }
    }

    #region Pathfinding Algorithms

    /// <summary>
    /// Adds the frontier's neighbors as new frontier nodes in BFS manner
    /// </summary>
    /// <param name="node"></param>
    void ExpandFrontierBFS(Node node)
    {
        if (node != null)
        {
            for (int i = 0; i < node.neighbors.Count; i++)
            {
                if (!m_exploredNodes.Contains(node.neighbors[i])
                    && !m_frontierNodes.Contains(node.neighbors[i]))
                {
                    float distanceToNeighbor = m_graph.GetNodeDistance(node, node.neighbors[i]);
                    float newDistanceTraveled = distanceToNeighbor + node.distanceTraveled + (int)node.nodeType;
                    node.neighbors[i].distanceTraveled = newDistanceTraveled;
                    node.neighbors[i].previousNode = node;
                    node.neighbors[i].priority = m_exploredNodes.Count;
                    m_frontierNodes.Enqueue(node.neighbors[i]);
                }
            }
        }
    }

    /// <summary>
    /// Adds the frontier's neighbors as new frontier nodes in Dijkstra manner
    /// </summary>
    /// <param name="node"></param>
    void ExpandFrontierDijkstra(Node node)
    {
        if (node != null)
        {
            for (int i = 0; i < node.neighbors.Count; i++)
            {
                if (!m_exploredNodes.Contains(node.neighbors[i]))
                {
                    float distanceToNeighbor = m_graph.GetNodeDistance(node, node.neighbors[i]);
                    float newDistranceTraveled = distanceToNeighbor + node.distanceTraveled + (int)node.nodeType;

                    if (float.IsPositiveInfinity(node.neighbors[i].distanceTraveled) || newDistranceTraveled < node.neighbors[i].distanceTraveled)
                    {
                        node.neighbors[i].previousNode = node;
                        node.neighbors[i].distanceTraveled = newDistranceTraveled;
                    }
                    if (!m_frontierNodes.Contains(node.neighbors[i]))
                    {
                        node.neighbors[i].priority = node.neighbors[i].distanceTraveled;
                        m_frontierNodes.Enqueue(node.neighbors[i]);
                    }

                }
            }
        }
    }

    /// <summary>
    /// Adds the frontier's neighbors as new frontier nodes in Greedy Best First manner
    /// </summary>
    /// <param name="node"></param>
    void ExpandFrontierGreedy(Node node)
    {
        if (node != null)
        {
            for (int i = 0; i < node.neighbors.Count; i++)
            {
                if (!m_exploredNodes.Contains(node.neighbors[i])
                    && !m_frontierNodes.Contains(node.neighbors[i]))
                {
                    float distanceToNeighbor = m_graph.GetNodeDistance(node, node.neighbors[i]);
                    float newDistanceTraveled = distanceToNeighbor + node.distanceTraveled + (int)node.nodeType;
                    node.neighbors[i].distanceTraveled = newDistanceTraveled;
                    node.neighbors[i].previousNode = node;

                    if(m_graph != null)
                    {
                        node.neighbors[i].priority = m_graph.GetNodeDistance(node.neighbors[i], m_goalNode);
                    }
                    
                    m_frontierNodes.Enqueue(node.neighbors[i]);
                }
            }
        }
    }

    /// <summary>
    /// Adds the frontier's neighbors as new frontier nodes in Dijkstra manner
    /// </summary>
    /// <param name="node"></param>
    void ExpandFrontierAStar(Node node)
    {
        if (node != null)
        {
            for (int i = 0; i < node.neighbors.Count; i++)
            {
                if (!m_exploredNodes.Contains(node.neighbors[i]))
                {
                    float distanceToNeighbor = m_graph.GetNodeDistance(node, node.neighbors[i]);
                    float newDistranceTraveled = distanceToNeighbor + node.distanceTraveled + (int)node.nodeType;

                    if (float.IsPositiveInfinity(node.neighbors[i].distanceTraveled) || newDistranceTraveled < node.neighbors[i].distanceTraveled)
                    {
                        node.neighbors[i].previousNode = node;
                        node.neighbors[i].distanceTraveled = newDistranceTraveled;
                    }
                    if (!m_frontierNodes.Contains(node.neighbors[i]) && m_graph != null)
                    {
                        // priority frontier nodes with fscore
                        float distanceToGoal = m_graph.GetNodeDistance(node.neighbors[i], m_goalNode);
                        node.neighbors[i].priority = node.neighbors[i].distanceTraveled + distanceToGoal;
                        m_frontierNodes.Enqueue(node.neighbors[i]);
                    }

                }
            }
        }
    }
    #endregion

    /// <summary>
    /// Returns list of nodes in the path
    /// </summary>
    /// <param name="endNode"></param>
    /// <returns></returns>
    List<Node> GetPathNodes(Node endNode)
    {
        List<Node> path = new List<Node>();
        if (endNode == null)
        {
            return path;
        }
        path.Add(endNode);

        Node currentNode = endNode.previousNode;

        while (currentNode != null)
        {
            // put the current node at the first spot
            path.Insert(0, currentNode);
            currentNode = currentNode.previousNode;
        }
        return path;
    }
}
