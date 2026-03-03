using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class CubeView : UIBehaviour
{
    public RectTransform RectTransform => _rectTransform;

    [SerializeField] private Image _image;
    [SerializeField] private RectTransform _rectTransform;

    

    private void Init(CubeViewProtocol protocol)
    {
        _image.sprite = protocol.Sprite;
        _image.color = protocol.Color;

        _rectTransform.localScale = Vector3.one;
    }
    public class Pool : MonoMemoryPool<CubeViewProtocol, CubeView>
    {
        public ISubject<CubeView> CubeSpawned { get; private set; } = new Subject<CubeView>();
        public ISubject<CubeView> CubeDespawned { get; private set; } = new Subject<CubeView>();
        protected override void OnDespawned(CubeView item)
        {
            base.OnDespawned(item);
            CubeDespawned.OnNext(item);
        }

        protected override void OnSpawned(CubeView item)
        {
            base.OnSpawned(item);
            CubeSpawned.OnNext(item);
        }

        protected override void Reinitialize(CubeViewProtocol p1, CubeView item)
        {
            item.Init(p1);
        }
    }
}

public readonly struct CubeViewProtocol
{
    public readonly Sprite Sprite;    

    public readonly Color Color;
    public CubeViewProtocol(Sprite sprite, Color color) : this()
    {
        Sprite = sprite;
        Color = color;
    }
}
