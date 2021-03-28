using System;
using System.Collections.Generic;

namespace EllieBot.Brain {

    public class CommandProcessor : ICommandProcessor {
        private readonly Action<string> Logger;
        private readonly Dictionary<string, ICommandExecutor> commands;

        public CommandProcessor(Action<string> logger = null) {
            this.Logger = logger;
            this.commands = new Dictionary<string, ICommandExecutor>();
        }

        public void RegisterCommand(ICommandExecutor executor) {
            string[] keys = executor.Commands;
            if (executor != null) {
                foreach (string key in keys) {
                    string cmd = key.Trim().ToLower();
                    this.commands.Add(cmd, executor);
                    this.Logger?.Invoke($"Registered command: {cmd}");
                }
            }
        }

        public void Initialize() {
        }

        public void QueueExecute(CommandPacket cmd) {
            if (cmd == null || string.IsNullOrWhiteSpace(cmd.Command)) {
                return;
            }
            try {
                if (this.commands.TryGetValue(cmd.Command.Trim().ToUpper(), out ICommandExecutor executor)) {
                    executor?.Execute(cmd);
                }
            } catch (Exception e) {
                this.Logger?.Invoke($"Error: {e.Message}");
            }
        }
    }
}
