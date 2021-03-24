using System.Collections.Generic;

namespace EllieBot.Brain {

    public class CommandProcessor : ICommandProcessor {
        private readonly Dictionary<string, ICommandExecutor> commands;

        public CommandProcessor() {
            this.commands = new Dictionary<string, ICommandExecutor>();
        }

        public void RegisterCommand(ICommandExecutor executor) {
            string key = executor.Command;
            if (!string.IsNullOrWhiteSpace(key) && executor != null) {
                this.commands.Add(key.Trim().ToUpper(), executor);
            }
        }

        public void Initialize() {
        }

        public void QueueExecute(RobotCommand cmd) {
            if (cmd == null || string.IsNullOrWhiteSpace(cmd.Command)) {
                return;
            }
            if (this.commands.TryGetValue(cmd.Command.Trim().ToUpper(), out ICommandExecutor executor)) {
                executor?.Execute(cmd.Arguments);
            }
        }
    }
}
