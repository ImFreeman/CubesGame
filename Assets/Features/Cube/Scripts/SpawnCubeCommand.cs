using Assets.Features.Core.Command;

public class SpawnCubeCommand : ICommand<CubeView>
{
    private CubeView.Pool _cubeViewPool;    
    private CubeConfig _cubeConfig;
    private string _name;

    public SpawnCubeCommand(CubeView.Pool cubeViewPool, CubeConfig cubeConfig, string name)
    {
        _cubeViewPool = cubeViewPool;
        _cubeConfig = cubeConfig;
        _name = name;
    }

    public void Dispose()
    {
        _cubeViewPool = null;
        _cubeConfig = null;
        _name = null;
    }

    public (CommandStatus, CubeView) Do()
    {
        var cubeModel = _cubeConfig.Get(_name);
        if (cubeModel.HasValue)
        {
            return (CommandStatus.Success, _cubeViewPool.Spawn(new CubeViewProtocol(cubeModel.Value.Sprite, cubeModel.Value.Color)));
        }
        
        return (CommandStatus.Failed, null);
    }    
}