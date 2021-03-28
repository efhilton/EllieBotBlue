namespace EllieBot.Brain {

    public interface ICommandExecutor {
        string[] Commands { get; }

        void Execute(RobotCommand command);
    }
}
