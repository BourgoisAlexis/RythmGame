using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Background : MonoBehaviour, IBeatObserver {
    [SerializeField] private Image _image;

    private Color _initialColor;

    private void Awake() {
        _initialColor = _image.color;
    }

    public void OnBeat(DirectionInput input, float beatDuration, int inputDelay) {
        Anim(beatDuration);
    }

    public async void Anim(float duration) {
        await _image.DOColor(Color.green, duration / 4).AsyncWaitForCompletion();
        _image.DOColor(_initialColor, duration / 2);
    }
}
