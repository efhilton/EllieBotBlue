using System.Device.Gpio;

namespace EllieBot.IO.Devices {

    public delegate void OnChangeReceived(PinValue value);

    public interface ITriggerable : IDevice {

        public event OnChangeReceived OnChangeEvent;
    }
}
