using System;
using System.Device.Gpio;

namespace EllieBot.IO.Devices {

    /// <summary>
    /// This models any device that can be turned on or off.
    /// </summary>
    public interface IBlinkable : IDisposable {
        string UniqueId { get; set; }

        void Init(GpioController controller);

        void TurnOff();

        void TurnOn();
    }
}
