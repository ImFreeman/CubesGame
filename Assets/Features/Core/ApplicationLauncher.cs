using Assets.Features.Cube.Scripts;
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
            var command = instantiator.Instantiate<FillCubeScrollCommand>();
            command.Do();
            command.Dispose();

            var checker = instantiator.Instantiate<BordersCheck>();
            onTowerDropHandler.SetChecker(checker);

            cubesPool.ItemDespawned
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
}