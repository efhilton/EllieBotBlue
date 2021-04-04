using System;
using System.Device.Gpio;
using System.Threading.Tasks;

namespace EllieBot.IO.Devices {

    public interface IDevice : IDisposable {
        string UniqueId { get; }

        Task Initialize(GpioController controller);
    }
}
