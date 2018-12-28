using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GenericExtensions
{
    public static bool InRange<T>(this List<T> self,int index)
    {
        return self.Count > index && index >= 0;
    }
}