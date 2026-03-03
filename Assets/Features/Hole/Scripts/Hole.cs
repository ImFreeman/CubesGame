using Assets.Features.Localization.Scripts;
using Assets.Features.Localization.Scripts.Interfaces;
using DG.Tweening;
using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;

public class Hole : IDisposable
{    
    private IDisposable _disposed;
    private ILocalizationManager _localizationManager;
    private UniRx.Diagnostics.Logger _logger;

    public Hole(
        UIMainWindow window,
        CubeView.Pool pool,
        ILocalizationManager localizationManager,
        UniRx.Diagnostics.Logger logger)
    {
        var holeView = window.Hole;
        holeView.alphaHitTestMinimumThreshold = 1.0f;

        _logger = logger;
        _localizationManager = localizationManager;
        _disposed = holeView
            .OnDropAsObservable()
            .Subscribe(pointerData =>
            {
                var cube = pointerData.pointerDrag.GetComponent<CubeView>();
                if (cube != null)
                {
                    ExecuteEvents.Execute(cube.gameObject, pointerData, ExecuteEvents.endDragHandler);
                    cube.RectTransform.DOMove(window.Hole.transform.position, 1.0f);
                    cube.RectTransform.DORotate(new Vector3(0, 0, 1080), 1.0f, RotateMode.FastBeyond360);
                    cube.RectTransform.DOScale(Vector3.zero, 1.0f).OnComplete(() =>
                    {
                        _logger.Log(localizationManager.Localize(LocalizationConsts.OnCubeThrow));
                        pool.Despawn(cube);
                    });
                }
            });
        
        
    }

    public void Dispose()
    {
        _disposed.Dispose();
        _disposed = null;
    }
}
