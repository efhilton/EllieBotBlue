using System;
using System.Device.Gpio;
using System.Threading;
using System.Threading.Tasks;

namespace EllieBot.Ambulator {

    public class RawMotorsController : IMotorsController, IDisposable {
        private static readonly int START_DELAY_IN_MS = 2000;
        private static readonly int REFRESH_PERIOD_IN_MS = 50;
        private bool disposedValue = false;
        private Timer PwmCycleTimer = null;
        private bool IsRunningPwm = false;

        public GpioController Controller { get; private set; }

        private readonly IMotor LeftMotor;
        private readonly IMotor RightMotor;
        private readonly Action<string> Logger;

        public RawMotorsController(IMotor motorLeft, IMotor motorRight, Action<string> logger = null) {
            this.LeftMotor = motorLeft;
            this.RightMotor = motorRight;
            this.Logger = logger;
        }

        public void SetDutyCycles(double leftValue, double rightValue) {
            this.LeftMotor.TargetDutyCycle = ScaleMotorInput(leftValue);
            this.RightMotor.TargetDutyCycle = ScaleMotorInput(rightValue);
        }

        public static int ScaleMotorInput(double valueNegOneToOne) {
            double rawDutyCycle = 10.0 * Math.Clamp(valueNegOneToOne, -1, 1);
            double dutyCycle = Math.Round(rawDutyCycle, 0) * 10.0;
            return Convert.ToInt32(dutyCycle);
        }

        public Task Initialize() {
            try {
                this.Controller = new GpioController();
            } catch (NotSupportedException) {
                this.Controller = null;
                this.Logger?.Invoke("Motors disabled");
            }
            this.LeftMotor.Init(this.Controller);
            this.RightMotor.Init(this.Controller);

            this.PwmCycleTimer = new Timer(new TimerCallback(this.RunPwmCycle), null, START_DELAY_IN_MS, REFRESH_PERIOD_IN_MS);
            return Task.FromResult(0);
        }

        private void RunPwmCycle(object state) {
            if (this.IsRunningPwm) {
                return;
            }

            this.IsRunningPwm = true;

            Task t1 = ScheduleMotor(this.LeftMotor);
            Task t2 = ScheduleMotor(this.RightMotor);
            Task.WhenAll(t1, t2).Wait();

            this.IsRunningPwm = false;
        }

        private static Task ScheduleMotor(IMotor motor) {
            int delayInMs = Convert.ToInt32(Math.Abs(motor.TargetDutyCycle) * REFRESH_PERIOD_IN_MS / 100.0);

            Task t;
            double absDutyCycle = Math.Abs(motor.TargetDutyCycle);
            if (absDutyCycle > 90) {
                t = Task.Run(() => motor.TurnOn());
            } else if (absDutyCycle < 10) {
                t = Task.Run(() => motor.TurnOff());
            } else {
                t = motor.TurnOnDelayOff(delayInMs);
            }

            return t;
        }

        protected virtual void Dispose(bool disposing) {
            if (!this.disposedValue) {
                if (disposing) {
                    if (this.PwmCycleTimer != null) {
                        this.PwmCycleTimer.Change(Timeout.Infinite, Timeout.Infinite);
                        this.PwmCycleTimer.DisposeAsync().AsTask().ContinueWith(x => {
                            this.LeftMotor.TurnOff();
                            this.RightMotor.TurnOff();

                            this.LeftMotor.Dispose();
                            this.RightMotor.Dispose();
                            this.Controller.Dispose();

                            this.Controller = null;
                        }).Wait();

                        this.PwmCycleTimer = null;
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
