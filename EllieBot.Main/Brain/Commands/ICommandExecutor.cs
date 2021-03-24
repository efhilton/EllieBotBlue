namespace EllieBot.Brain
{
    public interface ICommandExecutor
    {
        string Command { get; }

        void Execute(string[] commandArguments);
    }
}
