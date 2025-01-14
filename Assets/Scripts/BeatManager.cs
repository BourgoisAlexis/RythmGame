using System.Collections.Generic;
using UnityEngine;

public class BeatManager : MonoBehaviour {
    #region Variables
    private const char FULL = '■';
    private const char EMPTY = '□';

    [Header("Parameters")]
    [SerializeField] private int _inputDelay;
    [SerializeField] private List<InputDirection> _inputs = new List<InputDirection>();

    [Header("Refs")]
    [SerializeField] private SFXManager _sfxManager;
    [SerializeField] private TextAsset _beatFile;

    private int _offset;
    private int _beatDiv = 4;
    private float _secondsPerBeat = 0f;
    private List<BeatCounter> _counters = new List<BeatCounter>();
    private List<IBeatObserver> _onBeatObservers = new List<IBeatObserver>();
    private List<IBeatObserver> _inputBeatObservers = new List<IBeatObserver>();


    private int GetDelay() => _inputDelay * _beatDiv;
    #endregion


    private void Awake() {
        _counters.Clear();
        _onBeatObservers.Clear();

        _sfxManager.Init();

        ReadBeatFile();
    }

    private void Update() {
        foreach (BeatCounter c in _counters)
            BeatUpdate(c);
    }


    private void BeatUpdate(BeatCounter counter) {
        int value = _sfxManager.CalculateCurrentBeat(_secondsPerBeat, _offset, counter.BeatDiv);

        if (counter.IsAFullBeat(value)) {
            InputDirection input = counter.Input;

            _sfxManager.PlayAudio(input.AudioClip.name, 1, false);

            foreach (IBeatObserver observer in _onBeatObservers)
                observer.OnBeat(input, _secondsPerBeat / (float)_beatDiv, 0);
        }

        if (counter.IsAFullInputBeat(value + GetDelay())) {
            InputDirection input = counter.Input;

            foreach (IBeatObserver observer in _inputBeatObservers)
                observer.OnBeat(input, _secondsPerBeat / (float)_beatDiv, GetDelay());
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

            _counters.Add(new BeatCounter(_inputs[i - infoLines], _beatDiv, delay, fulls));
        }

        _sfxManager.PlayMusic(trackKey, volume / 100);
    }

    public void AddObserver(IBeatObserver obs, bool input) {
        if (obs == null)
            return;

        if (input) {
            if (_inputBeatObservers.Contains(obs))
                return;
            _inputBeatObservers.Add(obs);
        }
        else {
            if (_onBeatObservers.Contains(obs))
                return;
            _onBeatObservers.Add(obs);
        }
    }

    public void RemoveObserver(IBeatObserver obs) {
        if (obs == null)
            return;

        if (_onBeatObservers.Contains(obs))
            _onBeatObservers.Remove(obs);

        if (_inputBeatObservers.Contains(obs))
            _inputBeatObservers.Remove(obs);
    }
}
