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
            get => _duty;
            set
            {
                this._duty = value;
                Console.Out.WriteLine($"{MotorName} Motor Duty Cycle set to {_duty}");
            }
        }

        public void Dispose()
        {
            // do nothing
        }

        public void Init(GpioController controller)
        {
            Console.Out.WriteLine($"{MotorName} Motor initialized");
        }

        public void TurnOff()
        {
            Console.Out.WriteLine($"{MotorName} Motor off");
        }

        public void TurnOn()
        {
            Console.Out.WriteLine($"{MotorName} Motor On");
        }

        public Task TurnOnDelayOff(int delayInMs)
        {
            TurnOn();

            return Task.Delay(delayInMs).ContinueWith(x =>
            {
                TurnOff();
            });
        }
    }
}