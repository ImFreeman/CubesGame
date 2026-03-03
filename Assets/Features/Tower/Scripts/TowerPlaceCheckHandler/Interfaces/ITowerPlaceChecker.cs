using System;

namespace Assets.Features.Cube.Scripts.TowerPlaceCheckHandler.Interfaces
{
    public interface ITowerPlaceChecker : IDisposable
    {
        public (bool isCanPlace, string errorMessage) CanPlace(UIElement item);
    }
}
