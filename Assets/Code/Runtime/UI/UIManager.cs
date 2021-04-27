using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Single;
    public List<string> meterNames;
    public List<DataMeter> meterObjects;
    
    private Dictionary<string, DataMeter> myMeters;

    private void Awake()
    {
        Single = this;
    }

    private void Start()
    {
        myMeters = new Dictionary<string, DataMeter>();
        for (int i = 0; i < meterNames.Count; i++)
        {
            myMeters.Add(meterNames[i], meterObjects[i]);
        }
    }

    public void provideData(string dataType, float dataValue = Mathf.Infinity, string dataString = "")
    {
        if (myMeters.ContainsKey(dataType)) myMeters[dataType]?.setValue(dataValue, dataString);
    }
}
