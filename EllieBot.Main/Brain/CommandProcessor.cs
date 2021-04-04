using EllieBot.Logging;
using System;
using System.Collections.Generic;

namespace EllieBot.Brain {

    public class CommandProcessor : ICommandProcessor {
        private readonly ILogger Logger;
        private readonly Dictionary<string, ICommandExecutor> commands;

        public CommandProcessor(ILogger logger) {
            this.Logger = logger;
            this.commands = new Dictionary<string, ICommandExecutor>();
        }

        public void RegisterCommand(ICommandExecutor executor) {
            string[] keys = executor.Commands;
            if (executor != null) {
                foreach (string key in keys) {
                    string cmd = key.Trim().ToLower();
                    this.commands.Add(cmd, executor);
                    this.Logger.Info($"Registered command: {cmd}");
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
                this.Logger.Error(e.Message);
            }
        }
    }
}
