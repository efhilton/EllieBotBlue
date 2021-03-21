
using System.Threading.Tasks;

namespace EllieBot.Ambulator
{
    public interface IMotors
    {
         Task Go(double leftValue, double rightValue);
         Task Initialize();
    }
}
