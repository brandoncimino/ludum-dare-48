using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DataMeter : MonoBehaviour
{
    public TextMeshProUGUI field;
    public float dataValue;
    public string dataString;
    public bool isNumeral = true;
    public string dataUnit;


    private void Start()
    {
        field.text = isNumeral ? dataValue.ToString() : dataString;
        field.text += dataUnit;
    }

    public void setValue(float newData)
    {
        if (newData != dataValue)
        {
            dataValue = newData;
            dataString = dataValue.ToString();
            field.text = dataString + " " + dataUnit;
        }
    }
    
    public void setValue(string newData)
    {
        if (newData != dataString)
        {
            dataString = newData;
            dataValue = Mathf.Infinity;
            field.text = dataString + dataUnit;
        }
    }
    
    public void setValue(float newDataFloat, string newDataString)
    {
        if (isNumeral)
        {
            setValue(newDataFloat);
        }
        else
        {
            setValue(newDataString);
        }
    }
}
