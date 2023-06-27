using Svelto.Command;

namespace Svelto.IoC
{
    public class ContextCommandFactory : ICommandFactory
    {
        public ContextCommandFactory(IContainer container)
        {
            _commandFactory = new Command.CommandFactory(cmd => container.Inject(cmd));
        }

        public TCommand Build<TCommand>() where TCommand : ICommand, new()
        {
            return _commandFactory.Build<TCommand>();
        }

        Command.CommandFactory _commandFactory;
    }
}
