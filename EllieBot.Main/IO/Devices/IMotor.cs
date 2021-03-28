using System.Device.Gpio;

namespace EllieBot.IO.Devices {

    /// <summary>
    /// This motors any motor device that has direction as well as the
    /// ability to be controlled via PWM.
    /// </summary>
    public interface IMotor : IPWMDevice {
        int ActivePin { get; set; }
        int BackwardPin { get; set; }
        GpioController Controller { get; set; }
        int ForwardPin { get; set; }
        int InactivePin { get; set; }
    }
}
