using System;
using System.Collections;
using System.Collections.Generic;
using Code.Runtime;
using JetBrains.Annotations;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Single;

    private void Awake()
    {
        Single = this;
    }

    #region game management

    public event Action ONTriggerGameOverWin;
    public void TriggerGameOverWin()
    {
        ONTriggerGameOverWin?.Invoke();
    }
    
    public event Action ONTriggerGameOverFail;
    public void TriggerGameOverFail()
    {
        ONTriggerGameOverFail?.Invoke();
    }
    
    public event Action <int> ONTriggerLevelUp;
    public void TriggerLevelUp(int level = -1)
    {
        ONTriggerLevelUp?.Invoke(level);
    }

    #endregion

    #region fish

    public event Action <FishBehaviour> ONTriggerCollisionFish;
    public void TriggerCollisionFish(FishBehaviour fish)
    {
        ONTriggerCollisionFish?.Invoke(fish);
    }
    
    public event Action ONTriggerFirstCatch;
    public void TriggerFirstCatch()
    {
        ONTriggerFirstCatch?.Invoke();
    }
    

    #endregion

    #region sharks

    public event Action ONTriggerCollisionShark;
    public void TriggerCollisionShark()
    {
        ONTriggerCollisionShark?.Invoke();
    }
    
    public event Action ONTriggerSharkAttack;
    public void TriggerSharkAttack()
    {
        ONTriggerSharkAttack?.Invoke();
    }

    #endregion

    #region collectables

    public event Action <DebrisBehaviour> ONTriggerCollisionDebris;
    public void TriggerCollisionDebris(DebrisBehaviour debris)
    {
        ONTriggerCollisionDebris?.Invoke(debris);
    }
    
    
    public event Action <Catchables> ONTriggerCollisionCatchable;
    public void TriggerCollisionCatchable(Catchables newCatch)
    {
        ONTriggerCollisionCatchable?.Invoke(newCatch);
    }

    #endregion

    
    
    
    
    

    
    
    
    
    

}
