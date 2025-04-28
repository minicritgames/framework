using System.Collections.Generic;
using UnityEngine;

public static class MKGeometryExtensions
{
    public static List<Vector3> FibonacciSphere(int _n)
    {
        List<Vector3> list = new List<Vector3>();
        float phi = Mathf.PI * (3f - Mathf.Sqrt(5f));

        for (int i = 0; i < _n; i++)
        {
            float y = 1f - (i / (float)(_n - 1)) * 2f;
            float r = Mathf.Sqrt(1f - y * y);
            float theta = phi * i;

            float x = Mathf.Cos(theta) * r;
            float z = Mathf.Sin(theta) * r;

            list.Add(new Vector3(x, y, z));
        }

        return list;
    }
}
