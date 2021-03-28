using EllieBot.IO;
using System;

namespace EllieBot.Brain.Commands {

    internal class SetPwmControl : ICommandExecutor {
        public string[] Commands => new string[] { Defaults.Commands.Pwm.SET_PWM };

        private readonly IPWMController PwmController;
        private readonly Action<string> Logger;

        public SetPwmControl(IPWMController motorsController, Action<string> logger = null) {
            this.PwmController = motorsController;
            this.Logger = logger;
        }

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
