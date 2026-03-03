using System;

namespace Assets.Features.Core.Command.Interfaces
{
    public interface ICommand : ICommand<CommandReturnValue>
    {

    }

    public interface ICommand<T> : IDisposable
    {
        public (CommandStatus, T) Do();
    }

    public class CommandReturnValue
    {
        public static CommandReturnValue Empty;
    }
}