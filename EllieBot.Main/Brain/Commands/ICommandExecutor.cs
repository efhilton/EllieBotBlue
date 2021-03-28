namespace EllieBot.Brain {

    public interface ICommandExecutor {
        string[] Commands { get; }

        void Execute(CommandPacket command);
    }
}
