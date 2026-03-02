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
    private CompositeDisposable _disposables = new CompositeDisposable();

    public ApplicationLauncher(
        IInstantiator instantiator,
        CubeView.Pool cubesPool,
        IDictionary<int, string> cubeTypes,
        [Inject(Id = CubesContainerType.DragAndDrop)] IReactiveCollection<CubeView> collection
        )
    {
        instantiator.Instantiate<FillCubeScrollCommand>().Do();

        cubesPool.CubeDespawned
            .Subscribe(cubeView => 
            {
                cubeTypes.Remove(cubeView.GetInstanceID());
                collection.Remove(cubeView);
            })
            .AddTo(_disposables);
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }
}
