using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extension
{
    private static System.Random rnd = new System.Random();
    
    
    
    //Name: Shuffle 
    //Description : Fisher-Yates 
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = (rnd.Next(n+1));
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}

