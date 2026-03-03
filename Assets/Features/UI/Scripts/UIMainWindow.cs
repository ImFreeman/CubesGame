using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Features.UI.Scripts
{
    public class UIMainWindow : MonoBehaviour
    {
        [SerializeField] private Image _hole;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private ScrollRect _scroll;
        [SerializeField] private Image _tower;
        [SerializeField] private RectTransform _towerCubeContainer;
        [SerializeField] private TMP_Text _logTextComponent;
        public TMP_Text LogTextComponent => _logTextComponent;
        public Image TowerDropPlace => _tower;
        public Image HoleDropPlace => _hole;
        public RectTransform RectTransform => _rectTransform;
        public ScrollRect Scroll => _scroll;
        public RectTransform TowerCubeContainer => _towerCubeContainer;

    }
}