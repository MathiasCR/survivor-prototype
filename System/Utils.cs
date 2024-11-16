using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Utils
{
    public static T FindComponentInChildrenWithTag<T>(this GameObject parent, string tag)
    {
        Transform t = parent.transform;
        foreach (Transform tr in t)
        {
            if (tr.tag == tag)
            {
                return tr.GetComponent<T>();
            }
        }

        throw new Exception("FindComponentInChildWithTag - No Component with tag " + tag + " found");
    }

    public static List<T> FindComponentsInChildrenWithTag<T>(this GameObject parent, string tag)
    {
        Transform t = parent.transform;
        List<T> childrenList = new List<T>();
        foreach (Transform tr in t)
        {
            if (tr.tag == tag)
            {
                childrenList.Add(tr.GetComponent<T>());
            }
        }

        return childrenList;
    }

    public static string RemoveCloneFromName(string name)
    {
        if (name.Contains("(Clone)"))
        {
            return name.Replace("(Clone)", "");
        }
        else
        {
            return name;
        }
    }

    public static Dictionary<TKey, int> ConcatenateTwoDictionaries<TKey, TValue>(Dictionary<TKey, int> firstDictionary, Dictionary<TKey, int> secondDictionary)
    {
        Dictionary<TKey, int> newDictionary = new Dictionary<TKey, int>();

        foreach (TKey key in firstDictionary.Keys)
        {
            newDictionary.Add(key, firstDictionary[key]);
        }

        foreach (TKey key in secondDictionary.Keys)
        {
            if (newDictionary.ContainsKey(key))
            {
                newDictionary[key] += secondDictionary[key];
            }
            else
            {
                newDictionary.Add(key, secondDictionary[key]);
            }
        }

        return newDictionary;
    }

    public static List<int> GenerateRandom(int count, int inclusiveMin, int exclusiveMax)
    {
        if (exclusiveMax <= inclusiveMin || count < 0 ||
                // max - min > 0 required to avoid overflow
                (count > exclusiveMax - inclusiveMin && exclusiveMax - inclusiveMin > 0))
        {
            // need to use 64-bit to support big ranges (negative min, positive max)
            throw new ArgumentOutOfRangeException("Range " + inclusiveMin + " to " + exclusiveMax +
                    " (" + ((Int64)exclusiveMax - (Int64)inclusiveMin) + " values), or count " + count + " is illegal");
        }

        // generate count random values.
        HashSet<int> candidates = new HashSet<int>();

        // start count values before max, and end at max
        for (int top = exclusiveMax - count; top < exclusiveMax; top++)
        {
            // May strike a duplicate.
            // Need to add +1 to make inclusive generator
            // +1 is safe even for MaxVal max value because top < max
            if (!candidates.Add(new System.Random().Next(inclusiveMin, top + 1)))
            {
                // collision, add inclusive max.
                // which could not possibly have been added before.
                candidates.Add(top);
            }
        }

        // load them in to a list, to sort
        List<int> result = candidates.ToList();

        // shuffle the results because HashSet has messed
        // with the order, and the algorithm does not produce
        // random-ordered results (e.g. max-1 will never be the first value)
        for (int i = result.Count - 1; i > 0; i--)
        {
            int k = new System.Random().Next(i + 1);
            int tmp = result[k];
            result[k] = result[i];
            result[i] = tmp;
        }
        return result;
    }

    public static bool CompareLayerToLayerMask(int layer, LayerMask layerMask)
    {
        return 1 << layer == layerMask.value;
    }

    public static T? PickRandomEnumValue<T>() { var v = Enum.GetValues(typeof(T)); return (T?)(v?.GetValue(new System.Random().Next(v.Length))); }

    public static bool IsNull<T>(this T source) where T : struct
    {
        return source.Equals(default(T));
    }

    public static List<Color> GetColors()
    {
        List<Color> colors = new List<Color>
        {
            Color.gray,
            Color.green,
            Color.yellow,
            Color.blue,
            Color.cyan,
            Color.magenta,
            Color.red,
            Color.black,
            Color.white
        };
        return colors;
    }
}

