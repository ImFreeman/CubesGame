using Assets.Features.Core.Command;
using Assets.Features.Core.Command.Interfaces;
using Assets.Features.Cube.Scripts;
using Assets.Features.UI.Scripts;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

namespace Assets.Features.SaveSystem.Scripts
{
    public class SaveGameCommand : ICommand
    {
        private IDictionary<int, float> _offsetData;
        private IDictionary<int, string> _colorData;
        private IReactiveCollection<UIElement> _towerCollection;

        public SaveGameCommand(
            [Inject(Id = UIElementsContainerType.Tower)]
            IReactiveCollection<UIElement> towerCollection,
            IDictionary<int, float> offsetData,
            IDictionary<int, string> colorData,
            UIMainWindow window
            )
        {
            _towerCollection = towerCollection;
            _offsetData = offsetData;
            _colorData = colorData;
        }

        public void Dispose()
        {
            _offsetData = null;
            _colorData = null;
            _towerCollection = null;
        }

        public (CommandStatus, CommandReturnValue) Do()
        {
            GameSaveData gameSaveData = new GameSaveData() { Data = new List<TowerItemData>() };
            foreach (var item in _towerCollection )
            {
                string color;
                if(!_colorData.TryGetValue(item.GetInstanceID(), out color))
                {
                    return (CommandStatus.Failed, CommandReturnValue.Empty);
                }
                
                float offset;
                if(!_offsetData.TryGetValue(item.GetInstanceID(), out offset))
                {
                    return (CommandStatus.Failed, CommandReturnValue.Empty);
                }

                gameSaveData.Data.Add(new TowerItemData() { Color = color, Offset = offset });
            }

            string json = JsonUtility.ToJson(gameSaveData);

            PlayerPrefs.SetString(GameSaveConst.GameDataKey, json);
            PlayerPrefs.Save();

            return (CommandStatus.Success, CommandReturnValue.Empty);
        }
    }
}
