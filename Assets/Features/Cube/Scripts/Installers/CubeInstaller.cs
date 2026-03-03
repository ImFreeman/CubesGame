using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

namespace Assets.Features.Cube.Scripts.Installers
{
    public class CubeInstaller : Installer<CubeView, Transform, CubeInstaller>
    {
        [Inject] private CubeView _view;
        [Inject] private Transform _poolParent;

        public override void InstallBindings()
        {
            Container
                .Bind<IReactiveCollection<UIElement>>()
                .WithId(UIElementsContainerType.Scroll)
                .To<ReactiveCollection<UIElement>>()
                .AsCached();

            Container
                .Bind<IReactiveCollection<UIElement>>()
                .WithId(UIElementsContainerType.Tower)
                .To<ReactiveCollection<UIElement>>()
                .AsCached();

            Container
                .Bind<IReactiveCollection<UIElement>>()
                .WithId(UIElementsContainerType.DragAndDrop)
                .To<ReactiveCollection<UIElement>>()
                .AsCached();

            Container
                .Bind<IDictionary<int, string>>()
                .To<Dictionary<int, string>>()
                .AsSingle();

            Container
                .BindMemoryPool<CubeView, CubeView.Pool>()
                .WithInitialSize(30)
                .FromComponentInNewPrefab(_view)
                .UnderTransform(_poolParent)
                .AsSingle();

            Container.Bind<IReactiveMemoryPool<CubeViewProtocol, CubeView>>().To<CubeView.Pool>().FromResolve();
        }

    }
}