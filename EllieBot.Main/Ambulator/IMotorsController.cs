using System.Threading.Tasks;

namespace EllieBot.Ambulator
{
    public interface IMotorsController
    {
        void SetDutyCycles(double leftValueNegOneToOne, double rightValueNegOneToOne);

        Task Initialize();
    }
}
