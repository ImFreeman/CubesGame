using Assets.Features.Cube.Scripts;
using Assets.Features.Localization.Scripts;
using Assets.Features.Localization.Scripts.Interfaces;
using Assets.Features.UI.Scripts;
using UniRx;
using UnityEngine;
using Zenject;

namespace Assets.Features.Tower.Scripts.TowerPlaceCheckHandler.Realization
{
    public class BordersCheck : TowerPlaceCheckerDecorator
    {
        private RectTransform _container;
        private IReactiveCollection<UIElement> _towerCollection;
        private ILocalizationManager _localizationManager;
        private UniRx.Diagnostics.Logger _logger;

        public BordersCheck(
            [Inject(Id = UIElementsContainerType.Tower)]
            IReactiveCollection<UIElement> collection,
            UIMainWindow window,
            ILocalizationManager localizationManager,
            UniRx.Diagnostics.Logger logger)
        {
            _towerCollection = collection;
            _container = window.TowerCubeContainer;
            _localizationManager = localizationManager;
            _logger = logger;
        }

        public override void Dispose()
        {
            base.Dispose();
            _container = null;
            _localizationManager = null;
            _logger = null;
            _towerCollection = null;
            _container = null;
        }

        public override (bool isCanPlace, string errorMessage) CanPlace(UIElement item)
        {
            var result = base.CanPlace(item);
            if(!result.isCanPlace)
            {
                return result;
            }

            float yEdge = TowerConsts.Ground * _container.rect.height;
            foreach(UIElement element in _towerCollection)
            {
                yEdge += element.RectTransform.rect.height;
            }

            if (yEdge + item.RectTransform.rect.height > _container.rect.height)
            {
                return (false, _localizationManager.Localize(LocalizationConsts.OnMaxHeightReached));
            }

            if (item.RectTransform.anchoredPosition.y - item.RectTransform.rect.height * item.RectTransform.pivot.y < yEdge)
            {                
                return (false, _localizationManager.Localize(LocalizationConsts.OnCubeDespawn));
            }            

            var referenceCube = _towerCollection[_towerCollection.Count - 1].RectTransform;
            var refCubePos = referenceCube.anchoredPosition;
            var xEdge = new Vector2(
                refCubePos.x - referenceCube.rect.width * referenceCube.pivot.x,
                refCubePos.x + referenceCube.rect.width * (1 - referenceCube.pivot.x)
                );

            var viewPos = item.RectTransform.anchoredPosition;
            var viewXEdge = new Vector2(
                viewPos.x - item.RectTransform.rect.width * item.RectTransform.pivot.x,
                viewPos.x + item.RectTransform.rect.width * (1 - item.RectTransform.pivot.x)
                );

            if (viewXEdge.y < xEdge.x || viewXEdge.x > xEdge.y)
            {
                return (false, _localizationManager.Localize(LocalizationConsts.OnCubeDespawn));
            }

            return (true, string.Empty);
        }        
    }
}
