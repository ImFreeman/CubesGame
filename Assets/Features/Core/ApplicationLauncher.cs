using Assets.Features.Cube.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

public class ApplicationLauncher : IDisposable
{
    private IDisposable _disposable;
    public ApplicationLauncher(
        IInstantiator instantiator,
        CubeView.Pool cubesPool,
        UIMainWindow mainWindow,
        Canvas canvas,
        IDictionary<int, string> cubeTypes,
        [Inject(Id = CubesContainerType.Scroll)] IReactiveCollection<CubeView> collection
        )
    {
        var cubeCommand = instantiator.Instantiate<SpawnCubeCommand>(new object[] { "Red" });
        var cube = cubeCommand.Do().Item2;

        var cubeCommand0 = instantiator.Instantiate<SpawnCubeCommand>(new object[] { "Green" });
        var cubeGreen = cubeCommand0.Do().Item2;

        var cubeCommand1 = instantiator.Instantiate<SpawnCubeCommand>(new object[] { "Blue" });
        var cubeBlue = cubeCommand1.Do().Item2;

        //cube.transform.SetParent(mainWindow.RectTransform);
        //cube.transform.localPosition = Vector3.zero;

        cubeTypes.Add(cube.GetInstanceID(), "Red");
        cubeTypes.Add(cubeBlue.GetInstanceID(), "Blue");
        cubeTypes.Add(cubeGreen.GetInstanceID(), "Green");

        collection.Add(cube);
        collection.Add(cubeBlue);
        collection.Add(cubeGreen);

        /*
        var cube = cubeCommand.Do().Item2;
        cube.transform.SetParent(mainWindow.RectTransform);
        cube.transform.localPosition = Vector3.zero;

        _disposable = cube
            .OnBeginDragAsObservable()
            .Subscribe(_ => 
            { 
                var view = cubeCommand.Do().Item2;
                view.transform.SetParent(mainWindow.RectTransform);
                view.transform.localPosition = Vector3.zero;
                view.OnDragAsObservable()
                .TakeUntil(view.OnEndDragAsObservable())
                .Subscribe(x => { view.RectTransform.anchoredPosition += x.delta / canvas.scaleFactor; }, () => { cubesPool.Despawn(view); });                
                
            });
        /*
        cube
            .OnDragAsObservable()
            .Subscribe(_ => { Debug.Log("OnDrag"); });

        cube
            .OnEndDragAsObservable()
            .Subscribe(_ => { Debug.Log("End drage"); });*/
    }

    public void Dispose()
    {
        _disposable.Dispose();
    }
}
