using Assets.Features.Cube.Scripts;
using Assets.Features.Cube.Scripts.TowerPlaceCheckHandler.Interfaces;
using Assets.Features.Localization.Scripts.Interfaces;
using Assets.Features.UI.Scripts;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Assets.Features.Tower.Scripts
{
    public class OnTowerDropHandler<TView, TProtocol> 
        : IDisposable 
        where TView : UIElement 
        where TProtocol : struct
    {
        private Graphic _dropPlace;
        private RectTransform _elementsContainer;
        private IReactiveCollection<UIElement> _towerCollection;
        private ILocalizationManager _localizationManager;
        private UniRx.Diagnostics.Logger _logger;
        private IReactiveMemoryPool<TProtocol, TView> _pool;
        private ITowerPlaceChecker _checker;

        private List<Tween> _tweens = new List<Tween>();
        private CompositeDisposable _disposables = new CompositeDisposable();
        public OnTowerDropHandler(
            IReactiveMemoryPool<TProtocol, TView> pool,
            UIMainWindow window,
            [Inject(Id = UIElementsContainerType.Tower)]
            IReactiveCollection<UIElement> collection,
            ILocalizationManager localizationManager,
            UniRx.Diagnostics.Logger logger)
        {
            _elementsContainer = window.TowerCubeContainer;
            _pool = pool;
            _dropPlace = window.TowerDropPlace;
            _logger = logger;
            _localizationManager = localizationManager;
            _towerCollection = collection;

            _dropPlace
                .OnDropAsObservable()
                .TakeUntil(_dropPlace.OnDestroyAsObservable())
                .Subscribe(OnDrop)
                .AddTo(_disposables);
        }

        public void Dispose()
        {
            _pool = null;
            _dropPlace = null;
            _elementsContainer = null;
            _towerCollection = null;
            _localizationManager = null;
            _logger = null;

            foreach (var tween in _tweens)
            {
                tween.Kill();
            }
            _tweens.Clear();
            _tweens = null;

            _checker.Dispose();
            _checker = null;

            _disposables.Dispose();
            _disposables = null;
        }

        public void SetChecker(ITowerPlaceChecker checker)
        {
            _checker = checker;
        }

        private void OnDrop(PointerEventData pointerData)
        {
            ExecuteEvents.Execute(pointerData.pointerDrag.gameObject, pointerData, ExecuteEvents.endDragHandler);
            if (pointerData.pointerDrag.TryGetComponent<TView>(out var view))
            {
                view.RectTransform.SetParent(_elementsContainer);
                if (_towerCollection.Count == 0)
                {
                    _towerCollection.Add(view);
                }
                else
                {
                    var canPlaceItem = CanPlaceItem(view);
                    if(canPlaceItem.check)
                    {
                        _towerCollection.Add(view);
                    }
                    else
                    {
                        DropCube(view, canPlaceItem.message);
                    }                                        
                }
            }
        }

        private (bool check, string message) CanPlaceItem(TView item)
        {
            if(_checker != null)
            {
                return _checker.CanPlace(item);
            }

            return (true, string.Empty);
        }

        private void DropCube(TView uiElement, string message)
        {
            uiElement.GetComponent<Graphic>().raycastTarget = false;
            var tween = uiElement.RectTransform
                .DOScale(Vector3.zero, 0.5f)
                .SetEase(Ease.InElastic);
            tween.OnComplete(() =>
            {
                _tweens.Remove(tween);
                _logger.Log(message);
                _pool.Despawn(uiElement);
            });
            _tweens.Add(tween);
        }

    }
}
