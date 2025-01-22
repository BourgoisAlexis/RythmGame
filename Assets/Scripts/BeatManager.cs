using System.Collections.Generic;
using UnityEngine;

public class BeatManager : MonoBehaviour {
    #region Variables
    private const char FULL = '■';
    private const char EMPTY = '□';

    [Header("Parameters")]
    [SerializeField] private int _inputDelay;
    [SerializeField] private List<DirectionInput> _inputs = new List<DirectionInput>();
    [SerializeField] private float[] _registeredInputs;
    [SerializeField] private float[] _scriptedInputs;

    [Header("Refs")]
    [SerializeField] private Background _background;
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
        _registeredInputs = new float[4];
        _scriptedInputs = new float[4];
        _counters.Clear();
        _onBeatObservers.Clear();

        _sfxManager.Init();

        ReadBeatFile();
    }

    private void Update() {
        float time = Time.timeSinceLevelLoad;

        foreach (BeatCounter c in _counters)
            BeatUpdate(time, c);

        RegisterInputs(time);
    }


    private void RegisterInputs(float time) {
        bool pressed = false;

        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            _registeredInputs[0] = time;
            pressed = true;
            Debug.Log(_scriptedInputs[0] - _registeredInputs[0]);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow)) {
            _registeredInputs[1] = time;
            pressed = true;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            _registeredInputs[2] = time;
            pressed = true;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            _registeredInputs[3] = time;
            pressed = true;
        }

        if (!pressed)
            return;

        _background.OnBeat(_inputs[0], _secondsPerBeat / (float)_beatDiv, GetDelay());
    }

    private void BeatUpdate(float time, BeatCounter counter) {
        int value = _sfxManager.CalculateCurrentBeat(_secondsPerBeat, _offset, counter.BeatDiv);

        if (counter.IsAFullBeat(value)) {
            DirectionInput input = counter.Input;
            _scriptedInputs[counter.Input.Index] = time;

            _sfxManager.PlayAudio(input.AudioClip.name, 1, false);

            foreach (IBeatObserver observer in _onBeatObservers)
                observer.OnBeat(input, _secondsPerBeat / (float)_beatDiv, 0);
        }

        if (counter.IsAFullInputBeat(value + GetDelay())) {
            DirectionInput input = counter.Input;

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
