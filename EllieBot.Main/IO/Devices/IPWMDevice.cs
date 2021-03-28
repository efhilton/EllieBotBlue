namespace EllieBot.IO.Devices {

    /// <summary>
    /// Model any device that can be controlled via PWM.
    /// </summary>
    public interface IPWMDevice : IBlinkable {
        int TargetDutyCycle { get; set; }
    }
}
