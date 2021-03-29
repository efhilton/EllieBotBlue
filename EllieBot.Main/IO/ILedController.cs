using System;

namespace EllieBot.IO {

    internal interface ILedController : IDisposable {

        void TurnOff(string deviceId);

        void TurnOn(string deviceId);
    }
}
