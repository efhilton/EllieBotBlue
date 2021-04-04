using EllieBot.IO.Devices;
using EllieBot.Logging;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Threading.Tasks;

namespace EllieBot.IO {

    internal class LedController : ILedController {
        private readonly Dictionary<string, IBlinkable> Blinkables;
        private readonly ILogger Logger;
        private bool disposedValue;

        public static LedController Instance { get; private set; }
        public GpioController Controller { get; private set; }

        private LedController(IEnumerable<IBlinkable> blinkables, ILogger logger) {
            this.Logger = logger;
            this.Blinkables = new Dictionary<string, IBlinkable>();
            if (blinkables == null) {
                return;
            }
            foreach (IBlinkable b in blinkables) {
                if (!string.IsNullOrWhiteSpace(b.UniqueId)) {
                    string id = b.UniqueId.Trim().ToLower();
                    this.Blinkables.Add(id, b);
                    this.Logger.Info($"Registered LED: {id}");
                }
            }
        }

        public static LedController CreateInstance(GpioController controller, IEnumerable<IBlinkable> blinkables, ILogger logger) {
            if (Instance != null) {
                return Instance;
            }

            Instance = new LedController(blinkables, logger);
            Instance.Initialize(controller).Wait();
            return Instance;
        }

        private Task Initialize(GpioController controller) {
            this.Controller = controller;

            List<Task> tasks = new List<Task>();
            foreach (IBlinkable device in this.Blinkables.Values) {
                tasks.Add(device.Initialize(this.Controller));
            }
            return Task.WhenAll(tasks.ToArray());
        }

        public void TurnOn(string deviceId) {
            if (string.IsNullOrWhiteSpace(deviceId)) {
                return;
            }
            if (this.Controller == null) {
                Logger.Fine($"LED {deviceId} set to On");
            }
            if (this.Blinkables.TryGetValue(deviceId.ToLower().Trim(), out IBlinkable device)) {
                device.TurnOn();
            }
        }

        public void TurnOff(string deviceId) {
            if (string.IsNullOrWhiteSpace(deviceId)) {
                return;
            }
            if (this.Controller == null) {
                Logger.Fine($"LED {deviceId} set to Off");
            }
            if (this.Blinkables.TryGetValue(deviceId.ToLower().Trim(), out IBlinkable device)) {
                device.TurnOff();
            }
        }

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    if (this.Controller != null) {
                        foreach (IBlinkable device in this.Blinkables.Values) {
                            device.TurnOff();
                            device.Dispose();
                        }
                        this.Controller = null;
                    }
                }

                disposedValue = true;
            }
        }

        public void Dispose() {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
