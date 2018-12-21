using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeView : MonoBehaviour {

    public GameObject tile;
    public GameObject arrow;
    Node m_node;


    [Range(0, 0.5f)]
    public float borderSize = 0.15f;

    /// <summary>
    /// Initialize the tiles
    /// </summary>
    /// <param name="node"></param>
    public void Init(Node node)
    {
        if (tile != null)
        {
            gameObject.name = "Node (" + node.xIndex + "," + node.yIndex + ")";
            gameObject.transform.position = node.position;
            tile.transform.localScale = new Vector3(1f - borderSize, 1f, 1f - borderSize);
            m_node = node;
            EnableObject(arrow, false);
        }
    }

    /// <summary>
    /// Color a specific node
    /// </summary>
    /// <param name="color"></param>
    /// <param name="go"></param>
    void ColorNode(Color color, GameObject go)
    {
        if(go != null)
        {
            Renderer goRenderer = go.GetComponent<Renderer>();
            if(goRenderer != null)
            {
                goRenderer.material.color = color;
            }
        }
    }

    /// <summary>
    /// Call to color a node
    /// </summary>
    /// <param name="color"></param>
    public void ColorNode(Color color)
    {
        ColorNode(color, tile);
    }

    /// <summary>
    /// Enables or disables any game object passed
    /// </summary>
    /// <param name="go"></param>
    /// <param name="state"></param>
    void EnableObject(GameObject go, bool state)
    {
        if(go != null)
        {
            go.SetActive(state);
        }
    }

    /// <summary>
    /// Show the arrow and point it to the next node 
    /// </summary>
    public void ShowArrow(Color color)
    {
        if(m_node != null && arrow != null && m_node.previousNode != null)
        {
            EnableObject(arrow, true);

            // make arrow look at next node
            Vector3 dirToPrevious = (m_node.previousNode.position - m_node.position).normalized;
            arrow.transform.rotation = Quaternion.LookRotation(dirToPrevious);

            Renderer arrowRenderer = arrow.GetComponent<Renderer>();
            if(arrowRenderer != null)
            {
                arrowRenderer.material.color = color;
            }
        }
    }


}
