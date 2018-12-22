using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

public class MapData : MonoBehaviour
{
    #region Variables
    public int width = 10;
    public int height = 5;

    public TextAsset textAsset;
    public Texture2D textureMap;
    public string resourcePath = "Mapdata";

    public Color32 openColor = Color.white;
    public Color32 blockedColor = Color.black;
    public Color32 lightTerrainColor = new Color32(124, 194, 78, 255);
    public Color32 mediumTerrainColor = new Color32(252, 255, 52, 255);
    public Color32 heavyTerrainColor = new Color32(255, 129, 12, 255);

    static Dictionary<Color32, NodeType> terrainLookupTable = new Dictionary<Color32, NodeType>();
    #endregion
    private void Awake()
    {
        SetupLookupTable();
    }

    private void Start()
    {
        string levelName = SceneManager.GetActiveScene().name;
        if (textureMap == null)
        {
            textureMap = Resources.Load(resourcePath + "/" + levelName) as Texture2D;
        }

        if (textAsset == null)
        {
            textAsset = Resources.Load(resourcePath + "/" + levelName) as TextAsset;
        }
    }

    #region Get Data from Text Files
    /// <summary>
    /// Get the text from the textAsset file
    /// </summary>
    /// <param name="tAsset"></param>
    /// <returns></returns>
    public List<string> GetMapFromTextFile(TextAsset tAsset)
    {
        List<string> lines = new List<string>();

        if (tAsset != null)
        {
            string textData = tAsset.text;

            // \r\n is for Windows, \n is for unix/mac
            string[] delimiters = { "\r\n", "\n" };

            // add the array of split textData into lines (more efficient to use AddRange than to use .ToList()
            lines.AddRange(textData.Split(delimiters, System.StringSplitOptions.None)); // split the text file and preserve blank lines
            lines.Reverse();
        }
        else
        {
            Debug.LogWarning("MAPDATA GetTextFromFile Error: invalid TextAsset");
        }
        return lines;
    }

    /// <summary>
    /// Get text from the file set in game object
    /// </summary>
    /// <returns></returns>
    public List<string> GetMapFromTextFile()
    {
        return GetMapFromTextFile(textAsset);
    }

    #endregion

    #region Get Data from Image Files
    public List<string> GetMapFromTexture(Texture2D texture)
    {
        List<string> lines = new List<string>();
        if (texture != null)
        {
            for (int y = 0; y < texture.height; y++)
            {
                string newLine = "";

                for (int x = 0; x < texture.width; x++)
                {
                    Color pixelColor = texture.GetPixel(x, y);

                    if (terrainLookupTable.ContainsKey(pixelColor))
                    {
                        NodeType nodeType = terrainLookupTable[pixelColor];
                        int nodeTypeNum = (int)nodeType;
                        newLine += nodeTypeNum;
                    }
                    else
                    {
                        newLine += '0';
                    }
                }
                lines.Add(newLine);
            }
        }
        return lines;
    }
    #endregion

    /// <summary>
    /// Set width and height from text map data
    /// </summary>
    /// <param name="textLines"></param>

    #region Setup
    public void SetDimensions(List<string> textLines)
    {
        height = textLines.Count;
        foreach (string line in textLines)
        {
            if (line.Length > width)
            {
                width = line.Length;
            }
        }
    }

    /// <summary>
    /// Create map 2d array and initialize
    /// </summary>
    /// <returns></returns>
    public int[,] MakeMap()
    {
        List<string> lines = new List<string>();

        if (textureMap != null)
        {
            lines = GetMapFromTexture(textureMap);
        }
        else
        {
            lines = GetMapFromTextFile(textAsset);
        }

        SetDimensions(lines);

        int[,] map = new int[width, height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (lines[y].Length > x)
                {
                    map[x, y] = (int)Char.GetNumericValue(lines[y][x]);
                }

            }
        }
        return map;
    }

    /// <summary>
    /// Sets up the lookup table with its nodes and node types
    /// </summary>
    void SetupLookupTable()
    {
        terrainLookupTable.Add(openColor, NodeType.Open);
        terrainLookupTable.Add(blockedColor, NodeType.Blocked);
        terrainLookupTable.Add(lightTerrainColor, NodeType.LightTerrain);
        terrainLookupTable.Add(mediumTerrainColor, NodeType.MediumTerrain);
        terrainLookupTable.Add(heavyTerrainColor, NodeType.HeavyTerrain);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nodeType"></param>
    /// <returns></returns>
    public static Color GetColorFromNodeType(NodeType nodeType)
    {
        if (terrainLookupTable.ContainsValue(nodeType))
        {
            // find the corresponding color key for node type
            Color colorKey = terrainLookupTable.FirstOrDefault(x => x.Value == nodeType).Key;
            return colorKey;
        }
        return Color.white;
    }
    #endregion
}
