using EllieBot.Ambulator;
using System.Collections.Generic;

namespace EllieBot.Brain
{
    public class CommandProcessor : ICommandProcessor
    {
        private readonly IMotorsController Motors;
        private Dictionary<string, ICommandExecutor> commands;

        public CommandProcessor(IMotorsController motorController)
        {
            this.Motors = motorController;
            commands = new Dictionary<string, ICommandExecutor>();
        }

        public void RegisterCommand(string key, ICommandExecutor executor)
        {
            if (key != null && executor != null)
            {
                this.commands.Add(key.Trim().ToUpper(), executor);
            }
        }

        public void Initialize()
        {
        }

        public void QueueExecute(RobotCommand cmd)
        {
            if (cmd == null || string.IsNullOrWhiteSpace(cmd.Command))
            {
                return;
            }
            if (commands.TryGetValue(cmd.Command.Trim().ToUpper(), out ICommandExecutor executor))
            {
                executor?.Execute(cmd.Arguments);
            }
        }
    }
}