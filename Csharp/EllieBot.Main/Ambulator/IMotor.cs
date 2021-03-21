using System;
using System.Device.Gpio;
using System.Threading.Tasks;

namespace EllieBot.Ambulator
{
    public interface IMotor : IDisposable
    {
        int ActivePin { get; set; }
        int BackwaurdPin { get; set; }
        GpioController Controller { get; set; }
        int ForwardPin { get; set; }
        int InactivePin { get; set; }
        string MotorName { get; set; }
        int TargetDutyCycle { get; set; }

        void Init(GpioController controller);

        void TurnOff();

        void TurnOn();

        Task TurnOnDelayOff(int delayInMs);
    }
}