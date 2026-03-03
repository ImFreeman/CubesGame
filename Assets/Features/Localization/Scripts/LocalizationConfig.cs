using System;
using UnityEngine;
[CreateAssetMenu(fileName = "LocalizationConfig", menuName = "ScriptableObjects/LocalizationConfig", order = 1)]

public class LocalizationConfig : ScriptableObject
{
    [SerializeField] private LanguageData[] _data;

    public LanguageData[] Data => _data;
}

[Serializable]
public class LanguageData
{
    public string Key;
    public string Text;
}