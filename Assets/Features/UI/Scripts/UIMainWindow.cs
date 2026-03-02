using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIMainWindow : MonoBehaviour
{
    [SerializeField] private Image _hole;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private ScrollRect _scroll;
    [SerializeField] private Image _tower;
    [SerializeField] private RectTransform _towerCubeContainer;
    public Image Tower => _tower;
    public Image Hole => _hole;
    public RectTransform RectTransform => _rectTransform;
    public ScrollRect Scroll => _scroll;
    public RectTransform TowerCubeContainer => _towerCubeContainer;

}
