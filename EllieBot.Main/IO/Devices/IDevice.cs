using System;
using System.Device.Gpio;
using System.Threading.Tasks;

namespace EllieBot.IO.Devices {

    public interface IDevice : IIdentifiable, IDisposable {

        Task Initialize(GpioController controller);
    }
}
