using Assets.Features.Core.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using Zenject;

namespace Assets.Features.Cube.Scripts
{
    public class FillCubeScrollCommand : ICommand<CommandReturnValue>
    {
        private CubeConfig _cubeConfig;
        private IReactiveCollection<CubeView> _scrollCollection;
        private CubeView.Pool _pool;
        private IDictionary<int, string> _cubeTypes;

        public FillCubeScrollCommand(
            CubeConfig cubeConfig,
            CubeView.Pool pool,
            [Inject(Id = CubesContainerType.Scroll)]
            IReactiveCollection<CubeView> collection,
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
        }

        public (CommandStatus, CommandReturnValue) Do()
        {            
            foreach(string color in _cubeConfig.CubesToShow)
            {
                var model = _cubeConfig.Get(color);
                if (model != null)
                {
                    var view = _pool.Spawn(new CubeViewProtocol(model.Value.Sprite, model.Value.Color));
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
