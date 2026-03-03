using Assets.Features.Core.Command;
using Assets.Features.Core.Command.Interfaces;
using System.Collections.Generic;
using UniRx;
using Zenject;

namespace Assets.Features.Cube.Scripts
{
    public class FillCubeScrollCommand : ICommand<CommandReturnValue>
    {
        private CubeConfig _cubeConfig;
        private IReactiveCollection<UIElement> _scrollCollection;
        private IReactiveMemoryPool<CubeViewProtocol, CubeView> _pool;
        private IDictionary<int, string> _cubeTypes;

        public FillCubeScrollCommand(
            CubeConfig cubeConfig,
            IReactiveMemoryPool<CubeViewProtocol, CubeView> pool,
            [Inject(Id = UIElementsContainerType.Scroll)]
            IReactiveCollection<UIElement> collection,
            IDictionary<int, string> cubeTypes)
        {
            _scrollCollection = collection;
            _cubeConfig = cubeConfig;
            _pool = pool;
            _cubeTypes = cubeTypes;
        }

        public void Dispose()
        {
            _cubeConfig = null;
            _scrollCollection = null;
            _pool = null;
            _cubeTypes = null;
        }

        public (CommandStatus, CommandReturnValue) Do()
        {            
            foreach(string color in _cubeConfig.CubesToShow)
            {
                var model = _cubeConfig.Get(color);
                if (model != null)
                {
                    var view = _pool.Spawn(new CubeViewProtocol(model.Value.Sprite));
                    if (!_cubeTypes.TryAdd(view.GetInstanceID(), color))
                    {
                        return (CommandStatus.Failed, CommandReturnValue.Empty);
                    }
                    _scrollCollection.Add(view);                    
                }
            }

            return (CommandStatus.Success, CommandReturnValue.Empty);
        }
    }
}
