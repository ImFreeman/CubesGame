using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CubesConfig", menuName = "ScriptableObjects/CubesConfig", order = 1)]
public class CubeConfig : ScriptableObject
{
    [SerializeField] private CubeModel[] _cubeModels;

    [NonSerialized] private bool _inited;

    private Dictionary<string, CubeModel> _cubeModelsDict = new Dictionary<string, CubeModel>();

    public CubeModel? Get(string name)
    {
        if(!_inited)
        {
            Init();
        }

        if(_cubeModelsDict.TryGetValue(name, out CubeModel cubeModel))
        {
            return cubeModel;
        }

        return null;
    }

    private void Init()
    {
        for (int i = 0; i < _cubeModels.Length; i++)
        {
            _cubeModelsDict.TryAdd(_cubeModels[i].Name, _cubeModels[i]);
        }
        _inited = true;
    }
}

[Serializable]
public struct CubeModel
{
    public string Name;
    public Sprite Sprite;
    public Color Color;
}
