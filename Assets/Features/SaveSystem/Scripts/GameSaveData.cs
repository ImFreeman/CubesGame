using System;
using System.Collections.Generic;

namespace Assets.Features.SaveSystem.Scripts
{
    [Serializable]
    public class GameSaveData
    {
        public List<TowerItemData> Data; 
    }

    [Serializable]
    public class TowerItemData
    {
        public string Color;
        public float Offset;
    }
}
