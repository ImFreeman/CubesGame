using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Hole
{
    private Image _holeView;

    public Hole(UIMainWindow window, CubeView.Pool pool)
    {
        _holeView = window.Hole;
        _holeView.alphaHitTestMinimumThreshold = 1.0f;

        _holeView
            .OnDropAsObservable()
            .Subscribe(pointerData => 
            {                
                //TODO: animation
                var cube = pointerData.pointerDrag.GetComponent<CubeView>();
                if (cube != null)
                {
                    pool.Despawn(cube);
                }
            });
    }
}
