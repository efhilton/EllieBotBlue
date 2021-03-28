using EllieBot.IO;

namespace EllieBot.Brain.Commands {

    internal class SetPwmControl : ICommandExecutor {
        private const string SET_PWM = "set.pwm";
        public string[] Commands => new string[] { SET_PWM };

        private readonly IPWMController PwmController;

        public SetPwmControl(IPWMController motorsController) => this.PwmController = motorsController;

        public void Execute(RobotCommand command) {
            if (command.Arguments.Length != 2) {
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
