namespace EllieBot.IO.Devices {

    /// <summary>
    /// This models any device that can be turned on or off.
    /// </summary>
    public interface IBlinkable : IDevice {

        void TurnOff();

        void TurnOn();
    }
}
