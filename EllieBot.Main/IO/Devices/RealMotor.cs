using System;
using System.Device.Gpio;

namespace EllieBot.IO.Devices {

    public class RealMotor : IMotor {
        public static int PWM_PERIOD_IN_MS = 100;
        public readonly Action<string> Logger;
        private bool disposedValue;

        public RealMotor(string uniqueId, int pinForward, int pinBackward, Action<string> logger = null) {
            this.UniqueId = uniqueId;
            this.ForwardPin = pinForward;
            this.BackwardPin = pinBackward;
            this.Logger = logger;
        }

        public int ActivePin { get; set; }
        public int BackwardPin { get; set; }
        public GpioController Controller { get; set; }
        public int ForwardPin { get; set; }
        public int InactivePin { get; set; }
        public string UniqueId { get; set; }
        public int TargetDutyCycle { get; set; }

        public void Init(GpioController ctl) {
            this.TargetDutyCycle = 0;

            this.Controller = ctl;
            if (this.Controller != null) {
                this.Controller.OpenPin(this.ForwardPin, PinMode.Output);
                this.Controller.OpenPin(this.BackwardPin, PinMode.Output);
                this.TurnOff();
            }
        }

        public void SetDirection() {
            if (this.TargetDutyCycle >= 0) {
                this.ActivePin = this.ForwardPin;
                this.InactivePin = this.BackwardPin;
            } else {
                this.ActivePin = this.BackwardPin;
                this.InactivePin = this.ForwardPin;
            }
        }

        public void TurnOff() {
            if (this.Controller == null) {
                this.Logger?.Invoke($"Motor {this.UniqueId} Off");
                return;
            }
            this.SetDirection();
            this.Controller.Write(this.ActivePin, PinValue.Low);
            this.Controller.Write(this.InactivePin, PinValue.Low);
        }

        public void TurnOn() {
            if (this.Controller == null) {
                this.Logger?.Invoke($"Motor {this.UniqueId} On");
                return;
            }
            this.SetDirection();
            this.Controller.Write(this.ActivePin, PinValue.High);
            this.Controller.Write(this.InactivePin, PinValue.Low);
        }

        protected virtual void Dispose(bool disposing) {
            if (!this.disposedValue) {
                if (disposing) {
                    if (this.Controller != null) {
                        this.TurnOff();
                        this.Controller.ClosePin(this.ForwardPin);
                        this.Controller.ClosePin(this.BackwardPin);
                    }
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
