using EllieBot.IO.Devices;
using System;
using System.Collections.Generic;

namespace EllieBot.Brain.Commands {

    internal class SetLedControl : ICommandExecutor {
        private readonly Dictionary<string, IBlinkable> Blinkables;
        private readonly Action<string> Logger;

        public SetLedControl(IEnumerable<IBlinkable> blinkables, Action<string> logger = null) {
            this.Logger = logger;
            this.Blinkables = new Dictionary<string, IBlinkable>();
            if (blinkables == null) {
                return;
            }
            foreach (IBlinkable b in blinkables) {
                if (!string.IsNullOrWhiteSpace(b.UniqueId)) {
                    string id = b.UniqueId.Trim().ToLower();
                    this.Blinkables.Add(id, b);
                    this.Logger?.Invoke($"Registered LED: {id}");
                }
            }
        }

        public string[] Commands => new string[] { Defaults.Commands.Led.ON, Defaults.Commands.Led.OFF };

        public void Execute(CommandPacket command) {
            if (string.IsNullOrWhiteSpace(command.Command) || command.Arguments == null || command.Arguments.Length != 1) {
                return;
            }

            string deviceId = command.Arguments[0];
            if (string.IsNullOrWhiteSpace(deviceId)) {
                return;
            }

            this.Blinkables.TryGetValue(deviceId.ToLower().Trim(), out IBlinkable device);
            if (command.Command.Trim().Equals(Defaults.Commands.Led.ON, StringComparison.OrdinalIgnoreCase)) {
                device.TurnOn();
            } else {
                device.TurnOff();
            }
        }
    }
}
