using System;
using System.Device.Gpio;
using System.Threading.Tasks;

namespace EllieBot.IO.Devices {

    internal class DummyMotor : IMotor {
        public int _duty = 0;

        public DummyMotor(string name) => this.UniqueId = name;

        public int ActivePin { get; set; }
        public int BackwardPin { get; set; }
        public GpioController Controller { get; set; }
        public int ForwardPin { get; set; }
        public int InactivePin { get; set; }

        public int TargetDutyCycle {
            get => this._duty;
            set {
                this._duty = value;
                Console.Out.WriteLine($"{this.UniqueId} Motor Duty Cycle set to {this._duty}");
            }
        }

        public string UniqueId { get; set; }

        public void Dispose() {
            // do nothing
        }

        public Task Initialize(GpioController controller) => Task.Run(() => Console.Out.WriteLine($"{this.UniqueId} Motor initialized"));

        public void TurnOff() => Console.Out.WriteLine($"{this.UniqueId} Motor off");

        public void TurnOn() => Console.Out.WriteLine($"{this.UniqueId} Motor On");
    }
}
