using System;
using UnityEngine;

public class GestureRecognizer
{
    #region public static methods

    /// <summary>
    /// Get matching shapes in range [0..1]
    /// </summary>
    public static float CheckMatchingShape(Gesture drawingShape, Gesture templateShape)
    {
        float minDistance = float.MaxValue;

        float dist = CompareSetsWithEachOther(drawingShape.Points, templateShape.Points);
        if (dist < minDistance)
        {
            minDistance = dist;
        }

        return Mathf.Max((minDistance - 2.0f) / -2.0f, 0.0f);
    }

    #endregion

    #region private static methods

    /// <summary>
    /// Compare sets with each other and get the minimum distance matching between two point sets
    /// </summary>
    private static float CompareSetsWithEachOther(Vector2[] points1, Vector2[] points2)
    {
        int n = points1.Length;

        // Controls the number of search trials (eps is in [0..1])
        float eps = 0.5f;       
        int step = (int)Math.Floor(Math.Pow(n, 1.0f - eps));
        float minDistance = float.MaxValue;
        for (int i = 0; i < n; i += step)
        {
            // Match points1 with points2 starting with index point i
            float dist1 = GetDistanceBetweenPointSets(points1, points2, i);

            // Match points2 with points1 starting with index point i  
            float dist2 = GetDistanceBetweenPointSets(points2, points1, i);  

            minDistance = Math.Min(minDistance, Math.Min(dist1, dist2));
        }
        return minDistance;
    }

    /// <summary>
    /// Calculates the distance between two point set by performing a minimum distance of matching
    /// </summary>
    private static float GetDistanceBetweenPointSets(Vector2[] points1, Vector2[] points2, int startIndex)
    {
        // The two set should have the same number of points by now
        int n = points1.Length;

        // Matched[i] signals whether point i from the 2nd set has been already matched    
        bool[] matched = new bool[n]; 
        Array.Clear(matched, 0, n);

        // The sum of the distance between the two sets
        float sum = 0;  
        int i = startIndex;
        do
        {
            int index = -1;
            float minDistance = float.MaxValue;
            for (int j = 0; j < n; j++)
                if (!matched[j])
                {
                    float dist = (points1[i].x - points2[j].x) * (points1[i].x - points2[j].x) + (points1[i].y - points2[j].y) * (points1[i].y - points2[j].y);
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        index = j;
                    }
                }

            // point index from the 2nd set is matched to point i from the 1st set
            matched[index] = true; 
            float weight = 1.0f - ((i - startIndex + n) % n) / (1.0f * n);

            // weight each distance with a confidence coefficient that decreases from 1 to 0
            sum += weight * minDistance;
            i = (i + 1) % n;
        } while (i != startIndex);
        return sum;
    }

    #endregion
}
