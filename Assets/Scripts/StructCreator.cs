using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Buff
{
    public GameControllerScript.BuffTypes type { get; set; }
    public float affectIntensity { get; set; }
    public float affectDelay { get; set; }
    public float affectDuration { get; set; }
    public bool isCompounding { get; set; }

    public Buff(GameControllerScript.BuffTypes type, float intensity, float delay, float duration, bool compounding)
    {
        this.type = type;
        affectIntensity = intensity;
        affectDelay = delay;
        affectDuration = duration;
        isCompounding = compounding;
    }
}

public struct Card
{
    public string name { get; set; }
    public Buff buff { get; set; }
}

public struct LevelProperty
{
    public int minTrows { get; set; }
    public int maxTrows { get; set; }

    public int minFaes { get; set; }
    public int maxFaes { get; set; }

    public int minPiskies { get; set; }
    public int maxPiskies { get; set; }

}

