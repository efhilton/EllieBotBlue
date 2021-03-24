using System;
using System.Device.Gpio;
using System.Threading.Tasks;

namespace EllieBot.Ambulator
{
    public class RealMotor : IMotor
    {
        public static int PWM_PERIOD_IN_MS = 100;
        public readonly Action<string> Logger;
        private bool disposedValue;

        public RealMotor(string name, int pinForward, int pinBackward, Action<string> logger = null)
        {
            this.MotorName = name;
            this.ForwardPin = pinForward;
            this.BackwaurdPin = pinBackward;
            this.Logger = logger;
        }

        public int ActivePin { get; set; }
        public int BackwaurdPin { get; set; }
        public GpioController Controller { get; set; }
        public int ForwardPin { get; set; }
        public int InactivePin { get; set; }
        public string MotorName { get; set; }
        public int TargetDutyCycle { get; set; }

        public void Init(GpioController ctl)
        {
            this.TargetDutyCycle = 0;

            this.Controller = ctl;
            if (this.Controller != null)
            {
                this.Controller.OpenPin(this.ForwardPin, PinMode.Output);
                this.Controller.OpenPin(this.BackwaurdPin, PinMode.Output);
                this.TurnOff();
            }
        }

        public void SetDirection()
        {
            if (this.TargetDutyCycle >= 0)
            {
                this.ActivePin = this.ForwardPin;
                this.InactivePin = this.BackwaurdPin;
            }
            else
            {
                this.ActivePin = this.BackwaurdPin;
                this.InactivePin = this.ForwardPin;
            }
        }

        public void TurnOff()
        {
            if (this.Controller == null)
            {
                this.Logger?.Invoke($"{this.MotorName} Motor Off");
                return;
            }
            this.SetDirection();
            this.Controller.Write(this.ActivePin, PinValue.Low);
            this.Controller.Write(this.InactivePin, PinValue.Low);
        }

        public void TurnOn()
        {
            if (this.Controller == null)
            {
                this.Logger?.Invoke($"{this.MotorName} Motor On");
                return;
            }
            this.SetDirection();
            this.Controller.Write(this.ActivePin, PinValue.High);
            this.Controller.Write(this.InactivePin, PinValue.Low);
        }

        public Task TurnOnDelayOff(int delayInMs)
        {
            this.TurnOn();

            return Task.Delay(delayInMs).ContinueWith((_) =>
           {
               this.TurnOff();
           });
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    if (this.Controller != null)
                    {
                        this.TurnOff();
                        this.Controller.ClosePin(this.ForwardPin);
                        this.Controller.ClosePin(this.BackwaurdPin);
                    }
                }

                this.disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            this.Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}