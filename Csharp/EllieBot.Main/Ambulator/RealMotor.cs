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
            TargetDutyCycle = 0;

            Controller = ctl;
            if (Controller != null)
            {
                Controller.OpenPin(ForwardPin, PinMode.Output);
                Controller.OpenPin(BackwaurdPin, PinMode.Output);
                TurnOff();
            }
        }

        public void SetDirection()
        {
            if (TargetDutyCycle >= 0)
            {
                ActivePin = ForwardPin;
                InactivePin = BackwaurdPin;
            }
            else
            {
                ActivePin = BackwaurdPin;
                InactivePin = ForwardPin;
            }
        }

        public void TurnOff()
        {
            if (Controller == null)
            {
                Logger?.Invoke($"{MotorName} Motor Off");
                return;
            }
            SetDirection();
            Controller.Write(ActivePin, PinValue.Low);
            Controller.Write(InactivePin, PinValue.Low);
        }

        public void TurnOn()
        {
            if (Controller == null)
            {
                Logger?.Invoke($"{MotorName} Motor On");
                return;
            }
            SetDirection();
            Controller.Write(ActivePin, PinValue.High);
            Controller.Write(InactivePin, PinValue.Low);
        }

        public Task TurnOnDelayOff(int delayInMs)
        {
            TurnOn();

            return Task.Delay(delayInMs).ContinueWith((_) =>
           {
               TurnOff();
           });
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (this.Controller != null)
                    {
                        TurnOff();
                        Controller.ClosePin(ForwardPin);
                        Controller.ClosePin(BackwaurdPin);
                    }
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}