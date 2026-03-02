using Assets.Features.Core.Command;
using Assets.Features.Cube.Scripts;
using System;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class CubesScroll : IDisposable
{
    private ScrollRect _scrollRect;    
    private IReadOnlyReactiveCollection<CubeView> _scrollCollection;
    private IReactiveCollection<CubeView> _dndCollection;
    private IDictionary<int, string> _cubeTypes;
    private IInstantiator _instantiator;
    private UIMainWindow _window;    

    private Dictionary<string, ICommand<CubeView>> _commandsDict = new Dictionary<string, ICommand<CubeView>>();
    private CompositeDisposable _compositeDisposable = new CompositeDisposable();

    public CubesScroll(
        [Inject(Id = CubesContainerType.Scroll)] IReactiveCollection<CubeView> scrollCollection,
        [Inject(Id = CubesContainerType.DragAndDrop)] IReactiveCollection<CubeView> dndCollection,
        IDictionary<int, string> cubeTypes,
        UIMainWindow window,
        IInstantiator instantiator)
    {
        _dndCollection = dndCollection;
        _window = window;
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
        _compositeDisposable.Dispose();
    }

    private void OnObjectAdded(CollectionAddEvent<CubeView> collectionAddEvent)
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

    private void AddViewToScroll(CubeView view)
    {
        view.RectTransform.SetParent(_scrollRect.content);               
    }

    private void OnDragBegin((PointerEventData pointerData, string color) data)
    {        
        if(_commandsDict.TryGetValue(data.color, out var command))
        {
            var commandResult = command.Do();
            if(commandResult.Item1 == CommandStatus.Success)
            {
                var currentDraggable = commandResult.Item2;
                currentDraggable.RectTransform.SetAsLastSibling();

                currentDraggable.GetComponent<Image>().raycastTarget = false;//TODO: refactor
                currentDraggable.transform.SetParent(_window.RectTransform);//TODO: refactor
                currentDraggable.transform.position = data.pointerData.pointerDrag.transform.position;

                data.pointerData.pointerDrag = currentDraggable.gameObject;

                ExecuteEvents.Execute(currentDraggable.gameObject, data.pointerData, ExecuteEvents.beginDragHandler);

                if(!_dndCollection.Contains(currentDraggable))
                {
                    _dndCollection.Add(currentDraggable);
                }                
            }
        }
    }

    
}
