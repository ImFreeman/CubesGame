using Assets.Features.Cube.Scripts;
using Assets.Features.SaveSystem.Scripts;
using Assets.Features.Tower.Scripts;
using Assets.Features.Tower.Scripts.TowerPlaceCheckHandler.Realization;
using System;
using System.Collections.Generic;
using UniRx;
using Zenject;

namespace Assets.Features.Core
{
    public class ApplicationLauncher : IDisposable
    {
        private CompositeDisposable _disposables = new CompositeDisposable();

        public ApplicationLauncher(
            OnTowerDropHandler<CubeView, CubeViewProtocol> onTowerDropHandler,
            IInstantiator instantiator,
            IReactiveMemoryPool<CubeViewProtocol, CubeView> cubesPool,
            IDictionary<int, string> cubeTypes,
            [Inject(Id = UIElementsContainerType.DragAndDrop)]
            IReactiveCollection<UIElement> collection
            )
        {            
            var checker = instantiator.Instantiate<BordersCheck>();
            onTowerDropHandler.SetChecker(checker);

            cubesPool.ItemDespawned
                .Subscribe(cubeView =>
                {
                    cubeTypes.Remove(cubeView.GetInstanceID());
                    collection.Remove(cubeView);
                })
                .AddTo(_disposables);

            Observable
                .OnceApplicationQuit()
                .Subscribe(_ => 
                {
                    var command = instantiator.Instantiate<SaveGameCommand>();
                    command.Do();
                    command.Dispose();
                });

            var fillScrollCommand = instantiator.Instantiate<FillCubeScrollCommand>();
            fillScrollCommand.Do();
            fillScrollCommand.Dispose();

            var loadGameCommand = instantiator.Instantiate<LoadGameCommand>();
            Observable
                .Timer(TimeSpan.FromSeconds(0.1f))
                .Subscribe(_ => 
                {
                    loadGameCommand.Do();
                    loadGameCommand.Dispose();
                });            
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}