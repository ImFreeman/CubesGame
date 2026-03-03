using Assets.Features.Core.Command;
using Assets.Features.Core.Command.Interfaces;
using Assets.Features.Cube.Scripts;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;


namespace Assets.Features.SaveSystem.Scripts
{
    public class LoadGameCommand : ICommand
    {
        private IInstantiator _instantiator;
        private IDictionary<int, float> _offsetData;
        private IReactiveCollection<UIElement> _towerCollection;
        private IReactiveCollection<UIElement> _dndCollection;

        public LoadGameCommand(
            IInstantiator instantiator,
            IDictionary<int, float> offsetData,
            [Inject(Id = UIElementsContainerType.Tower)]
            IReactiveCollection<UIElement> towerCollection,
            [Inject(Id = UIElementsContainerType.DragAndDrop)]
            IReactiveCollection<UIElement> dndCollection)
        {
            _instantiator = instantiator;
            _offsetData = offsetData;
            _towerCollection = towerCollection;
            _dndCollection = dndCollection;
        }
        public void Dispose()
        {
            _instantiator = null;
            _offsetData = null;
            _towerCollection = null;
        }

        public (CommandStatus, CommandReturnValue) Do()
        {
            if(!PlayerPrefs.HasKey(GameSaveConst.GameDataKey))
            {
                return (CommandStatus.Failed, CommandReturnValue.Empty);
            }

            var json = PlayerPrefs.GetString(GameSaveConst.GameDataKey);
            var gameData = JsonUtility.FromJson<GameSaveData>(json);

            foreach(var item in gameData.Data)
            {
                var createItemCommand = _instantiator.Instantiate<SpawnCubeCommand>(new object[] { item.Color });
                var commandResult = createItemCommand.Do();
                if(commandResult.Item1 == CommandStatus.Failed)
                {
                    return (CommandStatus.Failed, CommandReturnValue.Empty);
                }

                _offsetData.TryAdd(commandResult.Item2.GetInstanceID(), item.Offset);
                _dndCollection.Add(commandResult.Item2);
                _towerCollection.Add(commandResult.Item2);
            }

            return (CommandStatus.Success, CommandReturnValue.Empty);
        }
    }
}
