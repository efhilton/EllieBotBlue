using System;
using System.Threading.Tasks;

namespace EllieBot.Ambulator
{
    public class DummyMotors : IMotors
    {
        Task IMotors.Go(double leftValue, double rightValue)
        {
            return Task.Run(() => Console.WriteLine($"Motors {leftValue}, {rightValue}"));
        }

        Task IMotors.Initialize()
        {
            return Task.Run(() => Console.WriteLine("Dummy Motor Initialized"));
        }
    }
}
