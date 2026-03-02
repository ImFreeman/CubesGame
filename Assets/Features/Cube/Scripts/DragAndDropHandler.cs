using System;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Assets.Features.Cube.Scripts
{
    public class DragAndDropHandler : IDisposable
    {
        private IReactiveCollection<CubeView> _collection;
        private CubeView.Pool _pool;
        private Canvas _canvas;

        private CompositeDisposable _compositeDisposable = new CompositeDisposable();

        public DragAndDropHandler(
            [Inject(Id = CubesContainerType.DragAndDrop)] IReactiveCollection<CubeView> collection,
            Canvas canvas,
            CubeView.Pool pool)
        {
            _collection = collection;
            _canvas = canvas;
            _pool = pool;

            _collection
                .ObserveAdd()
                .Subscribe(OnAdd)
                .AddTo(_compositeDisposable);            
        }

        private void OnAdd(CollectionAddEvent<CubeView> collectionAddEvent)
        {
            collectionAddEvent.Value
                .OnDragAsObservable()
                .TakeUntil(_collection.ObserveRemove().Where(removedItem => removedItem.Value == collectionAddEvent.Value))
                .Subscribe(pointerData =>
                {
                    collectionAddEvent.Value.GetComponent<Graphic>().raycastTarget = false;
                    collectionAddEvent.Value.RectTransform.anchoredPosition += pointerData.delta / _canvas.scaleFactor;
                })
                .AddTo(_compositeDisposable);

            collectionAddEvent.Value
                .OnEndDragAsObservable()
                .TakeUntil(_collection.ObserveRemove().Where(removedItem => removedItem.Value == collectionAddEvent.Value))
                .Subscribe(pointerData =>
                {
                    collectionAddEvent.Value.GetComponent<Graphic>().raycastTarget = true;
                    if (pointerData.pointerEnter == null || pointerData.pointerEnter.GetComponent<IDropHandler>() == null)
                    {
                        _pool.Despawn(collectionAddEvent.Value);
                    }
                })
                .AddTo(_compositeDisposable);
        }
        
        public void Dispose()
        {
            _pool = null;
            _canvas = null;
            _collection = null;

            _compositeDisposable.Dispose();
            _compositeDisposable = null;
        }
    }
}
