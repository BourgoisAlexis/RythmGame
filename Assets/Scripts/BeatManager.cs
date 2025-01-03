using System.Collections.Generic;
using UnityEngine;

public class BeatManager : MonoBehaviour {
    #region Variables
    private const char FULL = '■';
    private const char EMPTY = '□';

    [Header("Parameters")]
    [SerializeField] private List<AudioClip> _instruments = new List<AudioClip>();

    [Header("Refs")]
    [SerializeField] private SFXManager _sfxManager;
    [SerializeField] private TextAsset _beatFile;

    private int _offset;
    private int _beatDiv = 4;
    private float _secondsPerBeat = 0f;
    private List<BeatCounter> _counters = new List<BeatCounter>();
    private List<IBeatObserver> _beatObservers = new List<IBeatObserver>();
    #endregion


    private void Awake() {
        _counters.Clear();
        _beatObservers.Clear();

        _sfxManager.Init();

        ReadBeatFile();
    }

    private void Update() {
        foreach (BeatCounter c in _counters)
            BeatUpdate(c);
    }


    private void BeatUpdate(BeatCounter counter) {
        int value = _sfxManager.CalculateCurrentBeat(_secondsPerBeat, _offset, counter.beatDiv);

        if (counter.IsAFullBeat(value)) {
            _sfxManager.PlayAudio(counter.soundKey, 1, false);

            foreach (IBeatObserver observer in _beatObservers)
                observer.OnBeat(counter.index, _secondsPerBeat / (float)_beatDiv);
        }
    }


    private void ReadBeatFile() {
        string[] lines = _beatFile.text.Split('\n');
        int infoLines = 1;

        string[] infos = lines[0].Split(FULL);
        string trackKey = infos[0];
        int bpm = int.Parse(infos[1]);
        int delay = int.Parse(infos[2]);
        float volume = float.Parse(infos[3]);
        int offset = int.Parse(infos[4]);

        _secondsPerBeat = 60f / (float)bpm;
        _offset = offset;

        for (int i = infoLines; i < lines.Length; i++) {
            string line = lines[i];

            List<bool> fulls = new List<bool>();
            foreach (char c in line)
                fulls.Add(c == FULL);

            _counters.Add(new BeatCounter(i - infoLines, _instruments[i - infoLines].name, _beatDiv, delay, fulls));
        }

        _sfxManager.PlayMusic(trackKey, volume / 100);
    }

    public void AddObserver(IBeatObserver obs) {
        if (obs == null)
            return;

        if (_beatObservers.Contains(obs))
            return;

        _beatObservers.Add(obs);
    }

    public void RemoveObserver(IBeatObserver obs) {
        if (obs == null)
            return;

        if (!_beatObservers.Contains(obs))
            return;

        _beatObservers.Remove(obs);
    }
}
