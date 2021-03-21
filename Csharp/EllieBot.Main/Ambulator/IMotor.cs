using System.Device.Gpio;
using System.Threading.Tasks;

namespace EllieBot.Ambulator
{
    public interface IMotor
    {
        int ActivePin { get; set; }
        int BackwaurdPin { get; set; }
        GpioController Controller { get; set; }
        int ForwardPin { get; set; }
        int InactivePin { get; set; }
        int TargetDutyCycle { get; set; }
        string MotorName { get; set; }

        void TurnOff();

        void TurnOn();

        void Init(GpioController controller);

        Task TurnOnDelayOff(int delayInMs);
    }
}