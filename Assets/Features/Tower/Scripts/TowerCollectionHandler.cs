using Assets.Features.Cube.Scripts;
using Assets.Features.Localization.Scripts;
using Assets.Features.Localization.Scripts.Interfaces;
using Assets.Features.UI.Scripts;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

namespace Assets.Features.Tower.Scripts
{
    public class TowerCollectionHandler : IDisposable
    {
        private IReactiveCollection<UIElement> _towerCollection;
        private ILocalizationManager _localizationManager;
        private UniRx.Diagnostics.Logger _logger;

        private CompositeDisposable _compositeDisposable = new CompositeDisposable();
        private RectTransform _elementsContainer;

        private Vector2 _nextPos;

        private List<Tween> _tweens = new List<Tween>();
        private List<Sequence> _sequences = new List<Sequence>();
        private System.Random _random = new System.Random();

        public TowerCollectionHandler(
            UniRx.Diagnostics.Logger logger,
            ILocalizationManager localizationManager,
            [Inject(Id = UIElementsContainerType.Tower)]
            IReactiveCollection<UIElement> towerCollection,
            UIMainWindow window)
        {
            _logger = logger;            
            _localizationManager = localizationManager;
            _towerCollection = towerCollection;
            _elementsContainer = window.TowerCubeContainer;

            _towerCollection
                .ObserveAdd()
                .Subscribe(OnItemAdded)
                .AddTo(_compositeDisposable);

            _towerCollection
                .ObserveRemove()
                .Subscribe(OnItemRemoved)
                .AddTo(_compositeDisposable);
        }

        public void Dispose()
        {
            _logger = null;
            _localizationManager = null;
            _towerCollection = null;
            _elementsContainer = null; 

            foreach(var sequence in _sequences)
            {
                sequence.Kill();
            }
            _sequences.Clear();
            _sequences = null;

            foreach(var tween in _tweens)
            {
                tween.Kill();
            }
            _tweens.Clear();
            _tweens = null;

            _compositeDisposable.Dispose();
            _compositeDisposable = null;
        }

        private void OnItemRemoved(CollectionRemoveEvent<UIElement> itemRemoveEvent)
        {
            var sequence = DOTween.Sequence();
            for (int i = itemRemoveEvent.Index; i < _towerCollection.Count; i++)
            {
                var towerItem = _towerCollection[i];
                var tween = towerItem.RectTransform
                    .DOAnchorPos(
                        new Vector2(
                            towerItem.RectTransform.anchoredPosition.x,
                            towerItem.RectTransform.anchoredPosition.y - _towerCollection[i].RectTransform.rect.height
                            ),
                        0.5f
                    )
                    .SetEase(Ease.InBack)
                    .OnComplete(() => { towerItem.Graphics.raycastTarget = true; });

                towerItem.Graphics.raycastTarget = false;

                if (i == itemRemoveEvent.Index)
                {
                    sequence.Append(tween);
                }
                else
                {
                    sequence.Join(tween);
                }
            }

            sequence.OnComplete(() => { _sequences.Remove(sequence); });
            _sequences.Add(sequence);

            if(_towerCollection.Count != 0)
            {
                var lastObj = _towerCollection[_towerCollection.Count - 1].RectTransform;
                _nextPos = new Vector2(
                           Mathf.Clamp(
                               lastObj.anchoredPosition.x,
                               lastObj.rect.width * lastObj.pivot.x,
                               _elementsContainer.rect.width - lastObj.rect.width * (1 - lastObj.pivot.x)
                               ),
                           _elementsContainer.rect.height * TowerConsts.Ground + lastObj.rect.height * lastObj.pivot.y
                           );
                CalculateNextPos();
            }            
        }

        private void OnItemAdded(CollectionAddEvent<UIElement> itemAddEvent)
        {
            var rectTransform = itemAddEvent.Value.GetComponent<RectTransform>();
            rectTransform.SetParent(_elementsContainer);
            if (_towerCollection.Count == 1)
            {
                _nextPos = new Vector2(
                            Mathf.Clamp(
                                rectTransform.anchoredPosition.x,
                                rectTransform.rect.width * rectTransform.pivot.x,
                                _elementsContainer.rect.width - rectTransform.rect.width * (1 - rectTransform.pivot.x)
                                ),
                            _elementsContainer.rect.height * TowerConsts.Ground + rectTransform.rect.height * rectTransform.pivot.y
                            );                
            }

            var tween = rectTransform
                    .DOAnchorPos(_nextPos,0.5f)
                    .SetEase(Ease.InBack);

            tween.OnComplete(() => { _tweens.Remove(tween); });
            _tweens.Add(tween);

            CalculateNextPos();

            itemAddEvent.Value.Graphics.raycastTarget = true;

            itemAddEvent.Value
                .OnBeginDragAsObservable()
                .Take(1)
                .Subscribe(_ =>
                {
                    itemAddEvent.Value.Graphics.raycastTarget = false;
                    _towerCollection.Remove(itemAddEvent.Value);
                });

            _logger.Log(_localizationManager.Localize(LocalizationConsts.OnTowerAdded));
        }

        private void CalculateNextPos()
        {
            var lastCube = _towerCollection[_towerCollection.Count - 1];
            var lastCubeRect = lastCube.RectTransform;
            float offset = (float)_random.NextDouble() * lastCubeRect.rect.width;
            var newX = _nextPos.x - lastCubeRect.rect.width * 0.5f + offset;
            var newY = _elementsContainer.rect.height * TowerConsts.Ground + lastCubeRect.rect.height * _towerCollection.Count + lastCubeRect.rect.height * lastCubeRect.pivot.y;
            _nextPos = new Vector2(
                Mathf.Clamp(
                    newX,
                    lastCubeRect.rect.width * lastCubeRect.pivot.x,
                    _elementsContainer.rect.width - lastCubeRect.rect.width * (1 - lastCubeRect.pivot.x)
                    ),
                newY);
        }
    }
}
