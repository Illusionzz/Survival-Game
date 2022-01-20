using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bed : Building, IInteractable
{
    public float wakeUpTime;
    public float startCanSleepTime;
    public float endCanSleepTime;
    public float sleepToGive;

    public string GetInteractPrompt() 
    {
        return CanSleep() ? "Sleep" : "Can't sleep";
    }

    public void OnInteract() 
    {
        if (CanSleep()) {
            DayNightCycle.instance.time = wakeUpTime;
            PlayerNeeds.instance.Sleep(sleepToGive);
        }
    }

    bool CanSleep() 
    {
        return DayNightCycle.instance.time >= startCanSleepTime || DayNightCycle.instance.time <= endCanSleepTime;
    }
}
