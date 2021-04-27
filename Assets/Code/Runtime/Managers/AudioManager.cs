using System;
using System.Collections;
using System.Collections.Generic;
using Code.Runtime;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource AudioSpooky;
    public AudioSource AudioCalm;
        
    // Start is called before the first frame update
    void Start()
    {
        EventManager.Single.ONTriggerLevelUp += SpookUp;
    }

    private void OnDestroy()
    {
        EventManager.Single.ONTriggerLevelUp -= SpookUp;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpookUp(int lvl)
    {
        switch (GameManager.Single.lvl)
        {
            case 2: 
                AudioCalm.volume *= 0.5f;
                AudioSpooky.Play();
                break;
            case 3:
                AudioCalm.Stop();
                AudioSpooky.volume *= 2;
                break;
            case 4:
                AudioSpooky.volume *= 1.1f;
                break;
        }
        
    }
    
    
}
