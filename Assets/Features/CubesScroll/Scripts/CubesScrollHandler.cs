using Assets.Features.Core.Command;
using Assets.Features.Core.Command.Interfaces;
using Assets.Features.Cube.Scripts;
using Assets.Features.UI.Scripts;
using System;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Assets.Features.CubesScroll.Scripts
{
    public class CubesScrollHandler : IDisposable
    {
        private ScrollRect _scrollRect;
        private IReadOnlyReactiveCollection<UIElement> _scrollCollection;
        private IReactiveCollection<UIElement> _dndCollection;
        private IDictionary<int, string> _cubeTypes;
        private IInstantiator _instantiator;
        private RectTransform _itemsContainer;

        private Dictionary<string, ICommand<CubeView>> _commandsDict = new Dictionary<string, ICommand<CubeView>>();
        private CompositeDisposable _compositeDisposable = new CompositeDisposable();

        public CubesScrollHandler(
            [Inject(Id = UIElementsContainerType.Scroll)] IReactiveCollection<UIElement> scrollCollection,
            [Inject(Id = UIElementsContainerType.DragAndDrop)] IReactiveCollection<UIElement> dndCollection,
            IDictionary<int, string> cubeTypes,
            UIMainWindow window,
            IInstantiator instantiator)
        {
            _dndCollection = dndCollection;
            _itemsContainer = window.TowerCubeContainer;
            _cubeTypes = cubeTypes;
            _scrollRect = window.Scroll;
            _scrollCollection = scrollCollection;
            _instantiator = instantiator;

            _scrollCollection
                .ObserveAdd()
                .Subscribe(OnObjectAdded)
                .AddTo(_compositeDisposable);
        }

        public void Dispose()
        {
            _scrollRect = null;
            _scrollCollection = null;
            _dndCollection = null;
            _cubeTypes = null;
            _instantiator = null;
            _itemsContainer = null;

            foreach (var command in _commandsDict.Values)
            {
                command.Dispose();
            }
            _commandsDict.Clear();
            _commandsDict = null;

            _compositeDisposable.Dispose();
            _compositeDisposable = null;
        }

        private void OnObjectAdded(CollectionAddEvent<UIElement> collectionAddEvent)
        {
            var view = collectionAddEvent.Value;

            if (_cubeTypes.TryGetValue(view.GetInstanceID(), out var color))
            {
                AddViewToScroll(view);

                if (!_commandsDict.ContainsKey(color))
                {
                    _commandsDict.Add(color, _instantiator.Instantiate<SpawnCubeCommand>(new object[] { color }));
                }

                view
                    .OnBeginDragAsObservable()
                    .Select(pointerData => (pointerData, color))
                    .TakeUntil(_scrollCollection.ObserveRemove().Where(removedItem => view == removedItem.Value))
                    .Subscribe(OnDragBegin)
                    .AddTo(_compositeDisposable);

                view.OnEndDragAsObservable();
                view.OnDragAsObservable();
            }
        }

        private void AddViewToScroll(UIElement view)
        {
            view.RectTransform.SetParent(_scrollRect.content);
        }

        private void OnDragBegin((PointerEventData pointerData, string color) data)
        {
            if (_commandsDict.TryGetValue(data.color, out var command))
            {
                var commandResult = command.Do();
                if (commandResult.Item1 == CommandStatus.Success)
                {
                    var currentDraggable = commandResult.Item2;
                    currentDraggable.RectTransform.SetAsLastSibling();

                    currentDraggable.Graphics.raycastTarget = false;
                    currentDraggable.RectTransform.SetParent(_itemsContainer);
                    currentDraggable.RectTransform.position = data.pointerData.pointerDrag.transform.position;

                    data.pointerData.pointerDrag = currentDraggable.gameObject;

                    ExecuteEvents.Execute(currentDraggable.gameObject, data.pointerData, ExecuteEvents.beginDragHandler);

                    if (!_dndCollection.Contains(currentDraggable))
                    {
                        _dndCollection.Add(currentDraggable);
                    }
                }
            }
        }


    }
}