namespace EllieBot.Brain
{
    public interface ICommandExecutor
    {
        void Execute(string[] commandArguments);
    }
}