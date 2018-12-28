using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions
{
    public static void DestroyAllChildren(this Transform root)
    {
        foreach (Transform c in root)
        {
            Object.Destroy(c.gameObject);
        }
    }
}
