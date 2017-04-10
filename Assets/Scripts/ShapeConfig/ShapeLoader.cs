using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class ShapeLoader : MonoBehaviour
{
    #region delegate and event fields

    public delegate void LoadAction();


    // Fire when config is loaded
    public event LoadAction OnSuccessLoad;

    // Fire when config is not loaded
    public event LoadAction OnFailLoad;

    #endregion

    #region constant fields

    private const string PATH_CONFIG = "shapes_config.xml";

    #endregion

    #region private fields

    private List<Shape> m_ShapeList;

    #endregion

    #region public methods

    public List<Shape> GetShapes()
    {
        return m_ShapeList;
    }

    #endregion

    #region private methods

    private List<Shape> Parse(string pathToFile)
    {
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(pathToFile);

        var shapeNodes = doc.DocumentElement.SelectNodes("shape");

        var shapeCollection = new List<Shape>();
        foreach (XmlNode shapeNode in shapeNodes)
        {
            var shape = new Shape();

            var pointList = new List<Vector2>();

            var pointNodes = shapeNode.SelectSingleNode("points");
            foreach (XmlNode pointNode in pointNodes.SelectNodes("point"))
            {
                var point_x = Convert.ToInt32(pointNode.Attributes["x"].InnerText);
                var point_y = Convert.ToInt32(pointNode.Attributes["y"].InnerText);

                var point = new Vector2((float)point_x, (float)point_y);

                pointList.Add(point);
            }
            shape.Points = pointList;
            shapeCollection.Add(shape);
        }

        return shapeCollection;
    }

    private IEnumerator DownloadConfig()
    {
        WWW data = new WWW("file://" + System.IO.Path.Combine(Application.streamingAssetsPath, PATH_CONFIG));
        yield return data;

        if (string.IsNullOrEmpty(data.error))
        {
            m_ShapeList = Parse(data.text);
            if (OnSuccessLoad != null)
            {
                OnSuccessLoad();
            }
        }
        else
        {
            if (OnFailLoad != null)
            {
                OnFailLoad();
            }
            Debug.LogError("Colud not dowdload config");
        }
    }

    private void Awake()
    {
        StartCoroutine(DownloadConfig());
    }

    #endregion
}

