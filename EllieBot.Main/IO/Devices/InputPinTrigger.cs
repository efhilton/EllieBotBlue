using EllieBot.Logging;
using System;
using System.Device.Gpio;
using System.Threading.Tasks;

namespace EllieBot.IO.Devices {

    internal class InputPinTrigger : ISensor<PinValue> {
        private readonly ILogger Logger;
        private readonly int PinNumber;
        private bool disposedValue;
        private GpioController GpioController;

        public InputPinTrigger(string uniqueId, int pinNumber, ILogger logger) {
            if (string.IsNullOrWhiteSpace(uniqueId)) {
                throw new ArgumentException("Unique ID cannot be empty");
            }
            this.UniqueId = uniqueId;
            this.PinNumber = pinNumber;
            this.Logger = logger;
        }

        public string UniqueId { get; set; }

        public event ISensor<PinValue>.OnDataReceived OnData;

        public void Dispose() {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public Task Initialize(GpioController controller) {
            return Task.Run(() => {
                if (controller == null) {
                    this.Logger.Info($"No IO. Disabled {this.UniqueId}");
                    return;
                }
                this.GpioController = controller;
                this.GpioController.OpenPin(this.PinNumber, PinMode.Input);
                this.GpioController.RegisterCallbackForPinValueChangedEvent(this.PinNumber, PinEventTypes.Rising, this.OnPinHigh);
                this.GpioController.RegisterCallbackForPinValueChangedEvent(this.PinNumber, PinEventTypes.Falling, this.OnPinLow);
                this.Logger.Info($"Listening for changes in {this.UniqueId}");
            });
        }

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    this.GpioController.UnregisterCallbackForPinValueChangedEvent(this.PinNumber, this.OnPinHigh);
                    this.GpioController.UnregisterCallbackForPinValueChangedEvent(this.PinNumber, this.OnPinLow);
                    this.GpioController.ClosePin(this.PinNumber);
                }

                disposedValue = true;
            }
        }

        private void OnPinHigh(object sender, PinValueChangedEventArgs pinValueChangedEventArgs) {
            if (pinValueChangedEventArgs.PinNumber == this.PinNumber) {
                this.OnData?.Invoke(this.UniqueId, PinValue.High);
            }
        }

        private void OnPinLow(object sender, PinValueChangedEventArgs pinValueChangedEventArgs) {
            if (pinValueChangedEventArgs.PinNumber == this.PinNumber) {
                this.OnData?.Invoke(this.UniqueId, PinValue.Low);
            }
        }
    }
}
