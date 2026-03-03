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
        private IDictionary<int, float> _offsetData;

        private CompositeDisposable _compositeDisposable = new CompositeDisposable();
        private RectTransform _elementsContainer;

        private Vector2 _prevPos;

        private List<Tween> _tweens = new List<Tween>();
        private List<Sequence> _sequences = new List<Sequence>();
        private System.Random _random = new System.Random();
        
        public TowerCollectionHandler(
            UniRx.Diagnostics.Logger logger,
            ILocalizationManager localizationManager,
            [Inject(Id = UIElementsContainerType.Tower)]
            IReactiveCollection<UIElement> towerCollection,
            UIMainWindow window,
            IDictionary<int, float> offsetData)
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
            _offsetData = offsetData;
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
                            towerItem.RectTransform.anchoredPosition.y - itemRemoveEvent.Value.RectTransform.rect.height
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
            _offsetData.Remove(itemRemoveEvent.Value.GetInstanceID());

            if (_towerCollection.Count != 0)
            {
                var lastObj = _towerCollection[_towerCollection.Count - 1].RectTransform;
                _prevPos.y -= itemRemoveEvent.Value.RectTransform.rect.height;
            }
        }

        private void OnItemAdded(CollectionAddEvent<UIElement> itemAddEvent)
        {
            var rectTransform = itemAddEvent.Value.RectTransform;            

            float posX;
            float posY;

            if(itemAddEvent.Index == 0)
            {                
                if(_offsetData.TryGetValue(itemAddEvent.Value.GetInstanceID(), out var firstOffset))
                {
                    posX = Mathf.Clamp(
                                _elementsContainer.rect.width * firstOffset,
                                rectTransform.rect.width * rectTransform.pivot.x,
                                _elementsContainer.rect.width - rectTransform.rect.width * (1 - rectTransform.pivot.x)
                                );
                }
                else
                {
                    posX = Mathf.Clamp(
                                rectTransform.anchoredPosition.x,
                                rectTransform.rect.width * rectTransform.pivot.x,
                                _elementsContainer.rect.width - rectTransform.rect.width * (1 - rectTransform.pivot.x)
                                );

                    _offsetData.TryAdd(itemAddEvent.Value.GetInstanceID(), rectTransform.anchoredPosition.x / _elementsContainer.rect.width);
                }
                posY = _elementsContainer.rect.height * TowerConsts.Ground + rectTransform.rect.height * rectTransform.pivot.y;                
            }
            else
            {
                float offsetCoef;
                if (!_offsetData.TryGetValue(itemAddEvent.Value.GetInstanceID(), out offsetCoef))
                {
                    offsetCoef = (float)_random.NextDouble();
                    _offsetData.Add(itemAddEvent.Value.GetInstanceID(), offsetCoef);
                }

                var lastCube = _towerCollection[_towerCollection.Count - 1];
                var lastCubeRect = lastCube.RectTransform;

                float offset = offsetCoef * lastCubeRect.rect.width;
                posX = _prevPos.x - lastCubeRect.rect.width * 0.5f + offset;
                posY = _elementsContainer.rect.height * TowerConsts.Ground + lastCubeRect.rect.height * (_towerCollection.Count - 1) + lastCubeRect.rect.height * lastCubeRect.pivot.y;
            }

            _prevPos = new Vector2(posX, posY);

            var tween = rectTransform
                    .DOAnchorPos(_prevPos, 0.5f)
                    .SetEase(Ease.InBack);

            tween.OnComplete(() => { _tweens.Remove(tween); });
            _tweens.Add(tween);

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
    }
}
