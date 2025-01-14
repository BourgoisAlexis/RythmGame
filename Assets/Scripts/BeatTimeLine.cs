using DG.Tweening;
using UnityEngine;

public class BeatTimeLine : MonoBehaviour, IBeatObserver {
    [SerializeField] private VFXManager _vfxManager;


    private void Start() {
        FindFirstObjectByType<BeatManager>().AddObserver(this, true);
    }

    public void OnBeat(InputDirection input, float beatDuration, int inputDelay) {
        float duration = beatDuration * (float)inputDelay;
        GameObject go = _vfxManager.Dequeue("InputDirection", transform);
        _vfxManager.DelayedEnqueue(go, "InputDirection", duration);
        Transform t = go.transform;
        t.localPosition = new Vector3(800, -200, 0);
        t.eulerAngles = Vector3.forward * input.Rotation;
        t.DOLocalMoveX(0, duration).SetEase(Ease.Linear);
    }
}
