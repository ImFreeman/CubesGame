using Assets.Features.Cube.Scripts;
using DG.Tweening;
using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class Tower<T> : IDisposable where T : CubeView
{
    private float _ground = 158f;
    private CompositeDisposable _compositeDisposable = new CompositeDisposable();
    private IReactiveCollection<CubeView> _towerCollection;
    private Image _towerBackground;
    private CubeView.Pool _pool;

    private Vector2 _nextPos;
    private float _width;
    private float _height;
    private System.Random _random = new System.Random();
    private Canvas _canvas;
    private RectTransform _cubesContainer;

    public Tower(
        CubeView.Pool pool,
        UIMainWindow window,
        [Inject(Id = CubesContainerType.Tower)] IReactiveCollection<CubeView> collection,
        Canvas canvas)
    {
        _cubesContainer = window.TowerCubeContainer;
        _pool = pool;
        _towerCollection = collection;

        _towerCollection
            .ObserveAdd()
            .Subscribe(OnItemAdded)
            .AddTo(_compositeDisposable);

        _towerCollection
            .ObserveRemove()
            .Subscribe(OnItemRemoved)
            .AddTo(_compositeDisposable);

        _towerBackground = window.Tower;

        _width = _towerBackground.rectTransform.rect.width;
        _height = _towerBackground.rectTransform.rect.height;

        _towerBackground
            .OnDropAsObservable()
            .TakeUntil(_towerBackground.OnDestroyAsObservable())
            .Subscribe(OnDrop)
            .AddTo(_compositeDisposable);
        _canvas = canvas;
    }

    private void OnItemRemoved(CollectionRemoveEvent<CubeView> itemRemoveEvent)
    {
        for (int i = itemRemoveEvent.Index; i < _towerCollection.Count; i++)
        {
            _towerCollection[i].RectTransform
                .DOAnchorPos(
                    new Vector2(
                        _towerCollection[i].RectTransform.anchoredPosition.x,
                        _towerCollection[i].RectTransform.anchoredPosition.y - _towerCollection[i].RectTransform.rect.height
                        ),
                    0.5f
                )
                .SetEase(Ease.InBack);
        }
        CalculateNextPos();
    }

    private void OnItemAdded(CollectionAddEvent<CubeView> itemAddEvent)
    {
        if(_towerCollection.Count == 1)
        {
            var rectTransform = itemAddEvent.Value.GetComponent<RectTransform>();
            rectTransform.SetParent(_cubesContainer);
            rectTransform
                .DOAnchorPos(
                    new Vector2(
                        Mathf.Clamp(rectTransform.anchoredPosition.x, rectTransform.rect.width * rectTransform.pivot.x, _width - rectTransform.rect.width * (1 - rectTransform.pivot.x)),
                        _ground + rectTransform.rect.height * rectTransform.pivot.y
                        ),
                    0.5f
                )
                .SetEase(Ease.InBack);
                //.OnComplete(() => { rectTransform.SetParent(_cubesContainer); });

            CalculateNextPos();
        }
        else
        {
            var rectTransform = itemAddEvent.Value.GetComponent<RectTransform>();
            if (_nextPos.y + rectTransform.rect.height > _height)
            {
                DropCube(itemAddEvent.Value);
                return;
            }
            
            rectTransform.SetParent(_cubesContainer);
            rectTransform
                .DOAnchorPos(
                    _nextPos,
                    0.5f
                )
                .SetEase(Ease.InBack);
                //.OnComplete(() => { rectTransform.SetParent(_cubesContainer); });
            CalculateNextPos();
        }

        itemAddEvent.Value.GetComponent<Image>().raycastTarget = true;

        itemAddEvent.Value
            .OnBeginDragAsObservable()
            .Take(1)
            .Subscribe(_ =>
            {
                _towerCollection.Remove(itemAddEvent.Value);
            });
    }    

    private void DropCube(CubeView cube, bool removeFromCollection = true)
    {
        _pool.Despawn(cube);
        if(removeFromCollection)
        {
            _towerCollection.Remove(cube);
        }        
    }

    private void CalculateNextPos()
    {
        if(_towerCollection.Count == 0)
        {
            return;
        }
        var lastCube = _towerCollection[_towerCollection.Count - 1];
        var lastCubeRect = lastCube.GetComponent<RectTransform>();
        float offset = (float)_random.NextDouble() * lastCubeRect.rect.width;
        var newX = lastCubeRect.anchoredPosition.x - lastCubeRect.rect.height * 0.5f + offset;
        var newY = _ground + lastCubeRect.rect.height * _towerCollection.Count + lastCubeRect.rect.height * lastCubeRect.pivot.y;        
        _nextPos = new Vector2(
            Mathf.Clamp(
                newX,
                lastCubeRect.rect.width * lastCubeRect.pivot.x,
                _width - lastCubeRect.rect.width * (1 - lastCubeRect.pivot.x)
                ),
            newY);
    }

    private void OnDrop(PointerEventData pointerData)
    {
        ExecuteEvents.Execute(pointerData.pointerDrag.gameObject, pointerData, ExecuteEvents.endDragHandler);
        if (pointerData.pointerDrag.TryGetComponent<T>(out var view))
        {            
            view.RectTransform.SetParent(_towerBackground.rectTransform);
            if (_towerCollection.Count == 0)
            {
                _towerCollection.Add(view);
            }
            else
            {
                var yEdge = _ground + view.RectTransform.rect.height * _towerCollection.Count;
                if (view.RectTransform.anchoredPosition.y - view.RectTransform.rect.height * view.RectTransform.pivot.y < yEdge)
                {
                    DropCube(view, false);
                    return;
                }

                var referenceCube = _towerCollection[0].RectTransform;
                var refCubePos = referenceCube.anchoredPosition;
                var xEdge = new Vector2(
                    refCubePos.x - referenceCube.rect.width * referenceCube.pivot.x,
                    refCubePos.x + referenceCube.rect.width * (1 - referenceCube.pivot.x)
                    );

                var viewPos = view.RectTransform.anchoredPosition;
                var viewXEdge = new Vector2(
                    viewPos.x - view.RectTransform.rect.width * view.RectTransform.pivot.x,
                    viewPos.x + view.RectTransform.rect.width * (1 - view.RectTransform.pivot.x)
                    );

                if(viewXEdge.y < xEdge.x || viewXEdge.x > xEdge.y)
                {
                    DropCube(view, false);
                    return;
                }

                _towerCollection.Add(view);
            }
        }        
    }

    public void Dispose()
    {
    }
}
