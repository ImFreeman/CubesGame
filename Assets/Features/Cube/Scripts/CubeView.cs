using UnityEngine;
using UnityEngine.UI;

namespace Assets.Features.Cube.Scripts
{
    public class CubeView : UIElement
    {
        [SerializeField] private Image _image;
        private void Init(CubeViewProtocol protocol)
        {
            _image.sprite = protocol.Sprite;

            _rectTransform.localScale = Vector3.one;
        }

        public class Pool : UIElementsPool<CubeViewProtocol, CubeView>
        {
            protected override void Reinitialize(CubeViewProtocol p1, CubeView item)
            {
                item.Init(p1);
            }
        }
    }

    public readonly struct CubeViewProtocol
    {
        public readonly Sprite Sprite;
        public CubeViewProtocol(Sprite sprite) : this()
        {
            Sprite = sprite;
        }
    }
}