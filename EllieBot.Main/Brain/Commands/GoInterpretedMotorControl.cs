using EllieBot.Ambulator;
using System;

namespace EllieBot.Brain.Commands
{
    internal class GoInterpretedMotorControl : ICommandExecutor
    {
        public string Command => "GO_INTERP";

        public IMotorsController Motors { get; }

        public GoInterpretedMotorControl(IMotorsController motorsController) => this.Motors = motorsController;

        public void Execute(string[] commandArguments)
        {
            if (commandArguments.Length != 2)
            {
                return;
            }
            string command = commandArguments[0];

            if (string.IsNullOrWhiteSpace(command))
            {
                return;
            }

            double leftDuty = 0;
            double rightDuty = 0;
            double effort;
            switch (command.Trim().ToUpper())
            {
                case "FORWARD":
                    effort = double.Parse(commandArguments[1]);
                    leftDuty = Math.Abs(effort);
                    rightDuty = Math.Abs(effort);
                    break;

                case "BACKWARD":
                    effort = double.Parse(commandArguments[1]);
                    leftDuty = -Math.Abs(effort);
                    rightDuty = -Math.Abs(effort);
                    break;

                case "LEFT":
                    effort = double.Parse(commandArguments[1]);
                    leftDuty = -Math.Abs(effort);
                    rightDuty = Math.Abs(effort);
                    break;

                case "RIGHT":
                    effort = double.Parse(commandArguments[1]);
                    leftDuty = Math.Abs(effort);
                    rightDuty = -Math.Abs(effort);
                    break;

                case "STOP":
                default:
                    leftDuty = 0;
                    rightDuty = 0;
                    break;
            }

            this.Motors.SetDutyCycles(leftDuty, rightDuty);
        }
    }
}
