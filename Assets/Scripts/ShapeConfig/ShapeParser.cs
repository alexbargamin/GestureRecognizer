using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public static class ShapeParser
{
    #region constant fields

    private const string PATH_CONFIG = "shapes_config";

    #endregion

    #region private fields

    private static List<Shape> m_ShapeList;

    #endregion

    #region public static methods

    public static List<Shape> GetShapes()
    {
        if (m_ShapeList == null)
        {
            TextAsset txt = (TextAsset)Resources.Load(PATH_CONFIG, typeof(TextAsset));
            m_ShapeList = Parse(txt.text);
        }
        return m_ShapeList;
    }

    #endregion

    #region private static methods

    private static List<Shape> Parse(string pathToFile)
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

    #endregion
}

