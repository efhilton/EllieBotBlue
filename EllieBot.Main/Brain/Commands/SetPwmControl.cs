using EllieBot.IO;
using EllieBot.Logging;

namespace EllieBot.Brain.Commands {

    internal class SetPwmControl : ICommandExecutor {
        public string[] Commands => new string[] { Constants.Commands.Pwm.SET_PWM };

        private readonly IPWMController PwmController;

        public SetPwmControl(IPWMController motorsController) => this.PwmController = motorsController;

        public void Execute(CommandPacket command) {
            if (command == null || command.Arguments.Length != 2) {
                return;
            }

            string deviceId = command.Arguments[0];
            if (string.IsNullOrWhiteSpace(deviceId)) {
                return;
            }

            double effort = double.Parse(command.Arguments[1] ?? "0");
            this.PwmController.SetDutyCycle(deviceId, effort);
        }
    }
}
