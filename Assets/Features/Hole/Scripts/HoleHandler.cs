using Assets.Features.Cube.Scripts;
using Assets.Features.Localization.Scripts;
using Assets.Features.Localization.Scripts.Interfaces;
using Assets.Features.UI.Scripts;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Features.Hole.Scripts
{
    public class HoleHandler : IDisposable
    {
        private IDisposable _disposed;
        private ILocalizationManager _localizationManager;
        private UniRx.Diagnostics.Logger _logger;
        private List<Sequence> _sequences = new List<Sequence>();


        public HoleHandler(
            UIMainWindow window,
            IReactiveMemoryPool<CubeViewProtocol, CubeView> pool,
            ILocalizationManager localizationManager,
            UniRx.Diagnostics.Logger logger)
        {
            var holeView = window.HoleDropPlace;
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
                        var sequence = DOTween.Sequence();
                        sequence.Append(cube.RectTransform.DOMove(window.HoleDropPlace.transform.position, 1.0f));
                        sequence.Join(cube.RectTransform.DORotate(new Vector3(0, 0, 1080), 1.0f, RotateMode.FastBeyond360));
                        sequence.Join(cube.RectTransform.DOScale(Vector3.zero, 1.0f).OnComplete(() =>
                        {
                            cube.RectTransform.localScale = Vector3.one;
                            _logger.Log(localizationManager.Localize(LocalizationConsts.OnCubeThrow));
                            sequence.Kill();
                            _sequences.Remove(sequence);
                            pool.Despawn(cube);
                        }));
                        _sequences.Add(sequence);
                    }
                });
        }

        public void Dispose()
        {
            foreach (var sequence in _sequences)
            {
                sequence.Kill();
            }
            _sequences.Clear();
            _sequences = null;

            _localizationManager = null;
            _logger = null;

            _disposed.Dispose();
            _disposed = null;
        }
    }
}