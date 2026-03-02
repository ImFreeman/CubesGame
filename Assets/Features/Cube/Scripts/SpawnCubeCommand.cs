using Assets.Features.Core.Command;
using System.Collections.Generic;

public class SpawnCubeCommand : ICommand<CubeView>
{
    private CubeView.Pool _cubeViewPool;    
    private CubeConfig _cubeConfig;
    private string _color;
    private IDictionary<int, string> _cubeTypes;

    public SpawnCubeCommand(
        CubeView.Pool cubeViewPool,
        CubeConfig cubeConfig,
        string color,
        IDictionary<int, string> cubeTypes
        )
    {
        _cubeViewPool = cubeViewPool;
        _cubeConfig = cubeConfig;
        _color = color;
        _cubeTypes = cubeTypes;
    }

    public void Dispose()
    {
        _cubeViewPool = null;
        _cubeConfig = null;
        _color = null;
    }

    public (CommandStatus, CubeView) Do()
    {
        var cubeModel = _cubeConfig.Get(_color);
        if (cubeModel.HasValue)
        {            
            var view = _cubeViewPool.Spawn(new CubeViewProtocol(cubeModel.Value.Sprite, cubeModel.Value.Color));
            _cubeTypes.Add(view.GetInstanceID(), _color);
            return (CommandStatus.Success, view);
        }
        
        return (CommandStatus.Failed, null);
    }    
}