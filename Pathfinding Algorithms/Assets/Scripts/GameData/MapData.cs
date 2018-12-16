using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.SceneManagement;

public class MapData : MonoBehaviour
{

    public int width = 10;
    public int height = 5;

    public TextAsset textAsset;
    public Texture2D textureMap;
    public string resourcePath = "Mapdata";

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
                    if (texture.GetPixel(x, y) == Color.black)
                    {
                        newLine += '1';
                    }
                    else if (texture.GetPixel(x, y) == Color.white)
                    {
                        newLine += '0';
                    }
                    else
                    {
                        newLine += ' ';
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
    public int[,] CreateMap()
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
}