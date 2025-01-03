using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour {
    #region Variables
    private const int INITIAL_SOURCE_NUMBER = 5;
    private const int MAX_SOURCE_NUMBER = 30;

    [SerializeField] private Transform _sourceParent;
    [SerializeField] private List<AudioClip> _clips = new List<AudioClip>();
    [SerializeField] private bool _mute = false;

    private Dictionary<string, AudioClip> _clipDictionnary = new Dictionary<string, AudioClip>();
    private List<AudioSource> _audioSources = new List<AudioSource>();
    private List<AudioSource> _musicSources = new List<AudioSource>();
    private int _currentSourceIndex = 0;
    #endregion


    public void Init(params object[] parameters) {
        if (_sourceParent == null) {
            Debug.LogError("_sourceParent is null");
            return;
        }

        _audioSources.Clear();
        _musicSources.Clear();
        _clipDictionnary.Clear();

        foreach (AudioClip clip in _clips)
            _clipDictionnary.Add(clip.name, clip);

        for (int i = 0; i < INITIAL_SOURCE_NUMBER; i++)
            CreateSource(ref _audioSources);

        for (int i = 0; i < 2; i++)
            CreateSource(ref _musicSources);
    }

    private AudioSource CreateSource(ref List<AudioSource> sourceList) {
        AudioSource source = _sourceParent.gameObject.AddComponent<AudioSource>();
        sourceList.Add(source);
        return source;
    }

    public void PlayAudio(string key, float volume = 1, bool randomPitch = true) {
        if (!CanPlay(key, out AudioClip clip))
            return;

        _currentSourceIndex++;

        if (_currentSourceIndex >= _audioSources.Count)
            _currentSourceIndex = 0;

        AudioSource source = _audioSources[_currentSourceIndex];

        if (source.isPlaying) {
            bool found = false;

            foreach (AudioSource s in _audioSources) {
                if (!s.isPlaying) {
                    source = s;
                    found = true;
                    break;
                }
            }

            if (!found) {
                if (_audioSources.Count >= MAX_SOURCE_NUMBER)
                    return;
                else
                    source = CreateSource(ref _audioSources);
            }
        }

        source.pitch = randomPitch ? Random.Range(0.8f, 1.2f) : 1f;
        source.volume = volume * Random.Range(0.8f, 1.2f);
        source.clip = clip;
        source.Play();
    }

    public void PlayMusic(string key, float volume = 1) {
        if (!CanPlay(key, out AudioClip clip))
            return;

        AudioSource playableSource = _musicSources[0].isPlaying ? _musicSources[1] : _musicSources[0];
        AudioSource stopableSource = _musicSources[0].isPlaying ? _musicSources[0] : _musicSources[1];

        if (playableSource == null)
            return;

        playableSource.volume = volume * Random.Range(0.8f, 1.2f);
        playableSource.clip = clip;
        playableSource.Play();

        stopableSource.Stop();
    }

    private bool CanPlay(string key, out AudioClip clip) {
        clip = null;

        if (_mute)
            return false;

        if (string.IsNullOrEmpty(key) || !_clipDictionnary.ContainsKey(key))
            return false;

        clip = _clipDictionnary[key];

        if (clip == null)
            return false;

        return true;
    }

    public void Mute(bool mute) {
        _mute = mute;
    }


    //RythmGame specific
    public int CalculateCurrentBeat(float secondsPerBeat, int offset, float multiplier) {
        AudioSource source = _musicSources[0];
        AudioClip clip = _musicSources[0].clip;

        return Mathf.FloorToInt((source.timeSamples - offset) / (clip.frequency * (secondsPerBeat / multiplier)));
    }
}
