using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Visual : MonoBehaviour, IBeatObserver {
    #region Variables
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private List<Sprite> _sprites = new List<Sprite>();

    private int _currentIndex;
    #endregion


    private void Start() {
        FindObjectOfType<BeatManager>().AddObserver(this);
    }

    public void OnBeat(int index, float beatDuration) {
        UpdateSprite(index);
        Anim(beatDuration);
    }


    public void UpdateSprite(int index) {
        if (index == _currentIndex)
            return;

        _renderer.sprite = _sprites[index];
        _currentIndex = index;
    }

    public async void Anim(float duration) {
        await transform.DOScaleY(0.8f, duration / 4).AsyncWaitForCompletion();
        transform.DOScaleY(1f, duration / 2);
    }
}
