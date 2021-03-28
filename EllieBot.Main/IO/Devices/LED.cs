using System;
using System.Device.Gpio;

namespace EllieBot.IO.Devices {

    internal class LED : IBlinkable {
        private readonly Action<string> Logger;
        private readonly int Pin;
        private GpioController Controller;
        private bool disposedValue;

        public string UniqueId { get; internal set; }

        public LED(string uniqueId, int headlightsPin, Action<string> logger = null) {
            if (string.IsNullOrWhiteSpace(uniqueId)) {
                throw new ArgumentException("Unique ID cannot be null");
            }
            this.UniqueId = uniqueId.Trim();
            this.Pin = headlightsPin;
            this.Logger = logger;
        }

        public void Init(GpioController controller) {
            this.Controller = controller;
            if (this.Controller != null) {
                this.Controller.OpenPin(this.Pin, PinMode.Output);
                this.TurnOff();
            }
        }

        public void TurnOff() {
            if (this.Controller == null) {
                this.Logger?.Invoke($"LED {this.UniqueId} Off");
                return;
            }
            this.Controller.Write(this.Pin, PinValue.Low);
        }

        public void TurnOn() {
            if (this.Controller == null) {
                this.Logger?.Invoke($"LED {this.UniqueId} On");
                return;
            }
            this.Controller.Write(this.Pin, PinValue.High);
        }

        protected virtual void Dispose(bool disposing) {
            if (!this.disposedValue) {
                if (disposing) {
                    if (this.Controller != null) {
                        this.Controller.ClosePin(this.Pin);
                    }
                    this.Controller = null;
                }

                this.disposedValue = true;
            }
        }

        public void Dispose() {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
