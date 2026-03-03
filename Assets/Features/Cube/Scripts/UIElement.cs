using UnityEngine.EventSystems;
using UnityEngine;
using Zenject;
using UniRx;
using UnityEngine.UI;

namespace Assets.Features.Cube.Scripts
{
    public abstract class UIElement : UIBehaviour
    {
        [SerializeField] protected RectTransform _rectTransform;
        [SerializeField] protected Graphic _graphics;

        public RectTransform RectTransform { get => _rectTransform; }
        public Graphic Graphics { get => _graphics; }        
    }

    public interface IReactiveMemoryPool<TProtocol, TView> : 
        IMemoryPool<TProtocol, TView> 
        where TView : UIElement 
        where TProtocol : struct
    {
        public ISubject<TView> ItemSpawned { get; }
        public ISubject<TView> ItemDespawned { get; }
    }

    public abstract class UIElementsPool<TProtocol, TView> : 
        MonoMemoryPool<TProtocol, TView>,
        IReactiveMemoryPool<TProtocol, TView> 
        where TView : UIElement
        where TProtocol : struct
    {
        public ISubject<TView> ItemSpawned { get; private set; } = new Subject<TView>();
        public ISubject<TView> ItemDespawned { get; private set; } = new Subject<TView>();
        protected override void OnDespawned(TView item)
        {
            base.OnDespawned(item);
            ItemDespawned.OnNext(item);
        }

        protected override void OnSpawned(TView item)
        {
            base.OnSpawned(item);
            ItemSpawned.OnNext(item);
        }
    }
}
