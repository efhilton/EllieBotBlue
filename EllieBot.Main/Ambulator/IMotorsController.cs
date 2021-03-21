using EllieBot.Brain;
using System.Threading.Tasks;

namespace EllieBot.Ambulator
{
    public interface IMotorsController : ICommandExecutor
    {
        void SetDutyCycles(double leftValueNegOneToOne, double rightValueNegOneToOne);

        Task Initialize();
    }
}