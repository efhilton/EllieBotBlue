using System;

namespace EllieBot.IO {

    public interface IPWMController : IDisposable {

        void SetDutyCycle(string deviceUniqueId, double valueNegOneToOne);
    }
}
