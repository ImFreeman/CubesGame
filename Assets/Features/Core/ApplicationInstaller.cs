using Assets.Features.Cube.Scripts;
using Assets.Features.CubesScroll.Scripts;
using Assets.Features.Hole.Scripts;
using Assets.Features.Tower.Scripts;
using Zenject;

namespace Assets.Features.Core
{
    public class ApplicationInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container
                .Bind<UniRx.Diagnostics.Logger>()
                .AsSingle()
                .WithArguments("MainLogger");

            Container
                .Bind<DragAndDropHandler>()
                .AsSingle()
                .NonLazy();

            Container
                .Bind<CubesScrollHandler>()
                .AsSingle()
                .NonLazy();

            Container
                .Bind<HoleHandler>()
                .AsSingle()
                .NonLazy();

            Container
                .Bind<TowerCollectionHandler>()
                .AsSingle()
                .NonLazy();

            Container
                .Bind<OnTowerDropHandler<CubeView, CubeViewProtocol>>()
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
}