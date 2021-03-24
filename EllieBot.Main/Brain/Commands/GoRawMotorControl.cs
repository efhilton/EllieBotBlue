using EllieBot.Ambulator;

namespace EllieBot.Brain.Commands
{
    internal class GoRawMotorControl : ICommandExecutor
    {
        public string Command => "GO_RAW";

        public IMotorsController Motors { get; }

        public GoRawMotorControl(IMotorsController motorsController) => this.Motors = motorsController;

        public void Execute(string[] commandArguments)
        {
            if (commandArguments.Length != 2)
            {
                return;
            }
            double leftDuty = double.Parse(commandArguments[0]);
            double rightDuty = double.Parse(commandArguments[1]);
            this.Motors.SetDutyCycles(leftDuty, rightDuty);
        }
    }
}
