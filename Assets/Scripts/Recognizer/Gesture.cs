using System;
using UnityEngine;

public class Gesture
{
    #region constant fields

    private const int FIXED_NUMBER_OF_POINTS = 64;

    #endregion

    #region public fields

    public Vector2[] Points = null;

    #endregion

    #region public methods

    public Gesture(Vector2[] points)
    {
        this.Points = NormalizeScale(points);
        this.Points = TranslateToOrigin(Points, GetCenterOfShape(Points));
        this.Points = DevideIntoFixedPointNumber(Points, FIXED_NUMBER_OF_POINTS);
    }

    #endregion

    #region private methods

    /// <summary>
    /// Normalize gestures with respect to scale where x=[0..1], y=[0..1]
    /// </summary>
    private Vector2[] NormalizeScale(Vector2[] points)
    {
        float minx = float.MaxValue, miny = float.MaxValue, maxx = float.MinValue, maxy = float.MinValue;
        for (int i = 0; i < points.Length; i++)
        {
            if (minx > points[i].x) minx = points[i].x;
            if (miny > points[i].y) miny = points[i].y;
            if (maxx < points[i].x) maxx = points[i].x;
            if (maxy < points[i].y) maxy = points[i].y;
        }

        Vector2[] newPoints = new Vector2[points.Length];
        float scale = Math.Max(maxx - minx, maxy - miny);
        for (int i = 0; i < points.Length; i++)
            newPoints[i] = new Vector2((points[i].x - minx) / scale, (points[i].y - miny) / scale);
        return newPoints;
    }

    /// <summary>
    /// Translate points relative to origin
    /// </summary>
    private Vector2[] TranslateToOrigin(Vector2[] points, Vector2 p)
    {
        Vector2[] newPoints = new Vector2[points.Length];
        for (int i = 0; i < points.Length; i++)
            newPoints[i] = new Vector2(points[i].x - p.x, points[i].y - p.y);
        return newPoints;
    }

    /// <summary>
    /// Get the center of shape
    /// </summary>
    private Vector2 GetCenterOfShape(Vector2[] points)
    {
        float cx = 0, cy = 0;
        for (int i = 0; i < points.Length; i++)
        {
            cx += points[i].x;
            cy += points[i].y;
        }
        return new Vector2(cx / points.Length, cy / points.Length);
    }

    /// <summary>
    /// Devide shape into a fixed number of n points
    /// </summary>
    private Vector2[] DevideIntoFixedPointNumber(Vector2[] points, int n)
    {
        Vector2[] newPoints = new Vector2[n];
        newPoints[0] = new Vector2(points[0].x, points[0].y);
        int numPoints = 1;

        // Computes interval length
        float I = GetPointArrayLength(points) / (n - 1); 
        float D = 0;
        for (int i = 1; i < points.Length; i++)
        {
            float d = Mathf.Sqrt((points[i - 1].x - points[i].x) * (points[i - 1].x - points[i].x) + (points[i - 1].y - points[i].y) * (points[i - 1].y - points[i].y));
            if (D + d >= I)
            {
                Vector2 firstPoint = points[i - 1];
                while (D + d >= I)
                {
                    // Add interpolated point
                    float t = Math.Min(Math.Max((I - D) / d, 0.0f), 1.0f);
                    if (float.IsNaN(t)) t = 0.5f;
                    newPoints[numPoints++] = new Vector2(
                        (1.0f - t) * firstPoint.x + t * points[i].x,
                        (1.0f - t) * firstPoint.y + t * points[i].y
                    );

                    // Update partial length
                    d = D + d - I;
                    D = 0;
                    firstPoint = newPoints[numPoints - 1];
                }
                D = d;
            }
            else D += d;
        }

        if (numPoints == n - 1) 
            newPoints[numPoints++] = new Vector2(points[points.Length - 1].x, points[points.Length - 1].y);
        return newPoints;
    }

    /// <summary>
    /// Get length between all points in array
    /// </summary>
    private float GetPointArrayLength(Vector2[] points)
    {
        float length = 0;
        for (int i = 1; i < points.Length; i++)
            length += Mathf.Sqrt((points[i - 1].x - points[i].x) * (points[i - 1].x - points[i].x) + (points[i - 1].y - points[i].y) * (points[i - 1].y - points[i].y));
        return length;
    }

    #endregion
}