using EllieBot.IO;
using System;

namespace EllieBot.Brain.Commands {

    internal class SetLedControl : ICommandExecutor {
        private readonly ILedController Controller;

        public SetLedControl(ILedController controller) => this.Controller = controller ?? throw new ArgumentNullException(nameof(controller));

        public string[] Commands => new string[] { Constants.Commands.Led.ON, Constants.Commands.Led.OFF };

        public void Execute(CommandPacket command) {
            if (string.IsNullOrWhiteSpace(command.Command) || command.Arguments == null || command.Arguments.Length != 1) {
                return;
            }

            string deviceId = command.Arguments[0];
            if (string.IsNullOrWhiteSpace(deviceId)) {
                return;
            }

            if (command.Command.Trim().Equals(Constants.Commands.Led.ON, StringComparison.OrdinalIgnoreCase)) {
                this.Controller.TurnOn(deviceId);
            } else {
                this.Controller.TurnOff(deviceId);
            }
        }
    }
}
