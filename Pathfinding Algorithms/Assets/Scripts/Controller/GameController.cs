using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public MapData mapData;
    public Graph graph;

    private void Start()
    {
        if (mapData != null && graph != null)
        {
            int[,] mapInstance = mapData.CreateMap();
            graph.Init(mapInstance);

            GraphView graphView = graph.gameObject.GetComponent<GraphView>();

            if(graphView != null)
            {
                graphView.Init(graph);
            }
        }
    }
}
