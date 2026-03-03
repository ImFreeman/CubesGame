using Assets.Features.Localization.Scripts;
using Assets.Features.Localization.Scripts.Interfaces;
using DG.Tweening;
using System;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Assets.Features.Cube.Scripts
{
    public class DragAndDropHandler : IDisposable
    {
        private ILocalizationManager _localizationManager;
        private UniRx.Diagnostics.Logger _logger;
        private IReactiveCollection<UIElement> _collection;
        private IReactiveMemoryPool<CubeViewProtocol, CubeView> _pool;
        private Canvas _canvas;

        private CompositeDisposable _compositeDisposable = new CompositeDisposable();

        public DragAndDropHandler(
            [Inject(Id = UIElementsContainerType.DragAndDrop)]
            IReactiveCollection<UIElement> collection,
            Canvas canvas,
            IReactiveMemoryPool<CubeViewProtocol, CubeView> pool,
            ILocalizationManager localizationManager,
            UniRx.Diagnostics.Logger logger)
        {
            _collection = collection;
            _canvas = canvas;
            _pool = pool;
            _localizationManager = localizationManager;
            _logger = logger;

            _collection
                .ObserveAdd()
                .Subscribe(OnAdd)
                .AddTo(_compositeDisposable);            
        }

        public void Dispose()
        {
            _localizationManager = null;
            _logger = null;
            _pool = null;
            _canvas = null;
            _collection = null;

            _compositeDisposable.Dispose();
            _compositeDisposable = null;
        }

        private void OnAdd(CollectionAddEvent<UIElement> collectionAddEvent)
        {
            collectionAddEvent.Value
                .OnDragAsObservable()
                .TakeUntil(_collection.ObserveRemove().Where(removedItem => removedItem.Value == collectionAddEvent.Value))
                .Subscribe(pointerData =>
                {
                    collectionAddEvent.Value.RectTransform.anchoredPosition += pointerData.delta / _canvas.scaleFactor;
                })
                .AddTo(_compositeDisposable);

            collectionAddEvent.Value
                .OnEndDragAsObservable()
                .TakeUntil(_collection.ObserveRemove().Where(removedItem => removedItem.Value == collectionAddEvent.Value))
                .Subscribe(pointerData =>
                {
                    if (pointerData.pointerEnter == null || pointerData.pointerEnter.GetComponent<IDropHandler>() == null)
                    {
                        collectionAddEvent.Value.RectTransform
                        .DOScale(Vector3.zero, 0.5f)
                        .SetEase(Ease.InElastic)
                        .OnComplete(() => 
                        {
                            _logger.Log(_localizationManager.Localize(LocalizationConsts.OnCubeDespawn));
                            _pool.Despawn(collectionAddEvent.Value);
                        });                        
                    }
                })
                .AddTo(_compositeDisposable);
        }               
    }
}
