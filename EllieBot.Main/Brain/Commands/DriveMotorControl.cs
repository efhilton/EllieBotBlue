using EllieBot.IO;
using System;

namespace EllieBot.Brain.Commands {

    internal class DriveMotorControl : ICommandExecutor {
        private readonly string leftMotorId;
        private readonly string rightMotorId;

        public DriveMotorControl(string leftMotorUniqueId, string rightMotorUniqueId, IPWMController motorsController) {
            this.leftMotorId = leftMotorUniqueId;
            this.rightMotorId = rightMotorUniqueId;
            this.Motors = motorsController;
        }

        public string[] Commands => new string[] { Constants.Commands.Go.LEFT, Constants.Commands.Go.RIGHT, Constants.Commands.Go.FORWARD, Constants.Commands.Go.BACKWARD, Constants.Commands.Go.STOP, Constants.Commands.Go.TANK };
        public IPWMController Motors { get; }

        public void Execute(CommandPacket command) {
            if (command == null
                || string.IsNullOrWhiteSpace(command.Command)
                || command.Arguments == null) {
                return;
            }

            double leftDuty = 0;
            double rightDuty = 0;
            double effort;
            switch (command.Command.Trim().ToLower()) {
                case Constants.Commands.Go.FORWARD:
                    if (command.Arguments.Length != 1) {
                        throw new ArgumentException("Expected 1 argument");
                    }
                    effort = double.Parse(command.Arguments[0] ?? "0");
                    leftDuty = Math.Abs(effort);
                    rightDuty = Math.Abs(effort);
                    break;

                case Constants.Commands.Go.BACKWARD:
                    if (command.Arguments.Length != 1) {
                        throw new ArgumentException("Expected 1 argument");
                    }
                    effort = double.Parse(command.Arguments[0] ?? "0");
                    leftDuty = -Math.Abs(effort);
                    rightDuty = -Math.Abs(effort);
                    break;

                case Constants.Commands.Go.LEFT:
                    if (command.Arguments.Length != 1) {
                        throw new ArgumentException("Expected 1 argument");
                    }
                    effort = double.Parse(command.Arguments[0] ?? "0");
                    leftDuty = -Math.Abs(effort);
                    rightDuty = Math.Abs(effort);
                    break;

                case Constants.Commands.Go.RIGHT:
                    if (command.Arguments.Length != 1) {
                        throw new ArgumentException("Expected 1 argument");
                    }
                    effort = double.Parse(command.Arguments[0] ?? "0");
                    leftDuty = Math.Abs(effort);
                    rightDuty = -Math.Abs(effort);
                    break;

                case Constants.Commands.Go.TANK:
                    if (command.Arguments.Length != 2) {
                        throw new ArgumentException("Expected 2 arguments");
                    }
                    double forwardSpeed = double.Parse(command.Arguments[0] ?? "0");
                    double antiClockwiseSpin = double.Parse(command.Arguments[1] ?? "0");
                    leftDuty = forwardSpeed - antiClockwiseSpin;
                    rightDuty = forwardSpeed + antiClockwiseSpin;
                    break;

                case Constants.Commands.Go.STOP:
                default:
                    leftDuty = 0;
                    rightDuty = 0;
                    break;
            }

            this.Motors.SetDutyCycle(this.leftMotorId, leftDuty);
            this.Motors.SetDutyCycle(this.rightMotorId, rightDuty);
        }
    }
}
