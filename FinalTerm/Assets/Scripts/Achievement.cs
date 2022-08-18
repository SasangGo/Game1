using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Achievement
{
    public string achieveTitle;
    public string achieveExplane;
    public string achieveEffect;
    public bool isAchieve;

    public Achievement(string title, string explane, string effect, bool isAchieve)
    {
        this.achieveTitle = title;
        this.achieveExplane = explane;
        this.achieveEffect = effect;
        this.isAchieve = isAchieve;
    }
}
