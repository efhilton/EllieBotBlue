using EllieBot.NervousSystem;

namespace EllieBot.Brain {

    public interface ICommandProcessor {

        void RegisterCommand(ICommandExecutor executor);

        void QueueExecute(CommandPdu cmd);

        void Initialize();
    }
}
