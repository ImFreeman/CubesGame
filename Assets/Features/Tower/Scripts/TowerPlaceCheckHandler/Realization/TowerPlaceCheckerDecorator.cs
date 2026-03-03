using Assets.Features.Cube.Scripts;
using Assets.Features.Cube.Scripts.TowerPlaceCheckHandler.Interfaces;

namespace Assets.Features.Tower.Scripts.TowerPlaceCheckHandler.Realization
{
    public abstract class TowerPlaceCheckerDecorator : ITowerPlaceChecker
    {
        public ITowerPlaceChecker _cachedObject;
        public void SetCachedObject(ITowerPlaceChecker checker)
        {
            _cachedObject = checker;
        }
        public virtual void Dispose()
        {
            _cachedObject.Dispose();
            _cachedObject = null;
        }
        public virtual (bool isCanPlace, string errorMessage) CanPlace(UIElement item)
        {
            if (_cachedObject != null)
            {
                return _cachedObject.CanPlace(item);
            }

            return (true, string.Empty);
        }
    }
}