using System;
using UniRx;

namespace Assets.Features.Cube.Scripts
{
    public class CubeDespawnHandler : IDisposable
    {
        private CompositeDisposable _disposable = new CompositeDisposable();

        public CubeDespawnHandler(
            ISubject<CubeView> byHole,
            ISubject<CubeView> defaultDespawn,
            CubeView.Pool pool)
        {
            //defaultDespawn
              //  .Subscribe()
        }

        public void Dispose()
        {
            
        }
    }
}
