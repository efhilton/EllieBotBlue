namespace EllieBot.Brain
{
    public interface ICommandProcessor
    {
        void RegisterCommand(ICommandExecutor executor);

        void QueueExecute(RobotCommand cmd);
    }
}
