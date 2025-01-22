using DG.Tweening;
using UnityEngine;

public class BeatTimeLine : MonoBehaviour, IBeatObserver {
    [SerializeField] private VFXManager _vfxManager;
    [SerializeField] private Transform _bar;

    private Bar b;


    private void Start() {
        b = new Bar(_bar);
        FindFirstObjectByType<BeatManager>().AddObserver(this, true);
        FindFirstObjectByType<BeatManager>().AddObserver(b, false);
    }

    public void OnBeat(DirectionInput input, float beatDuration, int inputDelay) {
        float duration = beatDuration * (float)inputDelay;
        GameObject go = _vfxManager.Dequeue("InputDirection", transform);
        _vfxManager.DelayedEnqueue(go, "InputDirection", duration + 0.2f);
        Transform t = go.transform;
        t.localPosition = new Vector3(800, -100 * input.Index, 0);
        t.eulerAngles = Vector3.forward * input.Rotation;
        t.DOLocalMoveX(0, duration).SetEase(Ease.Linear);
    }
}

public class Bar : IBeatObserver {
    private Transform _t;

    public Bar(Transform t) {
        _t = t;
    }

    public void OnBeat(DirectionInput input, float beatDuration, int inputDelay) {
        Anim(beatDuration);
    }

    private async void Anim(float duration) {
        await _t.DOScaleY(1.2f, duration / 4).AsyncWaitForCompletion();
        _t.DOScaleY(1f, duration / 2);
    }
}
