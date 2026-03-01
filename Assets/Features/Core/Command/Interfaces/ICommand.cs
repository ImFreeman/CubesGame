using Assets.Features.Core.Command;
using System;
using UnityEngine.EventSystems;

public interface ICommand<T> : IDisposable
{
    public (CommandStatus, T) Do();
}
