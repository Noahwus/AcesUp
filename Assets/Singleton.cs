using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{

    protected void NewInstance(T obj)
    {
        instance = obj;
    }

    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
                Debug.LogError("Class IS NULL, make sure you called NewInstance function in awake");
            return instance;
        }
    }

}
