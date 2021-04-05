using EllieBot.NervousSystem;

namespace EllieBot.Brain {

    public interface ICommandExecutor {
        string[] Commands { get; }

        void Execute(CommandPdu command);
    }
}
