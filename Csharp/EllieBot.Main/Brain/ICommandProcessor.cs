using System.Threading.Tasks;

namespace EllieBot.Brain
{
    public interface ICommandProcessor
    {
        void QueueExecute(RobotCommand cmd);
    }
}