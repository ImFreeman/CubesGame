using Assets.Features.Core;
using Assets.Features.Cube.Scripts;
using Assets.Features.Localization.Scripts.Interfaces;
using Assets.Features.Localization.Scripts.Realizations;
using System.Collections.Generic;
using UniRx;
using UniRx.Diagnostics;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ApplicationInstaller : MonoInstaller
{
    [SerializeField] private CubeConfig _config;
    [SerializeField] private CubeView _view;
    [SerializeField] private LocalizationConfig _language;
    public override void InstallBindings()
    {
        Container
            .Bind<UniRx.Diagnostics.Logger>()
            .AsSingle()
            .WithArguments("MainLogger");

        Container
            .Bind<LocalizationConfig>()
            .FromScriptableObject(_language)
            .AsSingle();

        Container
            .Bind<ILocalizationManager>()
            .To<LocalizationManager>()
            .AsSingle();

        Container
            .Bind<CubeConfig>()
            .FromScriptableObject(_config)
            .AsSingle();

        Container
            .Bind<IReactiveCollection<CubeView>>()
            .WithId(CubesContainerType.Scroll)
            .To<ReactiveCollection<CubeView>>()            
            .AsCached();

        Container
            .Bind<IReactiveCollection<CubeView>>()
            .WithId(CubesContainerType.Tower)
            .To<ReactiveCollection<CubeView>>()            
            .AsCached();

        Container
            .Bind<IReactiveCollection<CubeView>>()
            .WithId(CubesContainerType.DragAndDrop)
            .To<ReactiveCollection<CubeView>>()            
            .AsCached();

        Container
            .Bind<IDictionary<int, string>>()
            .To<Dictionary<int, string>>()
            .AsSingle();

        Container
            .BindMemoryPool<CubeView, CubeView.Pool>()
            .WithInitialSize(30)
            .FromComponentInNewPrefab(_view)
            .AsSingle();

        Container
            .Bind<DragAndDropHandler>()
            .AsSingle()
            .NonLazy();

        Container
            .Bind<CubesScroll>()
            .AsSingle()
            .NonLazy();

        Container
            .Bind<Hole>()
            .AsSingle()
            .NonLazy();

        Container
            .Bind<Tower<CubeView>>()
            .AsSingle()
            .NonLazy();

        Container
            .Bind<LogHandler>()
            .AsSingle()
            .NonLazy();

        Container
            .Bind<ApplicationLauncher>()
            .AsSingle()
            .NonLazy();

    }
}