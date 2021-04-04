using EllieBot.IO.Devices;
using EllieBot.Logging;
using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading.Tasks;

public class UltrasonicHCSR04 : IDevice {
    private readonly ITriggerable EchoPin;
    private readonly ILogger logger;
    private readonly IBlinkable TriggerPin;
    private bool disposedValue;
    private long RelativeTimeToFallingEdge;
    private long RelativeTimeToRisingEdge;
    private Stopwatch stopwatch;
    private double TimeOfFlightTicks = 0.0;

    public UltrasonicHCSR04(string uniqueId, IBlinkable triggerPin, ITriggerable echoPin, ILogger logger) {
        this.UniqueId = uniqueId;
        this.TriggerPin = triggerPin;
        this.EchoPin = echoPin;
        this.logger = logger;
    }

    public string UniqueId { get; set; }

    public double CalculateDistanceInCm()
        => this.TimeOfFlightTicks / Stopwatch.Frequency * 17000.0;

    public void Dispose() {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        System.GC.SuppressFinalize(this);
    }

    public Task Initialize(GpioController gpioController) {
        if (stopwatch == null) {
            stopwatch = new Stopwatch();
        }

        Task t1 = this.TriggerPin.Initialize(gpioController);
        Task t2 = this.EchoPin.Initialize(gpioController).ContinueWith(t => EchoPin.OnChangeEvent += this.GpioValueChanged);

        return Task.WhenAll(t1, t2);
    }

    public void Trigger() {
        RelativeTimeToFallingEdge = 0;
        RelativeTimeToRisingEdge = 0;
        if (stopwatch.IsRunning) {
            stopwatch.Stop();
        }

        stopwatch.Start();
        TriggerPin.TurnOn();
        Wait(stopwatch, 100); // datasheet says 10us would do but this too is fine
        TriggerPin.TurnOff();
    }

    protected virtual void Dispose(bool disposing) {
        if (!disposedValue) {
            if (disposing) {
                this.TriggerPin.TurnOff();
                this.EchoPin.OnChangeEvent -= this.GpioValueChanged;
            }

            disposedValue = true;
        }
    }

    // busy wait for milliseconds
    // source: http://www.iot-developer.net/windows-iot/uwp-programming-in-c/timer-and-timing/stopwatch
    private static void Wait(Stopwatch stopwatch, long microseconds) {
        long initialTick = stopwatch.ElapsedTicks;
        double desiredWaitTicks = Convert.ToDouble(microseconds) * Stopwatch.Frequency / 1_000_000.0;
        double finalTick = initialTick + desiredWaitTicks;
        do { } while (stopwatch.ElapsedTicks < finalTick); // busy wait
    }

    private void GpioValueChanged(PinValue value) {
        if (value == PinValue.High) {
            RelativeTimeToRisingEdge = stopwatch.ElapsedTicks;
        } else if (value == PinValue.Low) {
            RelativeTimeToFallingEdge = stopwatch.ElapsedTicks;
            stopwatch.Stop();

            TimeOfFlightTicks = RelativeTimeToFallingEdge - RelativeTimeToRisingEdge;
        }
    }
}
