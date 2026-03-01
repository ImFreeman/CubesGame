using Assets.Features.Cube.Scripts;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ApplicationInstaller : MonoInstaller
{
    [SerializeField] private CubeConfig _config;
    [SerializeField] private CubeView _view;
    public override void InstallBindings()
    {
        Container
            .Bind<CubeConfig>()
            .FromScriptableObject(_config)
            .AsSingle();

        Container
            .Bind<IReactiveCollection<CubeView>>()
            .WithId(CubesContainerType.Scroll)
            .To<ReactiveCollection<CubeView>>()            
            .AsSingle();

        Container
            .Bind<IDictionary<int, string>>()
            .To<Dictionary<int, string>>()
            .AsSingle();

        Container
            .BindMemoryPool<CubeView, CubeView.Pool>()
            .WithInitialSize(5)
            .FromComponentInNewPrefab(_view)
            .AsSingle();

        Container
            .Bind<CubesScroll>()
            .AsSingle()
            .NonLazy();

        Container
            .Bind<Hole>()
            .AsSingle()
            .NonLazy();

        Container
            .Bind<ApplicationLauncher>()
            .AsSingle()
            .NonLazy();

    }
}