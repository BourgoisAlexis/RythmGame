using System.Collections.Generic;
using System;

[Serializable]
public class BeatCounter {
    #region Variables
    public string id;
    public int index;
    public string soundKey;
    public int beatDiv;
    public int delay;
    public List<bool> fulls;

    public int currentCount;
    #endregion


    public BeatCounter(int index, string soundKey, int beatDiv, int delay, List<bool> fulls) {
        this.index = index;
        this.soundKey = soundKey;
        this.beatDiv = beatDiv;
        this.delay = delay;
        this.fulls = fulls;

        id = soundKey;
        currentCount = 0;
    }


    public bool IsAFullBeat(int value) {
        //Apply delay
        value -= delay;

        if (value == currentCount)
            return false;

        currentCount = value;

        if (currentCount < 0 || currentCount >= fulls.Count)
            return false;

        return fulls[currentCount];
    }

    public void RepeatPattern(int n) {
        List<bool> list = new List<bool>(fulls);

        for (int i = 0; i < n; i++)
            fulls.AddRange(list);
    }
}