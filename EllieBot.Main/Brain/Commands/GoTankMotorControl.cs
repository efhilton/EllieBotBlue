using EllieBot.Ambulator;

namespace EllieBot.Brain.Commands
{
    internal class GoTankMotorControl : ICommandExecutor
    {
        public string Command => "GO_TANK";

        public IMotorsController Motors { get; }

        public GoTankMotorControl(IMotorsController motorsController) => this.Motors = motorsController;

        public void Execute(string[] commandArguments)
        {
            if (commandArguments.Length != 2)
            {
                return;
            }
            double forwardSpeed = double.Parse(commandArguments[0]);
            double antiClockwiseSpin = double.Parse(commandArguments[1]);
            double leftDuty = forwardSpeed - antiClockwiseSpin;
            double rightDuty = forwardSpeed + antiClockwiseSpin;

            this.Motors.SetDutyCycles(leftDuty, rightDuty);
        }
    }
}
