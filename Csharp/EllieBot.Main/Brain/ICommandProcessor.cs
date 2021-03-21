using System.Threading.Tasks;

namespace EllieBot.Brain
{
    public interface ICommandProcessor
    {
        void RegisterCommand(string key, ICommandExecutor executor);

        void QueueExecute(RobotCommand cmd);
    }
}