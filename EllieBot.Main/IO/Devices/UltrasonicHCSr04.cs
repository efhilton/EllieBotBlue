using EllieBot.IO.Devices;
using EllieBot.Logging;
using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

public class UltrasonicHCSR04 : ISensor<double> {
    private const int SWEEP_TIMEOUT_IN_MS = 100;
    private const int TRIGGER_HOLD_IN_US = 10; // trigger pin must be held by at least 10us to trigger a scan.
    private const int TRIGGER_FIRST_START_IN_MS = 5000;
    private const int TRIGGER_PERIOD_IN_MS = 1000;
    private readonly ISensor<PinValue> EchoPin;
    private readonly ILogger logger;
    private readonly IBlinkable TriggerPin;
    private bool disposedValue;
    private long RelativeTimeToRisingEdge;
    private Stopwatch stopwatch;
    private long TimeOfFlightTicks = 0;
    private Timer RefreshTrigger;

    public event ISensor<double>.OnDataReceived OnData;

    public double DistanceToObject { get; private set; }

    public UltrasonicHCSR04(string uniqueId, IBlinkable triggerPin, ISensor<PinValue> echoPin, ILogger logger) {
        this.UniqueId = uniqueId;
        this.TriggerPin = triggerPin;
        this.EchoPin = echoPin;
        this.logger = logger;
    }

    public string UniqueId { get; set; }

    /// <summary>
    /// Calculate the distance to the target based on the roundtrip time (dividing by 2 because it is a roundtrip).
    /// cm =  seconds * (340.0 m/s * 100.0 cm/m) / 2.0
    /// </summary>
    /// <returns>Distance to object, in centimeters.</returns>
    private static double CalculateDistanceInCm(long timeOfFlightInTicks, double speedOfSoundInMetersPerSec = 340.0)
        => timeOfFlightInTicks / Stopwatch.Frequency * speedOfSoundInMetersPerSec / 200.0;

    public void Dispose() {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public Task Initialize(GpioController gpioController) {
        if (stopwatch == null) {
            stopwatch = new Stopwatch();
        }

        Task t1 = this.TriggerPin.Initialize(gpioController);
        Task t2 = this.EchoPin.Initialize(gpioController).ContinueWith(t => EchoPin.OnData += this.GpioValueChanged);
        Task t3 = Task.Run(() => this.RefreshTrigger = new Timer(this.OnStartScan, null, TRIGGER_FIRST_START_IN_MS, TRIGGER_PERIOD_IN_MS));
        return Task.WhenAll(t1, t2, t3);
    }

    private void OnStartScan(object ignored) {
        if (stopwatch != null && stopwatch.IsRunning) {
            // at most, it would have taken 38ms, so if we
            // are still running, then something is wrong.
            stopwatch.Stop();
        }
        this.Trigger().Wait(SWEEP_TIMEOUT_IN_MS);
    }

    public Task Trigger() {
        if (stopwatch != null && stopwatch.IsRunning) {
            return Task.FromResult(0);
        }

        RelativeTimeToRisingEdge = 0;

        stopwatch.Reset();
        stopwatch.Start();

        TriggerPin.TurnOn();
        return BusyWaitMicroSeconds(stopwatch, TRIGGER_HOLD_IN_US)
            .ContinueWith((t) => TriggerPin.TurnOff());
    }

    protected virtual void Dispose(bool disposing) {
        if (!disposedValue) {
            if (disposing) {
                this.RefreshTrigger.Dispose();
                this.RefreshTrigger = null;

                this.TriggerPin.TurnOff();
                this.EchoPin.OnData -= this.GpioValueChanged;
            }

            disposedValue = true;
        }
    }

    // busy wait for microseconds
    // source: http://www.iot-developer.net/windows-iot/uwp-programming-in-c/timer-and-timing/stopwatch
    private static Task BusyWaitMicroSeconds(Stopwatch stopwatch, long microseconds) {
        return Task.Run(() => {
            long initialTick = stopwatch.ElapsedTicks;
            double desiredWaitTicks = Convert.ToDouble(microseconds * Stopwatch.Frequency) / 1_000_000.0;
            double finalTick = initialTick + desiredWaitTicks;
            do { } while (stopwatch.ElapsedTicks < finalTick); // busy wait
        });
    }

    private void GpioValueChanged(string id, PinValue value) {
        if (!this.stopwatch.IsRunning) {
            return;
        }
        if (value == PinValue.High) {
            RelativeTimeToRisingEdge = stopwatch.ElapsedTicks;
        } else {
            this.TimeOfFlightTicks = stopwatch.ElapsedTicks - RelativeTimeToRisingEdge;
            stopwatch.Stop();

            this.DistanceToObject = CalculateDistanceInCm(this.TimeOfFlightTicks);
            this.OnData?.Invoke(this.UniqueId, this.DistanceToObject);
            this.logger.Debug($"In {this.UniqueId}: {TimeOfFlightTicks} at {stopwatch.ElapsedTicks} - {stopwatch.Elapsed} - {stopwatch.ElapsedMilliseconds}");
        }
    }
}
