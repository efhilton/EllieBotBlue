using System;
using System.Threading.Tasks;

namespace EllieBot.Ambulator
{
    public class Motors : IMotors
    {
        Task IMotors.Go(double leftValue, double rightValue)
        {
            throw new NotImplementedException();
        }

        Task IMotors.Initialize()
        {
            throw new NotImplementedException();
        }
    }
}
