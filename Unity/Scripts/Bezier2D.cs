using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

[System.Serializable]
public class Bezier2D
{
    /// <summary>
    /// Holds 3 points in 2D space that define a bezier curve
    /// </summary>
    public Vector2 P1;
    public Vector2 P2;
    public Vector2 P3;

    public Bezier2D(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        P1 = p1;
        P2 = p2;
        P3 = p3;
    }

    public Vector2 GetPoint(float t)
    {
        return Vector2.Lerp(Vector2.Lerp(P1, P2, t), Vector2.Lerp(P2, P3, t), t);
    }

    /// <summary>
    /// The exporter exports strings with 2D coords in the form "X,Y".
    /// Parses each string received by the exporter to form a Vector2.
    /// </summary>
    public static Vector2 ParsePoint(string pointStr)
    {
        var coords = pointStr.Split(',');


        var pt = new Vector2(float.Parse(coords[0], CultureInfo.InvariantCulture), -float.Parse(coords[1], CultureInfo.InvariantCulture));
        return pt;
    }

    public static Bezier2D ParseBezier(string p1, string p2, string p3)
    {
        var bezier = new Bezier2D(ParsePoint(p1), ParsePoint(p2), ParsePoint(p3));
        return bezier;
    }
}

[System.Serializable]
public class Path
{
    public List<Bezier2D> Beziers = new List<Bezier2D>();
    public int BezierCount { get { return Beziers.Count; } }
    public float T;

    public void Clear()
    {
        Beziers.Clear();
    }

    public void Add(Bezier2D bez)
    {
        Beziers.Add(bez);
    }

    public void Add(IEnumerable<Bezier2D> beziers)
    {
        Beziers.AddRange(beziers);
    }

    public Vector2 GetPoint(float t)
    {
        int i;
        if (t >= 1f)
        {
            t = 1f;
            i = BezierCount - 1;
        }
        else
        {
            t = Mathf.Clamp01(t) * BezierCount;
            i = (int) t;
            t -= i;
        }

        T = i;
        
        var pt = Beziers[i].GetPoint(t);
        return pt;
    }
}