using System.Threading.Tasks;

namespace EllieBot.Brain
{
    public interface ICommandProcessor
    {
        Task Execute(RobotCommand cmd);
    }
}