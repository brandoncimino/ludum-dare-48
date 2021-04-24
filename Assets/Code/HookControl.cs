using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HookControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
         var keyboard = Keyboard.current;
         if (keyboard == null)
         {
             Debug.LogError("No Keyboard Detected");
             return; //no keyboard detected
         }



         transform.position += DirectInputToVector3(keyboard);
    }

    /// <summary>
    /// Given the current buttons pushed within WASD, return a Vector2 where A&D control left and right, and W&S control up and down respectively.
    /// </summary>
    /// <param name="keyboard">Input Manager's keyboard. This is the current keyboard</param>
    /// <returns>A Normalized Vector2 based on WASD input</returns>
    Vector3 DirectInputToVector3(Keyboard keyboard)
    {
        return new Vector2(keyboard.dKey.ReadValue() - keyboard.aKey.ReadValue(),
            keyboard.wKey.ReadValue() - keyboard.sKey.ReadValue());
    }
    
}
