using System.Collections.Generic;
using System;

[Serializable]
public class BeatCounter {
    #region Variables
    public string id;
    public DirectionInput Input;
    public int BeatDiv;
    public int Delay;
    public List<bool> Fulls;

    public int CurrentCount;
    public int InputCount;
    #endregion


    public BeatCounter(DirectionInput input, int beatDiv, int delay, List<bool> fulls) {
        Input = input;
        BeatDiv = beatDiv;
        Delay = delay;
        Fulls = fulls;

        id = input.AudioClip.name;
        CurrentCount = 0;
    }


    public bool IsAFullBeat(int value) {
        //Apply delay
        value -= Delay;

        if (value == CurrentCount)
            return false;

        CurrentCount = value;

        if (CurrentCount < 0 || CurrentCount >= Fulls.Count)
            return false;

        return Fulls[CurrentCount];
    }

    public bool IsAFullInputBeat(int value) {
        //Apply delay
        value -= Delay;

        if (value == InputCount)
            return false;

        InputCount = value;

        if (InputCount < 0 || InputCount >= Fulls.Count)
            return false;

        return Fulls[InputCount];
    }

    public void RepeatPattern(int n) {
        List<bool> list = new List<bool>(Fulls);

        for (int i = 0; i < n; i++)
            Fulls.AddRange(list);
    }
}