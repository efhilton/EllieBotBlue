using System;
using System.Device.Gpio;
using System.Threading;
using System.Threading.Tasks;

namespace EllieBot.Ambulator
{
    public class MotorsController : IMotorsController, IDisposable
    {
        private static int REFRESH_PERIOD_IN_MS = 50;
        private bool disposedValue = false;
        private Timer PwmCycleTimer = null;
        private bool IsRunningPwm = false;

        public GpioController Controller { get; private set; }

        private readonly IMotor LeftMotor;
        private readonly IMotor RightMotor;
        private readonly Action<string> Logger;

        public MotorsController(IMotor motorLeft, IMotor motorRight, Action<string> logger = null)
        {
            LeftMotor = motorLeft;
            RightMotor = motorRight;
            Logger = logger;
        }

        public void SetDutyCycles(double leftValue, double rightValue)
        {
            LeftMotor.TargetDutyCycle = ScaleMotorInput(leftValue);
            RightMotor.TargetDutyCycle = ScaleMotorInput(rightValue);
        }

        public static int ScaleMotorInput(double valueNegOneToOne)
        {
            double rawDutyCycle = 10.0 * Math.Clamp(valueNegOneToOne, -1, 1);
            double dutyCycle = Math.Round(rawDutyCycle, 0) * 10.0;
            return Convert.ToInt32(dutyCycle);
        }

        public Task Initialize()
        {
            try
            {
                this.Controller = new GpioController();
            }
            catch (NotSupportedException)
            {
                this.Controller = null;
                Logger?.Invoke("Motors disabled");
            }
            LeftMotor.Init(Controller);
            RightMotor.Init(Controller);

            this.PwmCycleTimer = new Timer(new TimerCallback(this.RunPwmCycle), null, 2000, REFRESH_PERIOD_IN_MS);
            return Task.FromResult(0);
        }

        private void RunPwmCycle(object state)
        {
            if (IsRunningPwm)
            {
                return;
            }

            IsRunningPwm = true;

            Task t1 = ScheduleMotor(LeftMotor);
            Task t2 = ScheduleMotor(RightMotor);
            Task.WhenAll(t1, t2).Wait();

            IsRunningPwm = false;
        }

        private static Task ScheduleMotor(IMotor motor)
        {
            int delayInMs = Convert.ToInt32(Math.Abs(motor.TargetDutyCycle) * REFRESH_PERIOD_IN_MS / 100.0);

            Task t;
            double absDutyCycle = Math.Abs(motor.TargetDutyCycle);
            if (absDutyCycle > 90)
            {
                t = Task.Run(() => motor.TurnOn());
            }
            else if (absDutyCycle < 10)
            {
                t = Task.Run(() => motor.TurnOff());
            }
            else
            {
                t = motor.TurnOnDelayOff(delayInMs);
            }

            return t;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (PwmCycleTimer != null)
                    {
                        PwmCycleTimer.Change(Timeout.Infinite, Timeout.Infinite);
                        PwmCycleTimer.DisposeAsync().AsTask().ContinueWith(x =>
                        {
                            LeftMotor.TurnOff();
                            RightMotor.TurnOff();

                            LeftMotor.Dispose();
                            RightMotor.Dispose();
                            Controller.Dispose();

                            Controller = null;
                        }).Wait();

                        PwmCycleTimer = null;
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

        public void Execute(string[] commandArguments)
        {
            if (commandArguments.Length != 2)
            {
                return;
            }
            double leftDuty = double.Parse(commandArguments[0]);
            double rightDuty = double.Parse(commandArguments[1]);
            SetDutyCycles(leftDuty, rightDuty);
        }
    }
}