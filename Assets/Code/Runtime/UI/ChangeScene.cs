using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public AudioSource clickEffectStart;
    private float SceneChangeTime = -1f;
    private bool shallChange = false;

    private void Update()
    {
        if (SceneChangeTime > 0) SceneChangeTime -= Time.deltaTime;
        if (shallChange && Mathf.Abs(SceneChangeTime) < 1e-1) SceneManager.LoadScene("GoFish");
    }

    public void buttonStart()
    {
        shallChange = true;
        SceneChangeTime = 1.5f;
        clickEffectStart.Play();
    }
}
