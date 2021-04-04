using EllieBot.IO.Devices;
using EllieBot.Logging;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Threading;
using System.Threading.Tasks;

namespace EllieBot.IO {

    public class PwmController : IPWMController, IDisposable {
        private static readonly int REFRESH_PERIOD_IN_MS = 50; // 20 seconds
        private static readonly int START_DELAY_IN_MS = 5000;
        private GpioController Controller;

        private readonly Dictionary<string, IPWMDevice> Devices;
        private readonly Task[] Tasks;
        private readonly ILogger Logger;
        private bool disposedValue = false;
        private bool IsRunningPwm = false;
        private Timer PwmCycleTimer = null;
        private static PwmController Instance;

        private PwmController(IEnumerable<IPWMDevice> devices, ILogger logger) {
            this.Logger = logger;
            this.Devices = new Dictionary<string, IPWMDevice>();

            foreach (IPWMDevice device in devices) {
                string deviceId = device.UniqueId.Trim().ToLower();
                this.Devices.Add(deviceId, device);
                this.Logger.Info($"Registered PWM device: {deviceId}");
            }

            this.Tasks = new Task[this.Devices.Keys.Count];
            this.Logger.Debug($"Registered {this.Devices.Keys.Count} PWM Devices");
        }

        public static int ScaleMotorInput(double valueNegOneToOne) {
            double rawDutyCycle = 10.0 * Math.Clamp(valueNegOneToOne, -1, 1);
            double dutyCycle = Math.Round(rawDutyCycle, 0) * 10.0;
            return Convert.ToInt32(dutyCycle);
        }

        internal static IPWMController CreateInstance(GpioController controller, IEnumerable<IPWMDevice> iPWMDevices, ILogger logger) {
            if (Instance != null) {
                return Instance;
            }
            Instance = new PwmController(iPWMDevices, logger);
            Instance.Initialize(controller).Wait();
            return Instance;
        }

        public void Dispose() {
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private Task Initialize(GpioController controller) {
            this.Controller = controller;

            List<Task> tasks = new List<Task>();
            foreach (IPWMDevice device in this.Devices.Values) {
                tasks.Add(device.Initialize(this.Controller));
            }
            return Task.WhenAll(tasks.ToArray())
            .ContinueWith((_) => this.PwmCycleTimer = new Timer(this.RunOnePwmCycle, null, START_DELAY_IN_MS, REFRESH_PERIOD_IN_MS));
        }

        public void SetDutyCycle(string deviceName, double value) {
            if (string.IsNullOrEmpty(deviceName)) {
                return;
            }
            int cleansedDutyCycle = ScaleMotorInput(value);
            this.Devices.TryGetValue(deviceName.Trim().ToLower(), out IPWMDevice device);
            if (device != null) {
                device.TargetDutyCycle = cleansedDutyCycle;
            }
        }

        protected virtual void Dispose(bool disposing) {
            if (!this.disposedValue) {
                if (disposing) {
                    if (this.PwmCycleTimer != null) {
                        this.PwmCycleTimer.Change(Timeout.Infinite, Timeout.Infinite);
                        this.PwmCycleTimer.DisposeAsync().AsTask().ContinueWith(x => {
                            foreach (IPWMDevice device in this.Devices.Values) {
                                device.TurnOff();
                                device.Dispose();
                            }
                            this.Devices.Clear();

                            this.Controller.Dispose();

                            this.Controller = null;
                        }).Wait();

                        this.PwmCycleTimer = null;
                    }
                }

                this.disposedValue = true;
            }
        }

        private static Task ScheduleDevice(IPWMDevice motor) {
            int delayInMs = Convert.ToInt32(Math.Abs(motor.TargetDutyCycle) * REFRESH_PERIOD_IN_MS / 100.0);

            Task t;
            double absDutyCycle = Math.Abs(motor.TargetDutyCycle);
            t = absDutyCycle > 90
                ? Task.Run(() => motor.TurnOn())
                : absDutyCycle < 10 ? Task.Run(() => motor.TurnOff()) : TurnOnThenOffAfterDelay(delayInMs, motor);
            return t;
        }

        private static Task TurnOnThenOffAfterDelay(int delayInMs, IPWMDevice device) {
            device.TurnOn();

            return Task.Delay(delayInMs).ContinueWith((_) => device.TurnOff());
        }

        private void RunOnePwmCycle(object ignored) {
            if (this.IsRunningPwm) {
                return;
            }

            this.IsRunningPwm = true;

            int id = 0;
            foreach (IPWMDevice device in this.Devices.Values) {
                this.Tasks[id] = ScheduleDevice(device);
                id++;
            }

            Task.WhenAll(this.Tasks).Wait();

            this.IsRunningPwm = false;
        }
    }
}
