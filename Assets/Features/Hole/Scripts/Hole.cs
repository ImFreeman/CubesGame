using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Hole : IDisposable
{    
    private IDisposable _disposed;

    public Hole(UIMainWindow window, CubeView.Pool pool)
    {
        var holeView = window.Hole;
        holeView.alphaHitTestMinimumThreshold = 1.0f;

        _disposed = holeView
            .OnDropAsObservable()
            .Subscribe(pointerData => 
            {                                
                var cube = pointerData.pointerDrag.GetComponent<CubeView>();
                if (cube != null)
                {
                    ExecuteEvents.Execute(cube.gameObject, pointerData, ExecuteEvents.endDragHandler);
                    pool.Despawn(cube);
                }
            });
    }

    public void Dispose()
    {
        _disposed.Dispose();
        _disposed = null;
    }
}
