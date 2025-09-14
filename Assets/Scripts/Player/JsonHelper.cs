/*using System;
using UnityEngine;

public static class JsonHelper
{
    [Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }

    public static T[] FromJson<T>(string json)
    {
        string newJson = "{\"array\":" + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.array;
    }

    public static string ToJson<T>(T[] array, bool prettyPrint = false)
    {
        Wrapper<T> wrapper = new Wrapper<T> { array = array };
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }
}*/