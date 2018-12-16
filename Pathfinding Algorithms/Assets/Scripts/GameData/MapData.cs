using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapData : MonoBehaviour {

    public int width = 10;
    public int height = 5;

	void Start () {
	    	
	}

    /// <summary>
    /// Create map 2d array and initialize
    /// </summary>
    /// <returns></returns>
    int[,] CreateMap()
    {
        int[,] map = new int[width, height];
        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                map[x, y] = 0;
            }
        }

        return map;
    }
}
