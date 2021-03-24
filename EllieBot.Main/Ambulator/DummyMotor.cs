using System;
using System.Device.Gpio;
using System.Threading.Tasks;

namespace EllieBot.Ambulator
{
    internal class DummyMotor : IMotor
    {
        public int _duty = 0;

        public DummyMotor(string name)
        {
            this.MotorName = name;
        }

        public int ActivePin { get; set; }
        public int BackwaurdPin { get; set; }
        public GpioController Controller { get; set; }
        public int ForwardPin { get; set; }
        public int InactivePin { get; set; }
        public string MotorName { get; set; }

        public int TargetDutyCycle
        {
            get => this._duty;
            set
            {
                this._duty = value;
                Console.Out.WriteLine($"{this.MotorName} Motor Duty Cycle set to {this._duty}");
            }
        }

        public void Dispose()
        {
            // do nothing
        }

        public void Init(GpioController controller)
        {
            Console.Out.WriteLine($"{this.MotorName} Motor initialized");
        }

        public void TurnOff()
        {
            Console.Out.WriteLine($"{this.MotorName} Motor off");
        }

        public void TurnOn()
        {
            Console.Out.WriteLine($"{this.MotorName} Motor On");
        }

        public Task TurnOnDelayOff(int delayInMs)
        {
            this.TurnOn();

            return Task.Delay(delayInMs).ContinueWith(x =>
            {
                this.TurnOff();
            });
        }
    }
}